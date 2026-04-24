using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace clavierdor.Services;

public class PdfExportService
{
    public void ExportPlayerReport(string filePath, ExportReportData report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(13).FontColor("#1F2340"));

                page.Content().Column(column =>
                {
                    column.Spacing(16);

                    column.Item().Border(1).BorderColor("#D9DDED").Padding(24).Column(header =>
                    {
                        header.Spacing(10);

                        header.Item().AlignCenter().Text("CLAVIER D'OR")
                            .FontSize(28)
                            .Bold()
                            .FontColor("#26235D");

                        header.Item().AlignCenter().Text("Rapport de partie")
                            .FontSize(18)
                            .SemiBold()
                            .FontColor("#4338CA");

                        header.Item().PaddingTop(6).LineHorizontal(1).LineColor("#4338CA");
                    });

                    column.Item().Border(1).BorderColor("#D9DDED").Padding(20).Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Spacing(10);
                            left.Item().Text($"Joueur : {report.PlayerName}").FontSize(15).SemiBold();
                            left.Item().Text($"Pouvoir choisi : {report.Pouvoir}");
                            left.Item().Text($"Score final : {report.Score} / 500").FontSize(18).Bold().FontColor("#4C43D0");
                            left.Item().Text($"Resultat : {report.Result}").SemiBold().FontColor("#0F8A6C");
                            left.Item().Text($"Questions repondues : {report.QuestionsAnswered}");
                        });

                        row.ConstantItem(1).Background("#E3E6F0");

                        row.RelativeItem().PaddingLeft(18).Column(right =>
                        {
                            right.Spacing(10);
                            right.Item().Text($"Statut : {report.Status}");
                            right.Item().Text($"Categorie atteinte : {report.Category}");
                            right.Item().Text($"Boss tues : {report.BossesKilled}");
                        });
                    });

                    column.Item().Border(1).BorderColor("#D9DDED").Padding(20).Column(section =>
                    {
                        section.Spacing(10);
                        section.Item().Text("Dates de la partie").FontSize(16).SemiBold().FontColor("#4338CA");
                        section.Item().Text($"Date de debut : {report.CreatedAt}");
                        section.Item().Text($"Date de fin : {report.FinishedAt}");
                    });

                    column.Item().Border(1).BorderColor("#D9DDED").Padding(20).Column(section =>
                    {
                        section.Spacing(10);
                        section.Item().Text("Evaluation").FontSize(16).SemiBold().FontColor("#4338CA");

                        var (title, lines) = BuildEvaluation(report.Score);

                        section.Item().Text(title).FontSize(20).Bold().FontColor("#26235D");

                        foreach (var line in lines)
                        {
                            section.Item().Text(line);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Genere par Clavier D'Or").FontColor("#6B7280");
                });
            });
        }).GeneratePdf(filePath);
    }

    private static (string Title, string[] Lines) BuildEvaluation(int score)
    {
        if (score == 500)
        {
            return (
                "Legende",
                [
                    "Incroyable...",
                    "Tu as atteint la perfection absolue.",
                    "Ton nom restera grave parmi les legendes."
                ]);
        }

        if (score > 400)
        {
            return (
                "Pro",
                [
                    "Impressionnant...",
                    "Ton esprit depasse la moyenne.",
                    "Tu es devenu un veritable professionnel."
                ]);
        }

        if (score >= 250)
        {
            return (
                "Reussi",
                [
                    "Tu as survecu...",
                    "Mais le chemin ne fait que commencer.",
                    "Continue a progresser."
                ]);
        }

        return (
            "Defaite",
            [
                "Tu as echoue...",
                "Le savoir t'echappe encore.",
                "Reviens plus fort."
            ]);
    }
}
