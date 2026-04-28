namespace clavierdor.Services;

// Contient les informations deja preparees pour le rapport
public class ExportReportData
{
    public string PlayerName { get; set; } = string.Empty;

    public string Pouvoir { get; set; } = string.Empty;

    public int Score { get; set; }

    public string Result { get; set; } = string.Empty;

    public string CreatedAt { get; set; } = "-";

    public string FinishedAt { get; set; } = "-";

    public string Category { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string BossesKilled { get; set; } = "Aucun boss tue";

    public string QuestionsAnswered { get; set; } = "0/100";
}
