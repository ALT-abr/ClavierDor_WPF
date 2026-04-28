using System.Windows;
using System.Windows.Controls;
using clavierdor.Views.Dialogs;

namespace clavierdor.Views.Pages;

public partial class Home : Page
{
    // Initialise l'interface de la page d'accueil
    public Home()
    {
        InitializeComponent();
    }

    // ouvrir la carte de jeu
    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Play
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && dialog.ViewModel.CreatedPartie is not null)
        {
            NavigationService?.Navigate(new Quiz(dialog.ViewModel.CreatedPartie.Id));
        }
    }

    // Ouvre la cart de reprise
    private void ResumeButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Resume
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() == true && dialog.ViewModel.FoundPartie is not null)
        {
            NavigationService?.Navigate(new Quiz(dialog.ViewModel.FoundPartie.Id));
        }
    }

    // Navigue vers la page historique
    private void HistoryButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new History());
    }

    // Navigue vers la page d'export PDF
    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new ExportPDF());
    }
}
