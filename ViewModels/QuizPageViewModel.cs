using System;
using System.Collections.Generic;
using System.Linq;
using clavierdor.Models;
using clavierdor.Services;

namespace clavierdor.ViewModels;

public class QuizPageViewModel : ViewModelBase
{
    private static readonly string[] CategoryOrder =
    {
        "anglais",
        "culture_generale",
        "metiers_informatique",
        "logique",
        "algorithmes"
    };

    private readonly GameDataService _gameDataService;
    private readonly int _partieId;
    private List<Question> _questions = new();
    private Partie? _partie;
    private Question? _currentQuestion;
    private string _progressText = string.Empty;
    private string _categoryText = string.Empty;
    private string _playerText = string.Empty;
    private string _scoreText = "0";
    private string _hintText = string.Empty;
    private string _feedbackText = string.Empty;
    private bool _isCompleted;
    private bool _pouvoirUsed;
    private bool _backPowerActiveForCurrentQuestion;

    public QuizPageViewModel(int partieId)
        : this(new GameDataService(), partieId)
    {
    }

    public QuizPageViewModel(GameDataService gameDataService, int partieId)
    {
        _gameDataService = gameDataService;
        _partieId = partieId;
        Load();
    }

    public Question? CurrentQuestion
    {
        get => _currentQuestion;
        private set
        {
            if (SetProperty(ref _currentQuestion, value))
            {
                RaisePropertyChanged(nameof(QuestionText));
                RaisePropertyChanged(nameof(OptionAText));
                RaisePropertyChanged(nameof(OptionBText));
                RaisePropertyChanged(nameof(OptionCText));
                RaisePropertyChanged(nameof(IsBossQuestion));
                RaisePropertyChanged(nameof(IsFinalBossQuestion));
                RaisePropertyChanged(nameof(BossTitle));
                RaisePropertyChanged(nameof(CanUsePouvoir));
                RaisePropertyChanged(nameof(PouvoirButtonText));
            }
        }
    }

    public string QuestionText => CurrentQuestion?.Text ?? "Aucune question disponible.";

    public string OptionAText => CurrentQuestion?.OptionA ?? string.Empty;

    public string OptionBText => CurrentQuestion?.OptionB ?? string.Empty;

    public string OptionCText => CurrentQuestion?.OptionC ?? string.Empty;

    public bool IsBossQuestion => CurrentQuestion?.IsBoss == true || CurrentQuestion?.IsFinalBoss == true;

    public bool IsFinalBossQuestion => CurrentQuestion?.IsFinalBoss == true;

    public string BossTitle
    {
        get
        {
            if (CurrentQuestion is null || !IsBossQuestion)
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(CurrentQuestion.BossName)
                ? "Boss"
                : CurrentQuestion.BossName;
        }
    }

    public string ProgressText
    {
        get => _progressText;
        private set => SetProperty(ref _progressText, value);
    }

    public string CategoryText
    {
        get => _categoryText;
        private set => SetProperty(ref _categoryText, value);
    }

    public string PlayerText
    {
        get => _playerText;
        private set => SetProperty(ref _playerText, value);
    }

    public string ScoreText
    {
        get => _scoreText;
        private set => SetProperty(ref _scoreText, value);
    }

    public string HintText
    {
        get => _hintText;
        private set => SetProperty(ref _hintText, value);
    }

