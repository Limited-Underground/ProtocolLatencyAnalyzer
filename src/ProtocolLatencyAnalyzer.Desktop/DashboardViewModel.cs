using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ProtocolLatencyAnalyzer.Desktop;

public sealed record InterfaceChoice(string Id, string DisplayName, string Details, string RecommendationReason = "");
public sealed record ProtocolRow(string Name, string State, string Timing);
public sealed record EventRow(string Context, string Detail);

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private bool isDemo;
    private InterfaceChoice? selected;
    private string status = "Ready • no traffic sent or captured";
    private string title = "Dashboard";
    private string subtitle = "Network and protocol diagnostics";
    private readonly Queue<EventRow> diagnostics = new();
    private int maxVisibleEvents = 1000;
    private string interfaceGuidance = "Refresh to list local Ethernet adapters. Select the adapter for the network you want to inspect.";
    public string InterfaceGuidance { get => interfaceGuidance; set { interfaceGuidance = value; Notify(); } }
    public long DroppedDiagnosticCount { get; private set; }
    public int MaxVisibleEvents { get => maxVisibleEvents; set { maxVisibleEvents = Math.Clamp(value, 10, 10_000); while (diagnostics.Count > maxVisibleEvents) { diagnostics.Dequeue(); DroppedDiagnosticCount++; } RefreshPreview(); Notify(nameof(DroppedDiagnosticCount)); Notify(nameof(Status)); } }
    public ObservableCollection<InterfaceChoice> Interfaces { get; } = [];
    public ObservableCollection<ProtocolRow> ProtocolRows { get; } = [];
    public ObservableCollection<EventRow> Events { get; } = [];
    public InterfaceChoice? SelectedInterface { get => selected; set { if (selected == value) return; selected = value; Notify(); Notify(nameof(InterfaceDetails)); InterfaceChanged?.Invoke(); } }
    public string InterfaceDetails => SelectedInterface?.Details ?? "No Ethernet adapter selected. Refresh lists local adapters only.";
    public bool IsDemo { get => isDemo; set { isDemo = value; RefreshPreview(); Notify(); Notify(nameof(DemoVisibility)); Notify(nameof(ChartCaption)); Notify(nameof(ChartEmpty)); PreviewChanged?.Invoke(); } }
    public Visibility DemoVisibility => IsDemo ? Visibility.Visible : Visibility.Collapsed;
    public string ChartCaption => IsDemo ? "Synthetic ICMP-style samples • illustrative ms • 24 fixed points" : "No measured samples. Measurements begin in a later milestone.";
    public string ChartEmpty => IsDemo ? "" : "No session measurements";
    public string Status { get => DroppedDiagnosticCount == 0 ? status : $"{status} • {DroppedDiagnosticCount} older diagnostic entries discarded"; set { status = value; Notify(); } }
    public string PageTitle { get => title; set { title = value; Notify(); } }
    public string PageSubtitle { get => subtitle; set { subtitle = value; Notify(); } }
    public event Action? InterfaceChanged;
    public event Action? PreviewChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
    public DashboardViewModel() => RefreshPreview();
    private void RefreshPreview()
    {
        ProtocolRows.Clear(); Events.Clear();
        foreach (var p in new[] { "ICMP", "TCP connect", "DNS", "HTTP / HTTPS" })
            ProtocolRows.Add(new(p, IsDemo ? "Synthetic preview" : "Not implemented", IsDemo ? "12 ms (demo)" : "Unavailable"));
        if (IsDemo) Events.Add(new("DEMONSTRATION", "Fixed synthetic preview enabled; no measurements acquired."));
        foreach (var entry in diagnostics.TakeLast(Math.Max(0, MaxVisibleEvents - Events.Count))) Events.Add(entry);
    }
    public void RecordDiagnostic(string outcome)
    {
        diagnostics.Enqueue(new("Foundation operation", outcome));
        while (diagnostics.Count > MaxVisibleEvents) { diagnostics.Dequeue(); DroppedDiagnosticCount++; }
        RefreshPreview();
        Notify(nameof(DroppedDiagnosticCount)); Notify(nameof(Status));
    }
    private void Notify([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}
