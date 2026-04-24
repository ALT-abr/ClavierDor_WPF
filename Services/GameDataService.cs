using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using clavierdor.Data;
using clavierdor.Models;
using Microsoft.EntityFrameworkCore;

namespace clavierdor.Services;

public class GameDataService
{
    public Partie? LastCreatedPartie { get; private set; }

    public Partie CreatePartie(string playerName, string pouvoir)
    {
        using var context = new ClavierDorDbContext();

        var normalizedName = playerName.Trim();
        var normalizedPouvoir = pouvoir.Trim();

        var player = context.Players
            .FirstOrDefault(x => x.Name.ToLower() == normalizedName.ToLower());

        if (player is null)
        {
            player = new Player
            {
                Name = normalizedName,
                Pouvoir = normalizedPouvoir
            };

            context.Players.Add(player);
        }
        else
        {
            player.Pouvoir = normalizedPouvoir;
        }

        var partie = new Partie
        {
            Player = player,
            Pouvoir = normalizedPouvoir,
            Category = "General",
            CurrentQuestionIndex = 0,
            Score = 0,
            IsFinished = false,
            CreatedAt = DateTime.Now
        };

        context.Parties.Add(partie);
        context.SaveChanges();

        var history = new History
        {
            PartieId = partie.Id,
            PlayerName = player.Name,
            Pouvoir = partie.Pouvoir,
            Category = partie.Category,
            Score = partie.Score,
            IsFinished = partie.IsFinished,
            PlayedAt = partie.CreatedAt,
            WonBoss = false,
            BossesKilled = string.Empty
        };

        context.Histories.Add(history);
        context.SaveChanges();

        LastCreatedPartie = partie;
        return partie;
    }

    public Partie? GetPartieById(int partieId)
    {
        using var context = new ClavierDorDbContext();

        return context.Parties
            .Include(x => x.Player)
            .FirstOrDefault(x => x.Id == partieId);
    }

    public IReadOnlyList<Question> GetOrderedQuizQuestions(IEnumerable<string> categories)
    {
        using var context = new ClavierDorDbContext();

        var result = new List<Question>();

        foreach (var category in categories)
        {
            var normalizedCategory = category.Trim().ToLower();

            var normalQuestions = context.Questions
                .Where(x =>
                    x.Category.ToLower() == normalizedCategory &&
                    !x.IsBoss &&
                    !x.IsFinalBoss)
                .OrderBy(x => x.Id)
                .ToList();

            result.AddRange(normalQuestions);

            var bossQuestion = context.Questions
                .Where(x =>
                    x.Category.ToLower() == normalizedCategory &&
                    x.IsBoss &&
                    !x.IsFinalBoss)
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            if (bossQuestion is not null)
            {
                result.Add(bossQuestion);
            }
        }

        var finalBoss = context.Questions
            .Where(x => x.IsFinalBoss)
            .OrderBy(x => x.Id)
            .FirstOrDefault();

        if (finalBoss is not null)
        {
            result.Add(finalBoss);
        }

        return result;
    }

