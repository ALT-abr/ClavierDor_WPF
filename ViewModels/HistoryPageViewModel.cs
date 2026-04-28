using System.Collections.ObjectModel;
using clavierdor.Services;

namespace clavierdor.ViewModels;

public class HistoryPageViewModel : ViewModelBase
{
    private readonly GameDataService _gameDataService;

    // Constructeur par defaut qui charge directement l'historique.
    public HistoryPageViewModel()
        : this(new GameDataService(), true)
    {
    }

    // Constructeur charge automatiquement l'historique.
    public HistoryPageViewModel(GameDataService gameDataService)
        : this(gameDataService, true)
    {
    }

    // Constructeur complet pour choisir si le chargement est automatique
    public HistoryPageViewModel(GameDataService gameDataService, bool autoLoad)
    {
        _gameDataService = gameDataService;

        if (autoLoad)
        {
            Load();
        }
    }

    public ObservableCollection<HistoryItemViewModel> Histories { get; } = new();

    // Recharge les parties depuis la base et les transforme pour l'affichage
    public void Load()
    {
        Histories.Clear();

        foreach (var history in _gameDataService.GetHistories())
        {
            Histories.Add(new HistoryItemViewModel(history));
        }
    }
}
