using clavierdor.Models;

namespace clavierdor.ViewModels;

public class HistoryItemViewModel
{
    // Transforme une entree History en textes 
    public HistoryItemViewModel(History history)
    {
        PlayerName = history.PlayerName;
        Score = history.Score;
        StatusText = history.IsFinished ? "Termine" : "Commence";
        DateText = history.PlayedAt.ToString("dd/MM/yyyy HH:mm");
    }

    public string PlayerName { get; }

    public int Score { get; }

    public string StatusText { get; }

    public string DateText { get; }
}
