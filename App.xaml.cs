using System.Windows;
using clavierdor.Data;

namespace clavierdor;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DatabaseInitializer.Initialize();
        base.OnStartup(e);
    }
}