    public string FeedbackText
    {
        get => _feedbackText;
        private set => SetProperty(ref _feedbackText, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        private set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                RaisePropertyChanged(nameof(CanUsePouvoir));
                RaisePropertyChanged(nameof(PouvoirButtonText));
            }
        }
    }

    public string CompleteMessage =>
        _partie is null
            ? "La partie est terminee."
            : $"Partie terminee. Score final : {_partie.Score} / 500";

    public bool CanUsePouvoir => !_pouvoirUsed && !IsCompleted && CurrentQuestion is not null;

    public string PouvoirButtonText
    {
        get
        {
            if (_partie is null)
            {
                return "Pouvoir";
            }

            return _pouvoirUsed
                ? "Pouvoir utilise"
                : $"Utiliser : {_partie.Pouvoir}";
        }
    }

    public bool UsePouvoir(out string message)
    {
        if (_partie is null || CurrentQuestion is null)
        {
            message = "Aucune question active.";
            FeedbackText = message;
            return false;
        }

        if (_pouvoirUsed)
        {
            message = "Vous avez deja utilise votre pouvoir.";
            FeedbackText = message;
            return false;
        }

        var success = true;

        switch (_partie.Pouvoir)
        {
            case "Developpeur Front":
                success = TryUseFrontPower(out message);
                break;
            case "Developpeur Back":
                _backPowerActiveForCurrentQuestion = true;
                message = "Pouvoir Back active : votre prochaine erreur sur cette question sera corrigee.";
                break;
            case "Developpeur Mobile":
                HintText = BuildMobileHint();
                message = "Pouvoir Mobile utilise : un indice a ete revele.";
                break;
            default:
                message = "Pouvoir inconnu.";
                success = false;
                break;
        }

        if (success)
        {
            FeedbackText = message;
            _pouvoirUsed = true;
            RaisePropertyChanged(nameof(CanUsePouvoir));
            RaisePropertyChanged(nameof(PouvoirButtonText));
        }
        else
        {
            FeedbackText = message;
        }

        return success;
    }

    public bool SubmitAnswer(string selectedAnswer, out string resultMessage)
    {
        if (_partie is null || CurrentQuestion is null)
        {
            resultMessage = "La partie n'a pas pu etre chargee.";
            FeedbackText = resultMessage;
            return false;
        }

        var isCorrect = string.Equals(
            selectedAnswer.Trim(),
            CurrentQuestion.CorrectAnswer.Trim(),
            StringComparison.OrdinalIgnoreCase);

        var correctionMessage = string.Empty;

        if (!isCorrect && _backPowerActiveForCurrentQuestion)
        {
            isCorrect = true;
            correctionMessage = "Rattrapage automatique active.";
        }

        var delta = 0;
        string? defeatedBossName = null;

        if (CurrentQuestion.IsBoss || CurrentQuestion.IsFinalBoss)
        {
            if (CurrentQuestion.IsFinalBoss)
            {
                delta = isCorrect ? 50 : -20;
                if (isCorrect)
                {
                    defeatedBossName = string.IsNullOrWhiteSpace(BossTitle) ? "Boss final" : BossTitle;
                }
                resultMessage = isCorrect
                    ? $"{BossTitle} vaincu."
                    : $"{BossTitle} vous a battu.";
            }
            else
            {
                delta = isCorrect ? 30 : -10;
                if (isCorrect)
                {
                    defeatedBossName = string.IsNullOrWhiteSpace(BossTitle) ? $"Boss {CurrentQuestion.Category}" : BossTitle;
                }
                resultMessage = isCorrect
                    ? $"{BossTitle} vaincu."
                    : $"{BossTitle} vous a battu.";
            }
        }
        else
        {
            delta = isCorrect ? 3 : 0;
            resultMessage = isCorrect
                ? "Bonne reponse."
                : "Mauvaise reponse.";
        }

        if (!string.IsNullOrWhiteSpace(correctionMessage))
        {
            resultMessage = $"{correctionMessage} {resultMessage}";
        }

        _partie.Score += delta;
        _partie.CurrentQuestionIndex += 1;
        _backPowerActiveForCurrentQuestion = false;
        HintText = string.Empty;

        if (_partie.CurrentQuestionIndex >= _questions.Count)
        {
            _partie.IsFinished = true;
            _partie.FinishedAt = DateTime.Now;
            IsCompleted = true;
        }

        _partie.Category = CurrentQuestion.Category;
        _gameDataService.SavePartie(_partie, defeatedBossName);
        UpdateState();
        FeedbackText = resultMessage;

        return isCorrect;
    }

    public void Load()
    {
        _partie = _gameDataService.GetPartieById(_partieId);
        _questions = _gameDataService.GetOrderedQuizQuestions(CategoryOrder).ToList();

        if (_partie is null || _questions.Count == 0)
        {
            CurrentQuestion = null;
            ProgressText = "0 / 0";
            CategoryText = "-";
            PlayerText = "Joueur introuvable";
            ScoreText = "0";
            HintText = string.Empty;
            FeedbackText = string.Empty;
            IsCompleted = false;
            return;
        }

        if (_partie.CurrentQuestionIndex >= _questions.Count)
        {
            _partie.CurrentQuestionIndex = _questions.Count;
            _partie.IsFinished = true;
            IsCompleted = true;
        }

        UpdateState();
    }

    private void UpdateState()
    {
        if (_partie is null)
        {
            return;
        }

        PlayerText = _partie.Player?.Name ?? "Joueur";
        ScoreText = _partie.Score.ToString();

        if (_partie.IsFinished || _partie.CurrentQuestionIndex >= _questions.Count)
        {
            CurrentQuestion = null;
            ProgressText = $"{_questions.Count} / {_questions.Count}";
            CategoryText = "Final";
            IsCompleted = true;
            HintText = string.Empty;
            RaisePropertyChanged(nameof(CompleteMessage));
            return;
        }

        CurrentQuestion = _questions[_partie.CurrentQuestionIndex];
        ProgressText = $"{_partie.CurrentQuestionIndex + 1} / {_questions.Count}";
        CategoryText = CurrentQuestion.Category;
        IsCompleted = false;
    }

    private bool TryUseFrontPower(out string message)
    {
        if (CurrentQuestion is null)
        {
            message = "Impossible d'utiliser ce pouvoir maintenant.";
            return false;
        }

        if (CurrentQuestion.IsBoss || CurrentQuestion.IsFinalBoss)
        {
            message = "Impossible de changer une question boss.";
            return false;
        }

        var currentIndex = _partie?.CurrentQuestionIndex ?? 0;
        var alternativeIndex = -1;

        for (var i = currentIndex + 1; i < _questions.Count; i++)
        {
            if (_questions[i].Category.Equals(CurrentQuestion.Category, StringComparison.OrdinalIgnoreCase) &&
                !_questions[i].IsBoss &&
                !_questions[i].IsFinalBoss)
            {
                alternativeIndex = i;
                break;
            }
        }

        if (alternativeIndex < 0)
        {
            message = "Impossible de changer cette question : aucune autre question dans cette categorie.";
            return false;
        }

        (_questions[currentIndex], _questions[alternativeIndex]) = (_questions[alternativeIndex], _questions[currentIndex]);
        CurrentQuestion = _questions[currentIndex];
        CategoryText = CurrentQuestion.Category;
        HintText = string.Empty;
        message = "Pouvoir Front utilise : question changee.";
        return true;
    }

    private string BuildMobileHint()
    {
        if (CurrentQuestion is null)
        {
            return string.Empty;
        }

        return CurrentQuestion.CorrectAnswer.ToUpperInvariant() switch
        {
            "A" => "Indice : l'option C est fausse.",
            "B" => "Indice : l'option A est fausse.",
            "C" => "Indice : l'option B est fausse.",
            _ => "Indice indisponible."
        };
    }
}
