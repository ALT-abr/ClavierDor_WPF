using System.ComponentModel.DataAnnotations;

namespace clavierdor.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string OptionA { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string OptionB { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string OptionC { get; set; } = string.Empty;

    [Required]
    [MaxLength(1)]
    public string CorrectAnswer { get; set; } = "A";

    public bool IsBoss { get; set; }

    public bool IsFinalBoss { get; set; }

    [MaxLength(120)]
    public string BossName { get; set; } = string.Empty;
}
