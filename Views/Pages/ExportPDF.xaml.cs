using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using clavierdor.Services;
using clavierdor.ViewModels;

namespace clavierdor.Views.Pages;
public partial class ExportPDF : Page
{
    public ExportPdfViewModel ViewModel { get; }
    private readonly GameDataService _gameDataService;
    private readonly PdfExportService _pdfExportService;

    // Initialise les services et charge les joueurs disponibles
    public ExportPDF()
    {
        _gameDataService = new GameDataService();
        _pdfExportService = new PdfExportService();
        ViewModel = new ExportPdfViewModel(_gameDataService, false);

        try
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.LoadPlayers();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Impossible de charger la page Export PDF.\n\n" + ex.Message,
                "Clavier D'Or");
        }
    }

    // Retourne a la page d'accueil
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new Home());
    }

    // Genere le PDF
    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.SelectedPlayerName))
        {
            MessageBox.Show("Aucun joueur n'est disponible pour l'export.", "Clavier D'Or");
            return;
        }

        var report = _gameDataService.GetExportReportData(ViewModel.SelectedPlayerName);

        if (report is null)
        {
            MessageBox.Show("Aucune donnee d'historique n'est disponible pour ce joueur.", "Clavier D'Or");
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = "PDF (*.pdf)|*.pdf",
            DefaultExt = ".pdf",
            FileName = $"rapport-{report.PlayerName.Replace(' ', '-').ToLowerInvariant()}.pdf"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            _pdfExportService.ExportPlayerReport(dialog.FileName, report);
            MessageBox.Show("Le PDF a ete genere avec succes.", "Clavier D'Or");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Impossible de generer le PDF.\n\n" + ex.Message, "Clavier D'Or");
        }
    }
}
