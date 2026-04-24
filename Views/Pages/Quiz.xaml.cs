using System.Windows;
using System.Windows.Controls;
using clavierdor.ViewModels;

namespace clavierdor.Views.Pages;

public partial class Quiz : Page
{
    public QuizPageViewModel ViewModel { get; }

    public Quiz(int partieId)
    {
        InitializeComponent();
        ViewModel = new QuizPageViewModel(partieId);
        DataContext = ViewModel;
        UpdateVisualState();
    }

    private void OptionAButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitAnswer("A");
    }

    private void OptionBButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitAnswer("B");
    }

    private void OptionCButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitAnswer("C");
    }

    private void BackHomeButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new Home());
    }

    private void UsePouvoirButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.UsePouvoir(out var message))
        {
            MessageBox.Show(message, "Clavier D'Or");
            UpdateVisualState();
            return;
        }

        MessageBox.Show(message, "Clavier D'Or");
    }

    private void SubmitAnswer(string answer)
    {
        ViewModel.SubmitAnswer(answer, out var message);
        MessageBox.Show(message, "Clavier D'Or");
        UpdateVisualState();

        if (ViewModel.IsCompleted)
        {
            MessageBox.Show(ViewModel.CompleteMessage, "Clavier D'Or");
        }
    }

    private void UpdateVisualState()
    {
        var showQuestion = !ViewModel.IsCompleted && ViewModel.CurrentQuestion is not null;
        OptionAButton.Visibility = showQuestion ? Visibility.Visible : Visibility.Collapsed;
        OptionBButton.Visibility = showQuestion ? Visibility.Visible : Visibility.Collapsed;
        OptionCButton.Visibility = showQuestion ? Visibility.Visible : Visibility.Collapsed;
        BossTitleTextBlock.Visibility = ViewModel.IsBossQuestion ? Visibility.Visible : Visibility.Collapsed;
        CompletedTextBlock.Visibility = ViewModel.IsCompleted ? Visibility.Visible : Visibility.Collapsed;
    }
}
