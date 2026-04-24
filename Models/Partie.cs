using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace clavierdor.Models;

public class Partie
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public Player? Player { get; set; }

    [Required]
    [MaxLength(50)]
    public string Pouvoir { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    public int CurrentQuestionIndex { get; set; }

    public int Score { get; set; }

    public bool IsFinished { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    public ICollection<History> HistoryEntries { get; set; } = new List<History>();
}
