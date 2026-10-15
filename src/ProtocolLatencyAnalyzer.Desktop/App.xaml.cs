using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace ProtocolLatencyAnalyzer.Desktop;
public partial class App : Application
{
    public App()
    {
        var firstArgument = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
        if (Environment.GetEnvironmentVariable("PLA_TRACE_STARTUP") == "1" || firstArgument is "--smoke-check" or "--verify-storage" or "--render-preview" or "--verify-startup")
            Console.Error.WriteLine("validation: app-constructor");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var isValidation = e.Args.Length > 0 && e.Args[0] is "--smoke-check" or "--verify-storage" or "--render-preview" or "--verify-startup";
        void TraceValidation(string stage) { if (isValidation) Console.Error.WriteLine($"validation: {stage}"); }
        TraceValidation("entry");
        base.OnStartup(e);
        if (e.Args.Length > 0)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            try
            {
                if (e.Args.Length == 1 && e.Args[0] == "--verify-startup")
                {
                    TraceValidation("normal-window-constructor-begin");
                    var startupWindow = new MainWindow() { ShowActivated = false };
                    TraceValidation("normal-window-constructor-end");
                    var rendered = false;
                    startupWindow.ContentRendered += (_, _) =>
                    {
                        if (rendered) return;
                        rendered = true;
                        TraceValidation("normal-window-content-rendered");
                        _ = Dispatcher.InvokeAsync(() =>
                        {
                            try
                            {
                                TraceValidation("normal-window-application-idle");
                                if (!startupWindow.IsVisible || PresentationSource.FromVisual(startupWindow) is null || new System.Windows.Interop.WindowInteropHelper(startupWindow).Handle == IntPtr.Zero)
                                    throw new InvalidOperationException("Startup window assertion failed.");
                                TraceValidation("normal-window-close-begin");
                                startupWindow.Close();
                                if (startupWindow.IsVisible) throw new InvalidOperationException("Startup window close assertion failed.");
                                TraceValidation("normal-window-close-end");
                                TraceValidation("shutdown-requested");
                                Shutdown(0);
                            }
                            catch { TraceValidation("validation-failed"); TraceValidation("shutdown-requested"); Shutdown(1); }
                        }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                    };
                    TraceValidation("normal-window-show-begin");
                    startupWindow.Show();
                    TraceValidation("normal-window-show-end");
                    return;
                }
                if (e.Args.Length == 2 && e.Args[0] == "--verify-storage" && Path.IsPathFullyQualified(e.Args[1]))
                {
                    TraceValidation("storage-checks-begin");
                    var root = Path.GetFullPath(e.Args[1]);
                    if (!root.Contains($"{Path.DirectorySeparatorChar}.artifacts{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException();
                    var directory = Path.Combine(root, Guid.NewGuid().ToString("N"));
                    Directory.CreateDirectory(directory);
                    Task.Run(async () =>
                    {
                        var document = new ProtocolLatencyAnalyzer.Core.SessionDocument
                        {
                            Name = "Synthetic SQLite parameterization ' ; -- check",
                            Origin = "Simulation",
                            SelectedInterfaceId = "synthetic-interface",
                            Events = [new(DateTimeOffset.UtcNow, "Offline validation", "Fixed synthetic storage event.")]
                        };
                        var repository = new ProtocolLatencyAnalyzer.Infrastructure.SessionRepository(directory);
                        await repository.SaveAsync(document);
                        var reopened = await repository.OpenAsync(document.Id);
                        if (reopened.Id != document.Id || reopened.Name != document.Name || reopened.Origin != "Simulation" || reopened.Events.Count != 1 || reopened.Events[0].Message != document.Events[0].Message || reopened.SelectedInterfaceId != document.SelectedInterfaceId)
                            throw new InvalidOperationException("Offline storage assertion failed.");
                        var configuration = new ProtocolLatencyAnalyzer.Core.AnalyzerConfiguration { SelectedInterfaceId = "synthetic-interface", MaxVisibleEvents = 10 };
                        var store = new ProtocolLatencyAnalyzer.Infrastructure.ConfigurationStore(Path.Combine(directory,"synthetic-configuration.json"));
                        await store.SaveAsync(configuration);
                        var loaded = await store.LoadAsync();
                        if (loaded.SelectedInterfaceId != configuration.SelectedInterfaceId || loaded.MaxVisibleEvents != 10) throw new InvalidOperationException("Offline configuration assertion failed.");
                    }).GetAwaiter().GetResult();
                    TraceValidation("storage-checks-end");
                    TraceValidation("shutdown-requested");
                    Shutdown(0); return;
                }
                if (e.Args.Length == 1 && e.Args[0] == "--smoke-check")
                {
                    static void Require(bool condition) { if (!condition) throw new InvalidOperationException("Offline smoke assertion failed."); }
                    TraceValidation("model-checks-begin");
                    var model = new DashboardViewModel();
                    Require(!model.IsDemo && model.Events.Count == 0 && model.Interfaces.Count == 0 && model.SelectedInterface is null);
                    Require(model.ProtocolRows.Count == 4 && model.ProtocolRows.All(x => x.State == "Not implemented" && x.Timing == "Unavailable"));
                    TraceValidation("model-demo-on-begin");
                    model.IsDemo = true;
                    TraceValidation("model-demo-on-end");
                    Require(model.Events.Count == 1 && model.Events[0].Context == "DEMONSTRATION" && model.ProtocolRows.All(x => x.State == "Synthetic preview"));
                    TraceValidation("model-demo-off-begin");
                    model.IsDemo = false;
                    TraceValidation("model-demo-off-end");
                    Require(model.Events.Count == 0 && model.ProtocolRows.All(x => x.Timing == "Unavailable"));
                    // Fixed synthetic messages only: these do not contain paths, secrets or interface values.
                    var configuration = new ProtocolLatencyAnalyzer.Core.AnalyzerConfiguration { MaxVisibleEvents = 10 };
                    configuration.Validate(); model.MaxVisibleEvents = configuration.MaxVisibleEvents;
                    for (var i = 0; i < 10; i++) model.RecordDiagnostic("Offline smoke operation completed.");
                    model.RecordDiagnostic("Offline smoke newest operation completed.");
                    Require(model.Events.Count == 10 && model.DroppedDiagnosticCount == 1 && model.Events.Last().Detail == "Offline smoke newest operation completed.");
                    Require(model.Status.Contains("1 older diagnostic entries discarded", StringComparison.Ordinal));
                    TraceValidation("model-demo-on-begin");
                    model.IsDemo = true;
                    TraceValidation("model-demo-on-end");
                    Require(model.Events.Count == 10 && model.Events[0].Context == "DEMONSTRATION" && model.Events.Last().Detail == "Offline smoke newest operation completed.");
                    TraceValidation("model-demo-off-begin");
                    model.IsDemo = false;
                    TraceValidation("model-demo-off-end");
                    Require(model.Events.Count == 10);
                    TraceValidation("model-checks-end");
                    foreach (var size in new[] { new Size(1280,800), new Size(1536,1024) })
                    {
                        TraceValidation("window-constructor-begin");
                        var check = new MainWindow(true) { Width = size.Width, Height = size.Height };
                        TraceValidation("window-constructor-end");
                        Require(check.Model.Interfaces.Count == 1 && check.Model.Interfaces[0].Id == "synthetic");
                        TraceValidation("window-demo-off-begin");
                        check.Model.IsDemo = false;
                        TraceValidation("window-demo-off-end");
                        var root = (FrameworkElement)check.Content;
                        TraceValidation("measure-begin");
                        root.Measure(size);
                        TraceValidation("measure-end");
                        TraceValidation("arrange-begin");
                        root.Arrange(new Rect(new Point(),size));
                        TraceValidation("arrange-end");
                        TraceValidation("update-layout-begin");
                        root.UpdateLayout();
                        TraceValidation("update-layout-end");
                        Require(root.IsMeasureValid && root.IsArrangeValid && root.ActualWidth == size.Width && root.ActualHeight == size.Height);
                        Require(double.IsFinite(root.DesiredSize.Width) && double.IsFinite(root.DesiredSize.Height));
                        TraceValidation("window-demo-on-begin");
                        check.Model.IsDemo = true;
                        TraceValidation("window-demo-on-end");
                        TraceValidation("update-layout-begin");
                        root.UpdateLayout();
                        TraceValidation("update-layout-end");
                        Require(check.Model.ProtocolRows.All(x => x.State == "Synthetic preview"));
                    }
                    TraceValidation("shutdown-requested");
                    Shutdown(0); return;
                }
                if (e.Args.Length == 2 && e.Args[0] == "--render-preview" && Path.IsPathFullyQualified(e.Args[1]))
                {
                    var output = Path.GetFullPath(e.Args[1]);
                    if (!output.Contains($"{Path.DirectorySeparatorChar}.artifacts{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException();
                    TraceValidation("window-constructor-begin");
                    var window = new MainWindow(true) { Width = 1536, Height = 1024 };
                    TraceValidation("window-constructor-end");
                    var visual = (FrameworkElement)window.Content;
                    TraceValidation("measure-begin");
                    visual.Measure(new Size(1536,1024));
                    TraceValidation("measure-end");
                    TraceValidation("arrange-begin");
                    visual.Arrange(new Rect(0,0,1536,1024));
                    TraceValidation("arrange-end");
                    TraceValidation("update-layout-begin");
                    visual.UpdateLayout();
                    TraceValidation("update-layout-end");
                    TraceValidation("render-begin");
                    var bitmap = new RenderTargetBitmap(1536,1024,96,96,PixelFormats.Pbgra32); bitmap.Render(visual);
                    TraceValidation("render-end");
                    var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    Directory.CreateDirectory(Path.GetDirectoryName(output)!); using (var stream = File.Create(output)) encoder.Save(stream);
                    TraceValidation("shutdown-requested");
                    Shutdown(0); return;
                }
            }
            catch { TraceValidation("validation-failed"); TraceValidation("shutdown-requested"); Shutdown(1); return; }
            TraceValidation("shutdown-requested");
            Shutdown(2); return;
        }
        var traceStartup = Environment.GetEnvironmentVariable("PLA_TRACE_STARTUP") == "1";
        if (traceStartup) Console.Error.WriteLine("validation: normal-window-constructor-begin");
        var mainWindow = new MainWindow();
        if (traceStartup) Console.Error.WriteLine("validation: normal-window-constructor-end");
        if (traceStartup) Console.Error.WriteLine("validation: normal-window-show-begin");
        mainWindow.Show();
        if (traceStartup) Console.Error.WriteLine("validation: normal-window-show-end");
    }
}
