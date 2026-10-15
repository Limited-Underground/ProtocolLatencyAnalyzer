using System.Globalization;
using Microsoft.Data.Sqlite;
using ProtocolLatencyAnalyzer.Core;

namespace ProtocolLatencyAnalyzer.Infrastructure;

public sealed class SessionRepository(string dataDirectory)
{
    private readonly string directory = Path.GetFullPath(dataDirectory);
    public string GetSessionPath(Guid id) => Path.Combine(directory, id.ToString("N") + ".plasession");
    private SqliteConnection Connect(Guid id, bool readOnly) => new(new SqliteConnectionStringBuilder
    {
        DataSource = GetSessionPath(id), Mode = readOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWriteCreate,
        Pooling = false, DefaultTimeout = 5
    }.ToString());

    private static async Task Execute(SqliteConnection connection, SqliteTransaction? transaction, string sql, CancellationToken ct)
    {
        using var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = sql;
        await command.ExecuteNonQueryAsync(ct);
    }
    private static async Task<int> Version(SqliteConnection connection, SqliteTransaction? transaction, CancellationToken ct)
    {
        using var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = "PRAGMA user_version";
        return Convert.ToInt32(await command.ExecuteScalarAsync(ct), CultureInfo.InvariantCulture);
    }

    public async Task SaveAsync(SessionDocument document, CancellationToken cancellationToken = default)
    {
        document.Validate(); cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(directory);
        using var connection = Connect(document.Id, false); await connection.OpenAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        var version = await Version(connection, transaction, cancellationToken);
        if (version is not (0 or SessionDocument.CurrentSchemaVersion)) throw new InvalidDataException("Unsupported session database version; no changes made.");
        if (version == 0)
        {
            // A versionless existing database must be empty; never adopt unrelated tables.
            using var inspect = connection.CreateCommand(); inspect.Transaction = transaction;
            inspect.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE name NOT LIKE 'sqlite_%'";
            if (Convert.ToInt64(await inspect.ExecuteScalarAsync(cancellationToken)) != 0) throw new InvalidDataException("Unrecognized database.");
            await Execute(connection, transaction, "CREATE TABLE session(id TEXT PRIMARY KEY,name TEXT NOT NULL,created TEXT NOT NULL,interface TEXT,origin TEXT NOT NULL); CREATE TABLE events(sequence INTEGER PRIMARY KEY,time TEXT NOT NULL,category TEXT NOT NULL,message TEXT NOT NULL); PRAGMA user_version=1;", cancellationToken);
        }
        else
        {
            using var identity = connection.CreateCommand(); identity.Transaction = transaction;
            identity.CommandText = "SELECT id FROM session LIMIT 2";
            using var reader = await identity.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken) || !Guid.TryParse(reader.GetString(0), out var existingId)
                || existingId != document.Id || await reader.ReadAsync(cancellationToken))
                throw new InvalidDataException("Existing session identity mismatch; no changes made.");
        }
        await Execute(connection, transaction, "DELETE FROM events; DELETE FROM session;", cancellationToken);
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction; command.CommandText = "INSERT INTO session VALUES($id,$name,$created,$interface,$origin)";
            command.Parameters.AddWithValue("$id", document.Id.ToString()); command.Parameters.AddWithValue("$name", document.Name);
            command.Parameters.AddWithValue("$created", document.CreatedUtc.ToString("O"));
            command.Parameters.AddWithValue("$interface", (object?)document.SelectedInterfaceId ?? DBNull.Value);
            command.Parameters.AddWithValue("$origin", document.Origin); await command.ExecuteNonQueryAsync(cancellationToken);
        }
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction; command.CommandText = "INSERT INTO events VALUES($sequence,$time,$category,$message)";
            var sequence = command.Parameters.Add("$sequence", SqliteType.Integer); var time = command.Parameters.Add("$time", SqliteType.Text);
            var category = command.Parameters.Add("$category", SqliteType.Text); var message = command.Parameters.Add("$message", SqliteType.Text);
            int i = 0;
            foreach (var entry in document.Events)
            {
                cancellationToken.ThrowIfCancellationRequested(); sequence.Value = i++; time.Value = entry.TimestampUtc.ToString("O");
                category.Value = entry.Category; message.Value = entry.Message; await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        cancellationToken.ThrowIfCancellationRequested(); transaction.Commit();
    }

    public Task<SessionDocument> OpenAsync(Guid id, CancellationToken cancellationToken = default) => OpenFileAsync(GetSessionPath(id), cancellationToken, id);
    public async Task<SessionDocument> OpenFileAsync(string explicitPath, CancellationToken cancellationToken = default, Guid? expectedId = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var file = new FileInfo(Path.GetFullPath(explicitPath));
        if (!file.Exists) throw new FileNotFoundException("Session not found.", file.FullName);
        if (file.Length > 128 * 1024 * 1024) throw new InvalidDataException("Session exceeds file size limit.");
        using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = file.FullName, Mode = SqliteOpenMode.ReadOnly, Pooling = false, DefaultTimeout = 5 }.ToString()); await connection.OpenAsync(cancellationToken);
        using var transaction = connection.BeginTransaction(deferred: true);
        if (await Version(connection, transaction, cancellationToken) != SessionDocument.CurrentSchemaVersion) throw new InvalidDataException("Unsupported session database version.");
        SessionDocument result;
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction; command.CommandText = "SELECT id,name,created,interface,origin FROM session LIMIT 2";
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken)) throw new InvalidDataException("Session metadata missing.");
            result = new SessionDocument { Id = Guid.Parse(reader.GetString(0)), Name = reader.GetString(1), CreatedUtc = DateTimeOffset.Parse(reader.GetString(2), CultureInfo.InvariantCulture), SelectedInterfaceId = reader.IsDBNull(3) ? null : reader.GetString(3), Origin = reader.GetString(4) };
            if ((expectedId.HasValue && result.Id != expectedId.Value) || await reader.ReadAsync(cancellationToken)) throw new InvalidDataException("Session identity mismatch.");
        }
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction; command.CommandText = "SELECT time,category,message FROM events ORDER BY sequence LIMIT 10001";
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (result.Events.Count >= SessionDocument.MaxEvents) throw new InvalidDataException("Session exceeds event limit.");
                result.Events.Add(new SessionEvent(DateTimeOffset.Parse(reader.GetString(0), CultureInfo.InvariantCulture), reader.GetString(1), reader.GetString(2)));
            }
        }
        result.Validate(); transaction.Commit(); return result;
    }
}

