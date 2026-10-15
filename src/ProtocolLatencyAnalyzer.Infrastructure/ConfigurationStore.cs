using System.Text.Json;
using ProtocolLatencyAnalyzer.Core;

namespace ProtocolLatencyAnalyzer.Infrastructure;

public sealed class ConfigurationStore(string filePath)
{
    private readonly string path = Path.GetFullPath(filePath);
    public async Task<AnalyzerConfiguration> LoadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(path)) return new AnalyzerConfiguration();
        if (new FileInfo(path).Length > 65536) throw new InvalidDataException("Configuration exceeds size limit.");
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var value = await JsonSerializer.DeserializeAsync<AnalyzerConfiguration>(stream, cancellationToken: cancellationToken)
                    ?? throw new InvalidDataException("Empty configuration.");
        value.Validate();
        return value;
    }

    public async Task SaveAsync(AnalyzerConfiguration configuration, CancellationToken cancellationToken = default)
    {
        configuration.Validate();
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await JsonSerializer.SerializeAsync(stream, configuration, cancellationToken: cancellationToken);
                await stream.FlushAsync(cancellationToken);
                stream.Flush(flushToDisk: true);
            }
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporary, path, overwrite: true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }
}
