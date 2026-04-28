using System.Windows;
using System.Windows.Controls;
using clavierdor.ViewModels;

namespace clavierdor.Views.Pages;

public partial class Quiz : Page
{
    public QuizPageViewModel ViewModel { get; }

    // Initialise la page pour la partie
    public Quiz(int partieId)
    {
        InitializeComponent();
        ViewModel = new QuizPageViewModel(partieId);
        DataContext = ViewModel;
        UpdateVisualState();
    }

    // Repond avec l'option A
    private void OptionAButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitAnswer("A");
    }

    // Repond avec l'option B
    private void OptionBButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitAnswer("B");
    }

    // Repond avec l'option C
    private void OptionCButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitAnswer("C");
    }

    // Retourne a la page d'acueil
    private void BackHomeButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new Home());
    }

    // Utilise le pouvoir
    private void UsePouvoirButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.UsePouvoir(out _);
        UpdateVisualState();
    }

    // Envoie la reponse choisie
    private void SubmitAnswer(string answer)
    {
        ViewModel.SubmitAnswer(answer, out _);
        UpdateVisualState();
    }

    // Affiche ou cache les elements selon l'etat du quiz.
    private void UpdateVisualState()
    {
        var showQuestion = !ViewModel.IsCompleted && ViewModel.CurrentQuestion is not null;
        OptionAButton.Visibility = showQuestion ? Visibility.Visible : Visibility.Collapsed;
        OptionBButton.Visibility = showQuestion ? Visibility.Visible : Visibility.Collapsed;
        OptionCButton.Visibility = showQuestion ? Visibility.Visible : Visibility.Collapsed;
        BossTitleTextBlock.Visibility = ViewModel.IsBossQuestion ? Visibility.Visible : Visibility.Collapsed;
        FeedbackTextBlock.Visibility = string.IsNullOrWhiteSpace(ViewModel.FeedbackText) ? Visibility.Collapsed : Visibility.Visible;
        CompletedTextBlock.Visibility = ViewModel.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
    }
}
