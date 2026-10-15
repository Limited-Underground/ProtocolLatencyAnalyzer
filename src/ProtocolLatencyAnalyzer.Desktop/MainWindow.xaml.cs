using Microsoft.Win32;
using ProtocolLatencyAnalyzer.Core;
using ProtocolLatencyAnalyzer.Infrastructure;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace ProtocolLatencyAnalyzer.Desktop;

public partial class MainWindow : Window
{
    public static RoutedUICommand ExportCommand { get; } = new("Export session metadata", nameof(ExportCommand), typeof(MainWindow), new InputGestureCollection { new KeyGesture(Key.E, ModifierKeys.Control) });
    public DashboardViewModel Model { get; } = new();
    private SessionDocument session = new();
    private string? dataDirectory;
    private bool dirty;
    private bool restoring;
    private bool busy;
    private Task<EthernetInterfaceInfo[]>? inventoryTask;
    private long inventoryGeneration;
    private bool windowClosed;
    private readonly bool previewMode;
    public MainWindow() : this(false) { }
    public MainWindow(bool renderPreview)
    {
        previewMode = renderPreview;
        InitializeComponent(); DataContext = Model;
        Model.InterfaceChanged += () => { if (!restoring && !previewMode) { session.SelectedInterfaceId = Model.SelectedInterface?.Id; MarkDirty(); } };
        Model.PreviewChanged += UpdateMetrics;
        if (renderPreview)
        {
            Model.Interfaces.Add(new("synthetic", "Demonstration Ethernet adapter • Connected • Suggested", "Synthetic adapter • documentation address 192.0.2.10 • Connected (synthetic)\nSuggested example only: simulated link-up, address and gateway metadata. No inventory performed."));
            Model.InterfaceGuidance = "Synthetic example: Connected / Suggested labels identify a likely choice.";
            Model.SelectedInterface = Model.Interfaces[0]; Model.IsDemo = true;
        }
        else Loaded += (_, _) => RefreshInterfaces();
        Closed += (_, _) => { windowClosed = true; inventoryGeneration++; };
        UpdateMetrics(); UpdateTitle();
    }
    private async void RefreshInterfaces()
    {
        if (previewMode || windowClosed) return;
        var generation = ++inventoryGeneration;
        // Restore only this session's chosen ID from the current list while the
        // operating-system inventory runs. Never inherit another session's choice.
        restoring = true;
        try { Model.SelectedInterface = Model.Interfaces.FirstOrDefault(x => x.Id == session.SelectedInterfaceId); }
        finally { restoring = false; }
        Model.InterfaceGuidance = "Reading local adapter metadata… Existing selection remains available.";
        // Native inventory is not cancellable. Retain a timed-out task rather
        // than starting more blocked enumerations on every Refresh click.
        if (inventoryTask is null)
        {
            inventoryTask = Task.Run(() => new EthernetInterfaceProvider().GetInterfaces().ToArray());
            _ = inventoryTask.ContinueWith(task => { _ = task.Exception; }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
        var pending = inventoryTask;
        try
        {
            var completed = await Task.WhenAny(pending, Task.Delay(TimeSpan.FromSeconds(10)));
            if (windowClosed || generation != inventoryGeneration) return;
            if (completed != pending)
            {
                Model.InterfaceGuidance = "Adapter inventory is taking longer than expected. Current list retained; refresh later to check again.";
                Model.Status = "Adapter inventory timed out • no acquisition • no overlapping retry started";
                return;
            }
            var interfaces = await pending;
            if (windowClosed || generation != inventoryGeneration) return;
            inventoryTask = null;
            // Read the desired ID now: the owner may have changed it while
            // inventory was pending. Applying metadata must not dirty the session.
            var id = session.SelectedInterfaceId;
            restoring = true; Model.Interfaces.Clear();
            foreach (var n in interfaces)
                Model.Interfaces.Add(new(n.Id, n.SelectionSummary, $"{n.Description}\n{n.ConnectionSummary} • {n.AdapterKindSummary} • {(n.SpeedBitsPerSecond > 0 ? $"{n.SpeedBitsPerSecond / 1_000_000:N0} Mbps" : "Link speed unavailable")}\nAddresses: {string.Join(", ", n.Addresses)}\n{n.RecommendationReason}{(string.IsNullOrEmpty(n.InventoryWarning) ? "" : $"\n{n.InventoryWarning}")}", n.IsRecommended ? n.RecommendationReason : ""));
            var recommended = Model.Interfaces.Where(x => !string.IsNullOrEmpty(x.RecommendationReason)).ToList();
            Model.InterfaceGuidance = Model.Interfaces.Count == 0 ? "No Ethernet adapters found. Connect or enable an Ethernet adapter, then refresh."
                : recommended.Count == 0 ? "No clear recommendation from local adapter metadata. Choose the adapter connected to the network you want to inspect."
                : recommended.Count == 1 ? $"Likely starting point: {recommended[0].DisplayName}."
                : $"{recommended.Count} adapters suggested. Choose the one for your intended network.";
            Model.SelectedInterface = Model.Interfaces.FirstOrDefault(x => x.Id == id);
            Model.Status = Model.Interfaces.Count == 0 ? "No Ethernet adapters found • no acquisition" : $"{Model.Interfaces.Count} Ethernet adapters inventoried • no acquisition";
        }
        catch
        {
            if (windowClosed || generation != inventoryGeneration) return;
            inventoryTask = null;
            Model.InterfaceGuidance = "Adapter inventory unavailable. Current list retained; refresh to try again.";
            Model.Status = "Ethernet inventory unavailable • no acquisition";
        }
        finally { restoring = false; }
    }
    private void UpdateMetrics()
    {
        if (Metrics is null) return;
        Metrics.Children.Clear();
        var values = new[] { ("Active tests", "0", "None running"), ("Protocols", Model.IsDemo ? "4" : "—", Model.IsDemo ? "Synthetic preview" : "No observations"), ("Transaction timing", Model.IsDemo ? "12 ms" : "—", Model.IsDemo ? "Demonstration" : "No samples"), ("Capture drops", "—", "Unavailable"), ("Connections", "—", "Not observed"), ("Alerts", "0", "No acquired evidence") };
        foreach (var (label, value, note) in values)
        {
            var stack = new StackPanel(); stack.Children.Add(new TextBlock { Text = label, MinHeight = 20 });
            stack.Children.Add(new TextBlock { Text = value, FontSize = 25, FontWeight = FontWeights.SemiBold, Margin = new(0,3,0,5) });
            stack.Children.Add(new TextBlock { Text = note, FontSize = 11 });
            Metrics.Children.Add(new Border { Style = (Style)FindResource("Card"), Child = stack });
        }
    }
    private void MarkDirty() { dirty = true; UpdateTitle(); }
    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.Control) return;
        RoutedCommand? command = e.Key switch
        {
            Key.N => ApplicationCommands.New,
            Key.O => ApplicationCommands.Open,
            Key.S => ApplicationCommands.Save,
            Key.E => ExportCommand,
            _ => null
        };
        if (command is null) return;
        // Handle before focused controls (for example TextBox Ctrl+E alignment),
        // and suppress repeat presses or a second routed execution.
        e.Handled = true;
        if (!e.IsRepeat && command.CanExecute(null, this)) command.Execute(null, this);
    }
    private void FileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = !busy; e.Handled = true; }
    private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) { e.CanExecute = !busy && !string.IsNullOrWhiteSpace(session.Name); e.Handled = true; }
    private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e) { e.Handled = true; New_Click(sender, e); }
    private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e) { e.Handled = true; Open_Click(sender, e); }
    private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) { e.Handled = true; Save_Click(sender, e); }
    private void ExportCommand_Executed(object sender, ExecutedRoutedEventArgs e) { e.Handled = true; Export_Click(sender, e); }
    private void Exit_Click(object sender, RoutedEventArgs e) { if (!busy) Close(); }
    private void UpdateTitle() => Title = $"{session.Name}{(dirty ? " *" : "")} — Protocol Latency Analyzer — By Limited Underground";
    private void Navigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DashboardPanel is null || Navigation.SelectedItem is not ListBoxItem item) return;
        ShowPage(item.Content?.ToString() ?? "Dashboard");
    }
    private void ShowPage(string page)
    {
        Model.PageTitle = page; Model.PageSubtitle = page == "Dashboard" ? "Network and protocol diagnostics" : "Phase 1 foundation • capabilities and session context";
        DashboardPanel.Visibility = page == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;
        DetailPanel.Visibility = page == "Dashboard" ? Visibility.Collapsed : Visibility.Visible;
        DetailContent.Children.Clear();
        void Text(string t) => DetailContent.Children.Add(new TextBlock { Text = t, Margin = new(0,0,0,14), MaxWidth = 850, HorizontalAlignment = HorizontalAlignment.Left });
        void Action(string t, RoutedEventHandler handler) { var b = new Button { Content = t, HorizontalAlignment = HorizontalAlignment.Left }; b.Click += handler; DetailContent.Children.Add(b); }
        switch (page)
        {
            case "Sessions":
                Text("Session name");
                var name = new TextBox { Text = session.Name, MaxLength = 200, Margin = new(0,0,0,15), MaxWidth = 650, HorizontalAlignment = HorizontalAlignment.Left, MinWidth = 350 };
                name.TextChanged += (_, _) => { session.Name = name.Text; MarkDirty(); }; DetailContent.Children.Add(name);
                Text($"Session ID: {session.Id}\nCreated: {session.CreatedUtc:u}\nData directory: {dataDirectory ?? "Choose explicitly when saving"}\nSaved session events: {session.Events.Count}\nDemonstration samples are never saved as acquired measurements.");
                Action("Save session…", Save_Click); Action("Open session…", Open_Click); break;
            case "Settings":
                Text("Storage is explicit. No automatic AppData location, telemetry, capture, driver installation, or test execution. This provisional light palette does not assert official brand colors.");
                Text($"Data directory: {dataDirectory ?? "Not selected"}\nVisible operation-log limit: {Model.MaxVisibleEvents}\nDiagnostics remain in memory and contain fixed operation outcomes only; no addresses, paths, credentials or exception text."); Action("Choose data directory…", ChooseDirectory_Click); Action("Save configuration…", SaveConfiguration_Click); Action("Load configuration…", LoadConfiguration_Click); break;
            case "Reports": Text("Foundation export contains session metadata and recorded session events only. It is not a measurement report and excludes synthetic preview readings. No payloads or credentials are collected in Phase 1."); Action("Export session metadata…", Export_Click); break;
            case "About": Text("Protocol Latency Analyzer\nBy Limited Underground\nApache-2.0\n\nWindows-first WPF application with an independent reusable core. Future Linux support requires its own UI, capture implementation, and validation.\n\nThis foundation cannot send protocol tests, capture packets, or discover remote services. Target OS qualification and official branding remain pending."); break;
            case "Active Tests": Text("Not implemented in Phase 1. No tests can be started. A later milestone will define bounded protocol tests and validate timing, cancellation, and actual source endpoints."); break;
            case "Custom Messages": Text("Not implemented. Message editing, exact-byte previews, framing, dry-run validation, transmission, and response matching require the separate custom-message milestone."); break;
            case "Passive Capture": Text("Not implemented. No capture session, privileged helper, or driver operation is available. Native Windows backend feasibility and per-OS qualification are pending."); break;
            case "Protocol Explorer": Text("No traffic has been inspected and no protocols have been discovered. Future protocol results must identify capture visibility, timing source, completeness, and matching rules."); break;
        }
    }
    private void Go_Click(object sender, RoutedEventArgs e) { var page = (sender as Button)?.Tag?.ToString(); foreach (ListBoxItem i in Navigation.Items) if (i.Content?.ToString() == page) Navigation.SelectedItem = i; }
    private void Settings_Click(object sender, RoutedEventArgs e) { Navigation.SelectedIndex = 7; ShowPage("Settings"); }
    private void Refresh_Click(object sender, RoutedEventArgs e) => RefreshInterfaces();
    private bool ConfirmDiscard()
    {
        if (busy) return false;
        if (!dirty) return true;
        return MessageBox.Show(this, "This session has unsaved changes. Discard those changes? Use Cancel to return and save first.", "Unsaved session", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
    }
    private void New_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard()) return;
        session = new(); dirty = false; Model.IsDemo = false; RefreshInterfaces(); UpdateTitle(); ShowPage(Model.PageTitle);
    }
    private void ChooseDirectory_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        var dialog = new OpenFolderDialog { Title = "Choose a writable portable data directory" };
        if (dialog.ShowDialog(this) == true) { dataDirectory = dialog.FolderName; Model.Status = "Data directory selected explicitly"; ShowPage("Settings"); }
    }
    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        if (string.IsNullOrWhiteSpace(session.Name)) { MessageBox.Show(this, "Enter a session name before saving."); return; }
        if (dataDirectory is null)
        {
            var dialog = new OpenFolderDialog { Title = "Choose a writable session data directory" };
            if (dialog.ShowDialog(this) != true) return; dataDirectory = dialog.FolderName;
        }
        try
        {
            SetBusy(true);
            var repository = new SessionRepository(dataDirectory);
            var snapshot = new SessionDocument { Id = session.Id, Name = session.Name, CreatedUtc = session.CreatedUtc, SelectedInterfaceId = session.SelectedInterfaceId, Origin = session.Origin, SchemaVersion = session.SchemaVersion, Events = [.. session.Events] };
            await Task.Run(() => repository.SaveAsync(snapshot));
            dirty = false; UpdateTitle(); Model.Status = "Session saved • no measurement data acquired";
            Model.RecordDiagnostic("Session save completed.");
            if (Model.PageTitle == "Sessions") ShowPage("Sessions");
        }
        catch { Model.RecordDiagnostic("Session save failed."); MessageBox.Show(this, "The session could not be saved. Check the session name, directory permissions, available space, and whether the file is in use. No save was confirmed.", "Save failed", MessageBoxButton.OK, MessageBoxImage.Error); }
        finally { SetBusy(false); }
    }
    private async void Open_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard()) return;
        var dialog = new OpenFileDialog { Title = "Open an Analyzer session", Filter = "Analyzer session (*.plasession)|*.plasession|All files (*.*)|*.*", CheckFileExists = true };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            SetBusy(true);
            var repository = new SessionRepository(Path.GetDirectoryName(dialog.FileName)!);
            var opened = await Task.Run(() => repository.OpenFileAsync(dialog.FileName));
            session = opened; dataDirectory = Path.GetDirectoryName(dialog.FileName); dirty = false;
            Model.IsDemo = false; RefreshInterfaces(); UpdateTitle(); Model.Status = "Session opened • no acquisition resumed"; ShowPage(Model.PageTitle);
            Model.RecordDiagnostic("Session open completed.");
        }
        catch { Model.RecordDiagnostic("Session open failed."); MessageBox.Show(this, "The file could not be opened as a supported Analyzer session. It may be incomplete, invalid, newer than this application, locked, or inaccessible. The current session was retained.", "Open failed", MessageBoxButton.OK, MessageBoxImage.Error); }
        finally { SetBusy(false); }
    }
    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        var dialog = new SaveFileDialog { Title = "Export session metadata (no measurements)", Filter = "JSON metadata (*.json)|*.json", FileName = "session-metadata.json", OverwritePrompt = true };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            var text = System.Text.Json.JsonSerializer.Serialize(new { Product = "Protocol Latency Analyzer", Attribution = "By Limited Underground", Kind = "Foundation session metadata; no measurements", session.Id, session.Name, session.CreatedUtc, session.SelectedInterfaceId, session.Events }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dialog.FileName, text); Model.Status = "Session metadata exported • no synthetic samples included"; Model.RecordDiagnostic("Session metadata export completed.");
        }
        catch { Model.RecordDiagnostic("Session metadata export failed."); MessageBox.Show(this, "Export failed. Check destination permissions and available space.", "Export failed", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
    private void Window_Closing(object? sender, CancelEventArgs e) => e.Cancel = !ConfirmDiscard();
    private async void SaveConfiguration_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        var picker = new SaveFileDialog { Filter = "Analyzer configuration (*.json)|*.json", FileName = "analyzer-config.json", OverwritePrompt = true };
        if (picker.ShowDialog(this) != true) return;
        try { SetBusy(true); await new ConfigurationStore(picker.FileName).SaveAsync(new AnalyzerConfiguration { SelectedInterfaceId = session.SelectedInterfaceId, MaxVisibleEvents = Model.MaxVisibleEvents }); Model.Status = "Configuration saved to the selected file"; Model.RecordDiagnostic("Configuration save completed."); }
        catch { Model.RecordDiagnostic("Configuration save failed."); MessageBox.Show(this, "Configuration could not be saved. Check the destination permissions and space.", "Configuration failed"); }
        finally { SetBusy(false); }
    }
    private async void LoadConfiguration_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        var picker = new OpenFileDialog { Filter = "Analyzer configuration (*.json)|*.json", CheckFileExists = true };
        if (picker.ShowDialog(this) != true) return;
        try { SetBusy(true); var config = await new ConfigurationStore(picker.FileName).LoadAsync(); Model.MaxVisibleEvents = config.MaxVisibleEvents; session.SelectedInterfaceId = config.SelectedInterfaceId; MarkDirty(); RefreshInterfaces(); Model.Status = "Configuration loaded • no acquisition started"; Model.RecordDiagnostic("Configuration load completed."); ShowPage("Settings"); }
        catch { Model.RecordDiagnostic("Configuration load failed."); MessageBox.Show(this, "Configuration could not be loaded. Check file access and format version.", "Configuration failed"); }
        finally { SetBusy(false); }
    }
    private void SetBusy(bool value) { busy = value; ((UIElement)Content).IsEnabled = !value; CommandManager.InvalidateRequerySuggested(); }
}
