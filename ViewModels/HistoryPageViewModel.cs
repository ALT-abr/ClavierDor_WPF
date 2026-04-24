using System.Collections.ObjectModel;
using clavierdor.Services;

namespace clavierdor.ViewModels;

public class HistoryPageViewModel : ViewModelBase
{
    private readonly GameDataService _gameDataService;

    public HistoryPageViewModel()
        : this(new GameDataService(), true)
    {
    }

    public HistoryPageViewModel(GameDataService gameDataService)
        : this(gameDataService, true)
    {
    }

    public HistoryPageViewModel(GameDataService gameDataService, bool autoLoad)
    {
        _gameDataService = gameDataService;

        if (autoLoad)
        {
            Load();
        }
    }

    public ObservableCollection<HistoryItemViewModel> Histories { get; } = new();

    public void Load()
    {
        Histories.Clear();

        foreach (var history in _gameDataService.GetHistories())
        {
            Histories.Add(new HistoryItemViewModel(history));
        }
    }
}
