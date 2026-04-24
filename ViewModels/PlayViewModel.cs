using clavierdor.Models;
using clavierdor.Services;

namespace clavierdor.ViewModels;

public class PlayViewModel : ViewModelBase
{
    private readonly GameDataService _gameDataService;
    private string _playerName = string.Empty;
    private string _selectedPouvoir = "Developpeur Front";

    public PlayViewModel()
        : this(new GameDataService())
    {
    }

    public PlayViewModel(GameDataService gameDataService)
    {
        _gameDataService = gameDataService;
    }

    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public string SelectedPouvoir
    {
        get => _selectedPouvoir;
        set => SetProperty(ref _selectedPouvoir, value);
    }

    public Partie? CreatedPartie { get; private set; }

    public bool TryCreatePartie(out string message)
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            message = "Veuillez entrer le nom du joueur.";
            return false;
        }

        CreatedPartie = _gameDataService.CreatePartie(PlayerName, SelectedPouvoir);
        message = $"La partie #{CreatedPartie.Id} a ete creee pour {PlayerName}.";
        return true;
    }
}
