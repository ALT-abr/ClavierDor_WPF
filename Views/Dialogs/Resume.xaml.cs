using System.Windows;
using clavierdor.ViewModels;

namespace clavierdor.Views.Dialogs;

public partial class Resume : Window
{
    private const string PlaceholderText = "Entrez le nom du joueur";

    public ResumeViewModel ViewModel { get; }

    // Prepare la fenetre 
    public Resume()
    {
        InitializeComponent();
        ViewModel = new ResumeViewModel();
        DataContext = ViewModel;
        PlayerNameTextBox.Focus();
        PlayerNameTextBox.SelectAll();
    }

    // Ferme la fenetre 
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // Annule la reprise de partie
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // Cherche une partie en cours
    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.PlayerName = PlayerNameTextBox.Text == PlaceholderText
            ? string.Empty
            : PlayerNameTextBox.Text.Trim();

        try
        {
            if (!ViewModel.TryFindPartie(out var message))
            {
                MessageBox.Show(message, "Clavier D'Or");
                PlayerNameTextBox.Focus();
                return;
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Impossible de retrouver la partie.\nVerifie que la base a bien ete recreee avec les migrations.\n\n" + ex.Message,
                "Clavier D'Or");
        }
    }

    // Efface le texte indicatif quand le champ recoit le focus
    private void PlayerNameTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (PlayerNameTextBox.Text == PlaceholderText)
        {
            PlayerNameTextBox.Text = string.Empty;
        }
    }

    // Remet le texte indicatif si aucun nom n'est saisi
    private void PlayerNameTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PlayerNameTextBox.Text))
        {
            PlayerNameTextBox.Text = PlaceholderText;
        }
    }
}
