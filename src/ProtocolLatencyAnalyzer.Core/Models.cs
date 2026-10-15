namespace ProtocolLatencyAnalyzer.Core;

public sealed record SessionEvent(DateTimeOffset TimestampUtc, string Category, string Message);

public sealed class SessionDocument
{
    public const int CurrentSchemaVersion = 1;
    public const int MaxEvents = 10_000;
    public int SchemaVersion { get; set; } = CurrentSchemaVersion;
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Untitled session";
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? SelectedInterfaceId { get; set; }
    public string Origin { get; set; } = "UserSession";
    public List<SessionEvent> Events { get; set; } = [];

    public void Validate()
    {
        if (SchemaVersion != CurrentSchemaVersion) throw new InvalidDataException("Unsupported session schema version.");
        if (Id == Guid.Empty || string.IsNullOrWhiteSpace(Name) || Name.Length > 256) throw new InvalidDataException("Invalid session identity or name.");
        if (Origin is not ("UserSession" or "Simulation")) throw new InvalidDataException("Invalid session origin.");
        if (SelectedInterfaceId?.Length > 512 || Events is null || Events.Count > MaxEvents) throw new InvalidDataException("Session exceeds its limits.");
        foreach (var e in Events)
            if (e is null || string.IsNullOrWhiteSpace(e.Category) || e.Category.Length > 64 || e.Message is null || e.Message.Length > 2048)
                throw new InvalidDataException("Invalid session event.");
    }
}

public sealed class AnalyzerConfiguration
{
    public int SchemaVersion { get; set; } = 1;
    public string? SelectedInterfaceId { get; set; }
    public int MaxVisibleEvents { get; set; } = 1000;
    public void Validate()
    {
        if (SchemaVersion != 1) throw new InvalidDataException("Unsupported configuration schema version.");
        if (SelectedInterfaceId?.Length > 512 || MaxVisibleEvents is < 10 or > SessionDocument.MaxEvents)
            throw new InvalidDataException("Invalid configuration limits.");
    }
}

public sealed record EthernetInterfaceInfo(string Id, string Name, string OperationalStatus, long SpeedBitsPerSecond, IReadOnlyList<string> Addresses);
