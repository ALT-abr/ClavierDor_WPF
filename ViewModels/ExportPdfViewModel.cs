using System.Collections.ObjectModel;
using clavierdor.Services;

namespace clavierdor.ViewModels;

// Prepare les donnees affichees sur la page d'export 
public class ExportPdfViewModel : ViewModelBase
{
    private readonly GameDataService _gameDataService;
    private string? _selectedPlayerName;
    private string _previewPlayerName = "Aucun joueur";
    private string _previewPouvoir = "-";
    private string _previewScore = "0";
    private string _previewResult = "Aucune partie trouvee";
    private string _previewCreatedAt = "-";
    private string _previewFinishedAt = "-";
    private string _previewStatus = "-";
    private string _previewCategory = "-";
    private string _previewBossStatus = "-";

    // Constructeur qui charge les joueurs "créer rapidement le ViewModel avec le service par défaut"
    public ExportPdfViewModel()
        : this(new GameDataService(), true)
    {
    }

    // Constructeur charge automatiquement les joueurs "permettre d’utiliser un service déjà créé."
    public ExportPdfViewModel(GameDataService gameDataService)
        : this(gameDataService, true)
    {
    }

    // Constructeur complet pour choisir si le chargement est automatique ""
    public ExportPdfViewModel(GameDataService gameDataService, bool autoLoad)
    {
        _gameDataService = gameDataService;

        if (autoLoad)
        {
            LoadPlayers();
        }
    }

    public ObservableCollection<string> Players { get; } = new();

    public string? SelectedPlayerName
    {
        get => _selectedPlayerName;
        set
        {
            if (SetProperty(ref _selectedPlayerName, value))
            {
                LoadPreview();
            }
        }
    }

    public string PreviewPlayerName
    {
        get => _previewPlayerName;
        private set
        {
            if (SetProperty(ref _previewPlayerName, value))
            {
                RaisePropertyChanged(nameof(PreviewPlayerLine));
            }
        }
    }

    public string PreviewPouvoir
    {
        get => _previewPouvoir;
        private set
        {
            if (SetProperty(ref _previewPouvoir, value))
            {
                RaisePropertyChanged(nameof(PreviewPouvoirLine));
            }
        }
    }

    public string PreviewScore
    {
        get => _previewScore;
        private set => SetProperty(ref _previewScore, value);
    }

    public string PreviewResult
    {
        get => _previewResult;
        private set => SetProperty(ref _previewResult, value);
    }

    public string PreviewCreatedAt
    {
        get => _previewCreatedAt;
        private set => SetProperty(ref _previewCreatedAt, value);
    }

    public string PreviewFinishedAt
    {
        get => _previewFinishedAt;
        private set => SetProperty(ref _previewFinishedAt, value);
    }

    public string PreviewStatus
    {
        get => _previewStatus;
        private set => SetProperty(ref _previewStatus, value);
    }

    public string PreviewCategory
    {
        get => _previewCategory;
        private set => SetProperty(ref _previewCategory, value);
    }

    public string PreviewBossStatus
    {
        get => _previewBossStatus;
        private set => SetProperty(ref _previewBossStatus, value);
    }

    public string PreviewPlayerLine => $"Joueur : {PreviewPlayerName}";

    public string PreviewPouvoirLine => $"Pouvoir choisi : {PreviewPouvoir}";

    public string PreviewScoreLine => $"{PreviewScore} / 500";

    // Charge les joueurs disponibles dans la liste deroulante
    public void LoadPlayers()
    {
        Players.Clear();

        foreach (var player in _gameDataService.GetPlayerNames())
        {
            Players.Add(player);
        }

        SelectedPlayerName = Players.Count > 0 ? Players[0] : null;
    }

    // Charge l'apercu du rapport pour le joueur selectionne
    private void LoadPreview()
    {
        if (string.IsNullOrWhiteSpace(SelectedPlayerName))
        {
            ResetPreview();
            return;
        }

        var history = _gameDataService.GetLatestHistoryForPlayer(SelectedPlayerName);

        if (history is null)
        {
            ResetPreview();
            PreviewPlayerName = SelectedPlayerName;
            return;
        }

        PreviewPlayerName = history.PlayerName;
        PreviewPouvoir = history.Pouvoir;
        PreviewScore = history.Score.ToString();
        PreviewResult = history.IsFinished ? "Partie terminee" : "Partie en cours";
        PreviewCreatedAt = history.Partie?.CreatedAt.ToString("dd/MM/yyyy HH:mm") ?? history.PlayedAt.ToString("dd/MM/yyyy HH:mm");
        PreviewFinishedAt = history.Partie?.FinishedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        PreviewStatus = history.IsFinished ? "Terminee" : "En cours";
        PreviewCategory = string.IsNullOrWhiteSpace(history.Category) ? "-" : history.Category;
        PreviewBossStatus = history.WonBoss ? "Boss vaincu" : "Boss non vaincu";
    }

    // Remet l'apercu a son etat vide.
    private void ResetPreview()
    {
        PreviewPlayerName = "Aucun joueur";
        PreviewPouvoir = "-";
        PreviewScore = "0";
        PreviewResult = "Aucune partie trouvee";
        PreviewCreatedAt = "-";
        PreviewFinishedAt = "-";
        PreviewStatus = "-";
        PreviewCategory = "-";
        PreviewBossStatus = "-";
    }
}
