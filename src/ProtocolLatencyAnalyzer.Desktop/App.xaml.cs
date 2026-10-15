using System.Windows;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace ProtocolLatencyAnalyzer.Desktop;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if (e.Args.Length > 0)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            try
            {
                if (e.Args.Length == 2 && e.Args[0] == "--verify-storage" && Path.IsPathFullyQualified(e.Args[1]))
                {
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
                    Shutdown(0); return;
                }
                if (e.Args.Length == 1 && e.Args[0] == "--smoke-check")
                {
                    static void Require(bool condition) { if (!condition) throw new InvalidOperationException("Offline smoke assertion failed."); }
                    var model = new DashboardViewModel();
                    Require(!model.IsDemo && model.Events.Count == 0 && model.Interfaces.Count == 0 && model.SelectedInterface is null);
                    Require(model.ProtocolRows.Count == 4 && model.ProtocolRows.All(x => x.State == "Not implemented" && x.Timing == "Unavailable"));
                    model.IsDemo = true;
                    Require(model.Events.Count == 1 && model.Events[0].Context == "DEMONSTRATION" && model.ProtocolRows.All(x => x.State == "Synthetic preview"));
                    model.IsDemo = false;
                    Require(model.Events.Count == 0 && model.ProtocolRows.All(x => x.Timing == "Unavailable"));
                    // Fixed synthetic messages only: these do not contain paths, secrets or interface values.
                    var configuration = new ProtocolLatencyAnalyzer.Core.AnalyzerConfiguration { MaxVisibleEvents = 10 };
                    configuration.Validate(); model.MaxVisibleEvents = configuration.MaxVisibleEvents;
                    for (var i = 0; i < 10; i++) model.RecordDiagnostic("Offline smoke operation completed.");
                    model.RecordDiagnostic("Offline smoke newest operation completed.");
                    Require(model.Events.Count == 10 && model.DroppedDiagnosticCount == 1 && model.Events.Last().Detail == "Offline smoke newest operation completed.");
                    Require(model.Status.Contains("1 older diagnostic entries discarded", StringComparison.Ordinal));
                    model.IsDemo = true;
                    Require(model.Events.Count == 10 && model.Events[0].Context == "DEMONSTRATION" && model.Events.Last().Detail == "Offline smoke newest operation completed.");
                    model.IsDemo = false;
                    Require(model.Events.Count == 10);
                    foreach (var size in new[] { new Size(1280,800), new Size(1536,1024) })
                    {
                        var check = new MainWindow(true) { Width = size.Width, Height = size.Height };
                        Require(check.Model.Interfaces.Count == 1 && check.Model.Interfaces[0].Id == "synthetic");
                        check.Model.IsDemo = false;
                        var root = (FrameworkElement)check.Content;
                        root.Measure(size); root.Arrange(new Rect(new Point(),size)); root.UpdateLayout();
                        Require(root.IsMeasureValid && root.IsArrangeValid && root.ActualWidth == size.Width && root.ActualHeight == size.Height);
                        Require(double.IsFinite(root.DesiredSize.Width) && double.IsFinite(root.DesiredSize.Height));
                        check.Model.IsDemo = true; root.UpdateLayout();
                        Require(check.Model.ProtocolRows.All(x => x.State == "Synthetic preview"));
                    }
                    Shutdown(0); return;
                }
                if (e.Args.Length == 2 && e.Args[0] == "--render-preview" && Path.IsPathFullyQualified(e.Args[1]))
                {
                    var output = Path.GetFullPath(e.Args[1]);
                    if (!output.Contains($"{Path.DirectorySeparatorChar}.artifacts{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)) throw new ArgumentException();
                    var window = new MainWindow(true) { Width = 1536, Height = 1024 };
                    var visual = (FrameworkElement)window.Content;
                    visual.Measure(new Size(1536,1024)); visual.Arrange(new Rect(0,0,1536,1024)); visual.UpdateLayout();
                    var bitmap = new RenderTargetBitmap(1536,1024,96,96,PixelFormats.Pbgra32); bitmap.Render(visual);
                    var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    Directory.CreateDirectory(Path.GetDirectoryName(output)!); using (var stream = File.Create(output)) encoder.Save(stream);
                    Shutdown(0); return;
                }
            }
            catch { Shutdown(1); return; }
            Shutdown(2); return;
        }
        new MainWindow().Show();
    }
}
