using Microsoft.Data.Sqlite;
using ProtocolLatencyAnalyzer.Core;
using ProtocolLatencyAnalyzer.Infrastructure;

string storageRoot;
if (args.Length > 0 && args[0] == "--storage-root")
{
    if (args.Length != 2 || !Path.IsPathFullyQualified(args[1]))
        throw new ArgumentException("--storage-root requires one absolute existing .artifacts directory path.");
    storageRoot = Path.GetFullPath(args[1]);
    if (!storageRoot.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
            .Contains(".artifacts", StringComparer.OrdinalIgnoreCase) || !Directory.Exists(storageRoot))
        throw new ArgumentException("--storage-root must exist and contain a .artifacts directory segment.");
}
else storageRoot = Path.GetFullPath(args.Length > 0 ? args[0] : ".artifacts");
var root = Path.Combine(storageRoot, "PLA-offline-tests-" + Guid.NewGuid().ToString("N"));
if (Directory.Exists(root) || File.Exists(root)) throw new IOException("Unique test directory already exists.");
Directory.CreateDirectory(root);
int passed = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); Console.WriteLine("PASS " + name); passed++; }
async Task Reject<T>(Func<Task> action, string name) where T : Exception
{ try { await action(); } catch (T) { Check(true, name); return; } throw new Exception("Expected rejection: " + name); }
async Task RejectLockedReplacement(Func<Task> action)
{
    try { await action(); }
    catch (Exception error) when (error is IOException or UnauthorizedAccessException)
    { Check(true, "failed atomic replacement surfaces"); return; }
    throw new Exception("Expected locked-target replacement rejection.");
}
try
{
    EthernetInterfaceInfo Adapter(string id, string status = "Up", string ip = "192.0.2.10", bool gateway = true, bool virtualHint = false)
        => new(id, id, status, 1_000_000_000, [ip]) { HasGateway = gateway, IsLikelyVirtual = virtualHint };
    var adapterChoices = EthernetInterfaceProvider.RankCandidates([
        Adapter("virtual", virtualHint: true), Adapter("down", status: "Down"),
        Adapter("lan", gateway: false), Adapter("uplink"), Adapter("apipa", ip: "169.254.2.3"),
        Adapter("ipv6-local", ip: "fe80::1"), Adapter("unspecified", ip: "0.0.0.0")]);
    Check(adapterChoices.First().Id == "uplink" && adapterChoices.Count(a => a.IsRecommended) == 1, "connected gateway candidate ranks first without auto selection");
    Check(!adapterChoices.Single(a => a.Id == "down").IsRecommended && adapterChoices.Single(a => a.Id == "down").ConnectionSummary == "Disconnected", "disconnected adapter never suggested despite gateway");
    Check(!adapterChoices.Single(a => a.Id == "lan").IsRecommended && adapterChoices.Single(a => a.Id == "lan").RecommendationReason.Contains("local network"), "connected LAN without gateway remains available");
    Check(adapterChoices.Where(a => a.Id is "apipa" or "ipv6-local" or "unspecified").All(a => !a.IsRecommended), "link-local and unspecified addresses do not qualify as uplinks");
    Check(!adapterChoices.Single(a => a.Id == "virtual").IsRecommended && adapterChoices.Single(a => a.Id == "uplink").AdapterKindSummary.Contains("unverified"), "virtual hint preference never proves physical hardware");
    var tied = EthernetInterfaceProvider.RankCandidates([Adapter("zeta"), Adapter("alpha")]);
    Check(tied.All(a => a.IsRecommended && a.RecommendationReason.Contains("Multiple adapters")) && tied.First().Id == "alpha", "equal gateway candidates show ambiguity with stable alphabetical order");
    var reordered = EthernetInterfaceProvider.RankCandidates([Adapter("alpha"), Adapter("zeta")]);
    Check(tied.Select(a => (a.Id, a.IsRecommended, a.RecommendationReason))
        .SequenceEqual(reordered.Select(a => (a.Id, a.IsRecommended, a.RecommendationReason))), "recommendation independent of enumeration order");
    var onlyVirtual = EthernetInterfaceProvider.RankCandidates([Adapter("virtual", virtualHint: true)]).Single();
    Check(onlyVirtual.IsRecommended && onlyVirtual.RecommendationReason.Contains("virtual") && onlyVirtual.RecommendationReason.Contains("destination route is unverified"), "only virtual candidate explains evidence and routing limit");
    Check(EthernetInterfaceProvider.RankCandidates([Adapter("ipv6", ip: "2001:db8::10")]).Single().IsRecommended, "IPv6 global address and gateway can qualify");
    Check(EthernetInterfaceProvider.HasVirtualHint("vEthernet", "Hyper-V adapter") && !EthernetInterfaceProvider.HasVirtualHint("Ethernet", "Generic adapter"), "virtual classification uses explicit name hints");
    Check(!EthernetInterfaceProvider.RankCandidates([Adapter("local", gateway: false)]).Any(a => a.IsRecommended) && EthernetInterfaceProvider.RankCandidates([]).Count == 0, "no suitable candidate and empty inventory produce no suggestion");
    var configuration = new ConfigurationStore(Path.Combine(root, "settings.json"));
    Check((await configuration.LoadAsync()).SchemaVersion == 1, "missing configuration gives defaults");
    await configuration.SaveAsync(new AnalyzerConfiguration { SelectedInterfaceId = "synthetic-interface", MaxVisibleEvents = 50 });
    Check((await configuration.LoadAsync()).MaxVisibleEvents == 50, "configuration round trip");
    await Reject<InvalidDataException>(() => configuration.SaveAsync(new AnalyzerConfiguration { SchemaVersion = 2 }), "future configuration rejected");
    Check((await configuration.LoadAsync()).MaxVisibleEvents == 50, "invalid save preserves configuration");
    using (var locked = new FileStream(Path.Combine(root, "settings.json"), FileMode.Open, FileAccess.Read, FileShare.None))
        await RejectLockedReplacement(() => configuration.SaveAsync(new AnalyzerConfiguration { MaxVisibleEvents = 75 }));
    Check((await configuration.LoadAsync()).MaxVisibleEvents == 50, "failed replacement preserves original configuration");
    Check(!Directory.EnumerateFiles(root, "*.tmp").Any(), "failed replacement cleans temporary file");
    if (OperatingSystem.IsWindows())
    {
        var settingsPath = Path.Combine(root, "settings.json");
        var originalSettings = await File.ReadAllBytesAsync(settingsPath);
        var attributes = File.GetAttributes(settingsPath);
        try
        {
            File.SetAttributes(settingsPath, attributes | FileAttributes.ReadOnly);
            try { await configuration.SaveAsync(new AnalyzerConfiguration { MaxVisibleEvents = 75 }); throw new InvalidOperationException("Expected read-only configuration rejection."); }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException)
            { Check(true, "Windows read-only configuration overwrite rejected"); }
            Check((await File.ReadAllBytesAsync(settingsPath)).SequenceEqual(originalSettings)
                && (await configuration.LoadAsync()).MaxVisibleEvents == 50, "read-only configuration preserves valid original bytes and state");
            Check(!Directory.EnumerateFiles(root, "*.tmp").Any(), "read-only configuration failure cleans temporary file");
        }
        finally { File.SetAttributes(settingsPath, attributes); }
    }
    await File.WriteAllTextAsync(Path.Combine(root, "settings.json"), "{\"SchemaVersion\":2}");
    await Reject<InvalidDataException>(() => configuration.LoadAsync(), "future configuration read rejected");
    var repository = new SessionRepository(root);
    var session = new SessionDocument { Name = "Synthetic ' ; SQL text", Origin = "Simulation", SelectedInterfaceId = "synthetic-interface" };
    session.Events.Add(new SessionEvent(DateTimeOffset.UtcNow, "Simulation", "Synthetic event; no measurement"));
    await repository.SaveAsync(session);
    var opened = await repository.OpenAsync(session.Id);
    Check(opened.Name == session.Name && opened.Events.SequenceEqual(session.Events) && opened.SelectedInterfaceId == session.SelectedInterfaceId && opened.Origin == "Simulation", "SQLite parameterized round trip");
    session.Name = "Updated"; await repository.SaveAsync(session);
    Check((await repository.OpenAsync(session.Id)).Events.Count == 1, "save replaces instead of duplicating events");
    if (OperatingSystem.IsWindows())
    {
        var sessionPath = repository.GetSessionPath(session.Id);
        var originalDatabase = await File.ReadAllBytesAsync(sessionPath);
        var attributes = File.GetAttributes(sessionPath);
        try
        {
            File.SetAttributes(sessionPath, attributes | FileAttributes.ReadOnly);
            session.Name = "Must not replace read-only session";
            try { await repository.SaveAsync(session); throw new InvalidOperationException("Expected read-only session rejection."); }
            catch (SqliteException error) when (error.SqliteErrorCode == 8)
            { Check(true, "Windows read-only SQLite session save rejected"); }
            var preserved = await repository.OpenAsync(session.Id);
            Check((await File.ReadAllBytesAsync(sessionPath)).SequenceEqual(originalDatabase)
                && preserved.Name == "Updated" && preserved.Events.SequenceEqual(opened.Events)
                && preserved.SelectedInterfaceId == opened.SelectedInterfaceId, "read-only SQLite session preserves valid original bytes and state");
        }
        finally { File.SetAttributes(sessionPath, attributes); session.Name = "Updated"; }
    }
    using (var connection = new SqliteConnection($"Data Source={repository.GetSessionPath(session.Id)};Pooling=False"))
    {
        await connection.OpenAsync(); using var command = connection.CreateCommand();
        command.CommandText = "CREATE TRIGGER reject_event BEFORE INSERT ON events BEGIN SELECT RAISE(ABORT, 'test fault'); END";
        await command.ExecuteNonQueryAsync();
    }
    session.Name = "Must roll back";
    await Reject<SqliteException>(() => repository.SaveAsync(session), "database write fault surfaces");
    Check((await repository.OpenAsync(session.Id)).Name == "Updated" && (await repository.OpenAsync(session.Id)).Events.Count == 1, "transaction rolls back deleted metadata and events");
    using (var connection = new SqliteConnection($"Data Source={repository.GetSessionPath(session.Id)};Pooling=False"))
    {
        await connection.OpenAsync(); using var command = connection.CreateCommand(); command.CommandText = "DROP TRIGGER reject_event"; await command.ExecuteNonQueryAsync();
    }
    using var canceled = new CancellationTokenSource(); canceled.Cancel();
    session.Name = "Canceled write";
    await Reject<OperationCanceledException>(() => repository.SaveAsync(session, canceled.Token), "canceled save rejected");
    Check((await repository.OpenAsync(session.Id)).Name == "Updated", "canceled save preserves database");
    session.Events = Enumerable.Repeat(session.Events[0], SessionDocument.MaxEvents + 1).ToList();
    await Reject<InvalidDataException>(() => repository.SaveAsync(session), "oversize event list rejected");
    Check((await repository.OpenAsync(session.Id)).Events.Count == 1, "oversize save preserves database");
    using (var connection = new SqliteConnection($"Data Source={repository.GetSessionPath(session.Id)};Pooling=False"))
    {
        await connection.OpenAsync(); using var command = connection.CreateCommand(); command.CommandText = "PRAGMA user_version=2"; await command.ExecuteNonQueryAsync();
    }
    await Reject<InvalidDataException>(() => repository.OpenAsync(session.Id), "future session read rejected");
    session.Events = []; await Reject<InvalidDataException>(() => repository.SaveAsync(session), "future session overwrite rejected");
    using (var connection = new SqliteConnection($"Data Source={repository.GetSessionPath(session.Id)};Pooling=False"))
    {
        await connection.OpenAsync(); using var command = connection.CreateCommand(); command.CommandText = "PRAGMA user_version";
        Check(Convert.ToInt32(await command.ExecuteScalarAsync()) == 2, "future schema marker preserved after rejected overwrite");
        command.CommandText = "SELECT name FROM session";
        Check((string?)await command.ExecuteScalarAsync() == "Updated", "future schema content preserved after rejected overwrite");
    }
    var blockedDirectory = Path.Combine(root, "ordinary-file"); await File.WriteAllTextAsync(blockedDirectory, "test");
    await Reject<IOException>(() => new SessionRepository(blockedDirectory).SaveAsync(new SessionDocument()), "storage failure surfaces");
    await Reject<FileNotFoundException>(() => repository.OpenAsync(Guid.NewGuid()), "missing session does not create database");
    var corruptPath = Path.Combine(root, "corrupt.plasession"); await File.WriteAllTextAsync(corruptPath, "not a database");
    await Reject<SqliteException>(() => repository.OpenFileAsync(corruptPath), "corrupted session rejected");
    var misnamed = new SessionDocument { Name = "Original identity" }; await repository.SaveAsync(misnamed);
    var replacement = new SessionDocument { Name = "Must not overwrite" };
    File.Copy(repository.GetSessionPath(misnamed.Id), repository.GetSessionPath(replacement.Id));
    await Reject<InvalidDataException>(() => repository.SaveAsync(replacement), "misnamed database overwrite rejected");
    Check((await repository.OpenFileAsync(repository.GetSessionPath(replacement.Id))).Id == misnamed.Id, "mismatched database identity preserved");
    Console.WriteLine($"{passed} offline checks passed. No network tests or capture performed.");
    return 0;
}
catch (Exception error) { Console.Error.WriteLine(error); return 1; }
finally
{
    SqliteConnection.ClearAllPools();
    if (!string.Equals(Path.GetDirectoryName(Path.GetFullPath(root)), Path.TrimEndingDirectorySeparator(storageRoot),
            OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
        throw new IOException("Refusing cleanup outside the owned test directory.");
    Directory.Delete(root, recursive: true);
}

