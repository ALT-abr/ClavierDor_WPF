using System.Windows;
using System.Windows.Controls;
using clavierdor.Services;
using clavierdor.ViewModels;

namespace clavierdor.Views.Pages;

public partial class History : Page
{
    public HistoryPageViewModel ViewModel { get; }

    public History()
    {
        InitializeComponent();
        try
        {
            ViewModel = new HistoryPageViewModel();
        }
        catch (Exception ex)
        {
            ViewModel = new HistoryPageViewModel(new GameDataService(), false);
            MessageBox.Show(
                "Impossible de charger l'historique.\nVerifie que la base a bien ete recreee avec les migrations.\n\n" + ex.Message,
                "Clavier D'Or");
        }

        DataContext = ViewModel;
    }

    private void BackHome_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new Home());
    }

    private void PreviousPage_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("La pagination n'est pas encore branchee.", "Clavier D'Or");
    }

    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("La pagination n'est pas encore branchee.", "Clavier D'Or");
    }
}
