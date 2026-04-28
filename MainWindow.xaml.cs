using System.Windows;
using System.Windows.Controls;

namespace clavierdor;

// Fenetre principale qui contient la navigation entre les pages.
public partial class MainWindow : Window
{
    // Affiche la page d'accueil au lancement.
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new Views.Pages.Home());
    }
}
