using clavierdor.Models;
using clavierdor.Services;

namespace clavierdor.ViewModels;

public class ResumeViewModel : ViewModelBase
{
    private readonly GameDataService _gameDataService;
    private string _playerName = string.Empty;

    public ResumeViewModel()
        : this(new GameDataService())
    {
    }

    public ResumeViewModel(GameDataService gameDataService)
    {
        _gameDataService = gameDataService;
    }

    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public Partie? FoundPartie { get; private set; }

    public bool TryFindPartie(out string message)
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            message = "Veuillez entrer le nom du joueur.";
            return false;
        }

        FoundPartie = _gameDataService.FindOpenPartie(PlayerName);

        if (FoundPartie is null)
        {
            message = "Aucune partie en cours n'a ete trouvee pour ce joueur.";
            return false;
        }

        message = $"Partie #{FoundPartie.Id} retrouvee. Score actuel : {FoundPartie.Score}.";
        return true;
    }
}
