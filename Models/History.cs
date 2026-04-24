using System;
using System.ComponentModel.DataAnnotations;

namespace clavierdor.Models;

public class History
{
    public int Id { get; set; }

    public int PartieId { get; set; }

    public Partie? Partie { get; set; }

    [Required]
    [MaxLength(100)]
    public string PlayerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Pouvoir { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    public int Score { get; set; }

    public bool IsFinished { get; set; }

    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

    public bool WonBoss { get; set; }

    [MaxLength(500)]
    public string BossesKilled { get; set; } = string.Empty;
}
