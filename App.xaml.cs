using System.Windows;
using clavierdor.Data;

namespace clavierdor;

// Point d'entree WPF de l'application.
public partial class App : Application
{
    // Initialise la base avant d'afficher l'interface.
    protected override void OnStartup(StartupEventArgs e)
    {
        DatabaseInitializer.Initialize();
        base.OnStartup(e);
    }
}
