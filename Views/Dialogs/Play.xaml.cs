using System.Windows;
using clavierdor.ViewModels;

namespace clavierdor.Views.Dialogs;

public partial class Play : Window
{
    private const string PlaceholderText = "Entrez votre nom";

    public PlayViewModel ViewModel { get; }

    // Prepare la fenetre 
    public Play()
    {
        InitializeComponent();
        ViewModel = new PlayViewModel();
        DataContext = ViewModel;
        FrontRoleRadioButton.IsChecked = true;
        PlayerNameTextBox.Focus();
        PlayerNameTextBox.SelectAll();
    }

    // Ferme la fenetre
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // Annule la creation
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // Valide le formulaire et cree la partie
    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.PlayerName = PlayerNameTextBox.Text == PlaceholderText
            ? string.Empty
            : PlayerNameTextBox.Text.Trim();

        ViewModel.SelectedPouvoir = FrontRoleRadioButton.IsChecked == true
            ? "Developpeur Front"
            : BackRoleRadioButton.IsChecked == true
                ? "Developpeur Back"
                : "Developpeur Mobile";

        try
        {
            if (!ViewModel.TryCreatePartie(out var message))
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
                "Impossible d'enregistrer la partie.\nVerifie que la base a bien ete recreee avec les migrations.\n\n" + ex.Message,
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

    // Remet le texte indicatif si il y a aucun nom
    private void PlayerNameTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PlayerNameTextBox.Text))
        {
            PlayerNameTextBox.Text = PlaceholderText;
        }
    }
}