    public void SavePartie(Partie partie, string? defeatedBossName = null)
    {
        using var context = new ClavierDorDbContext();

        context.Parties.Update(partie);

        var history = context.Histories
            .FirstOrDefault(x => x.PartieId == partie.Id);

        if (history is not null)
        {
            history.PlayerName = partie.Player?.Name ?? history.PlayerName;
            history.Pouvoir = partie.Pouvoir;
            history.Category = partie.Category;
            history.Score = partie.Score;
            history.IsFinished = partie.IsFinished;
            history.PlayedAt = partie.FinishedAt ?? partie.CreatedAt;

            if (!string.IsNullOrWhiteSpace(defeatedBossName))
            {
                history.WonBoss = true;

                var existingBosses = string.IsNullOrWhiteSpace(history.BossesKilled)
                    ? new List<string>()
                    : history.BossesKilled
                        .Split('|', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();

                if (!existingBosses.Any(x => x.Equals(defeatedBossName, StringComparison.OrdinalIgnoreCase)))
                {
                    existingBosses.Add(defeatedBossName.Trim());
                }

                history.BossesKilled = string.Join("|", existingBosses);
            }
        }

        context.SaveChanges();
    }

    public Partie? FindOpenPartie(string playerName)
    {
        using var context = new ClavierDorDbContext();

        var normalizedName = playerName.Trim();

        return context.Parties
            .Include(x => x.Player)
            .Where(x => !x.IsFinished && x.Player != null && x.Player.Name.ToLower() == normalizedName.ToLower())
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();
    }

    public IReadOnlyList<History> GetHistories()
    {
        using var context = new ClavierDorDbContext();

        return context.Histories
            .OrderByDescending(x => x.PlayedAt)
            .ToList();
    }

    public IReadOnlyList<string> GetPlayerNames()
    {
        using var context = new ClavierDorDbContext();

        return context.Players
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .Distinct()
            .ToList();
    }

    public History? GetLatestHistoryForPlayer(string playerName)
    {
        var normalizedName = playerName.Trim();

        try
        {
            using var context = new ClavierDorDbContext();

            return context.Histories
                .Include(x => x.Partie)
                .OrderByDescending(x => x.PlayedAt)
                .FirstOrDefault(x => x.PlayerName.ToLower() == normalizedName.ToLower());
        }
        catch (Exception ex) when (ex.Message.Contains("BossesKilled", StringComparison.OrdinalIgnoreCase))
        {
            return GetLatestHistoryForPlayerWithoutBossesKilled(normalizedName);
        }
    }

    public ExportReportData? GetExportReportData(string playerName)
    {
        var history = GetLatestHistoryForPlayer(playerName);

        if (history is null)
        {
            return null;
        }

        var questionsAnswered = $"{history.Partie?.CurrentQuestionIndex ?? 0}/100";
        var bossesKilled = string.IsNullOrWhiteSpace(history.BossesKilled)
            ? "Aucun boss tue"
            : string.Join(", ", history.BossesKilled
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()));

        return new ExportReportData
        {
            PlayerName = history.PlayerName,
            Pouvoir = history.Pouvoir,
            Score = history.Score,
            Result = history.IsFinished ? "Partie terminee" : "Partie en cours",
            CreatedAt = history.Partie?.CreatedAt.ToString("dd/MM/yyyy HH:mm") ?? history.PlayedAt.ToString("dd/MM/yyyy HH:mm"),
            FinishedAt = history.Partie?.FinishedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-",
            Category = string.IsNullOrWhiteSpace(history.Category) ? "-" : history.Category,
            Status = history.IsFinished ? "Terminee" : "En cours",
            BossesKilled = bossesKilled,
            QuestionsAnswered = questionsAnswered
        };
    }

    private History? GetLatestHistoryForPlayerWithoutBossesKilled(string normalizedName)
    {
        using var context = new ClavierDorDbContext();
        using var connection = context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT h.Id,
                   h.PartieId,
                   h.PlayerName,
                   h.Pouvoir,
                   h.Category,
                   h.Score,
                   h.IsFinished,
                   h.PlayedAt,
                   h.WonBoss,
                   p.CreatedAt,
                   p.FinishedAt,
                   p.CurrentQuestionIndex
            FROM histories h
            LEFT JOIN parties p ON p.Id = h.PartieId
            WHERE LOWER(h.PlayerName) = LOWER(@playerName)
            ORDER BY h.PlayedAt DESC
            LIMIT 1;
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@playerName";
        parameter.Value = normalizedName;
        command.Parameters.Add(parameter);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        var history = new History
        {
            Id = reader.GetInt32(0),
            PartieId = reader.GetInt32(1),
            PlayerName = reader.GetString(2),
            Pouvoir = reader.GetString(3),
            Category = reader.GetString(4),
            Score = reader.GetInt32(5),
            IsFinished = reader.GetBoolean(6),
            PlayedAt = reader.GetDateTime(7),
            WonBoss = reader.GetBoolean(8),
            BossesKilled = string.Empty,
            Partie = new Partie
            {
                Id = reader.GetInt32(1),
                CreatedAt = reader.IsDBNull(9) ? DateTime.UtcNow : reader.GetDateTime(9),
                FinishedAt = reader.IsDBNull(10) ? null : reader.GetDateTime(10),
                CurrentQuestionIndex = reader.IsDBNull(11) ? 0 : reader.GetInt32(11)
            }
        };

        return history;
    }
}
