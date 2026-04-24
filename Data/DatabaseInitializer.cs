using System;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace clavierdor.Data;

public static class DatabaseInitializer
{
    public static void Initialize()
    {
        try
        {
            using var context = new ClavierDorDbContext();
            context.Database.Migrate();
            EnsureBossesKilledColumnExists(context);
        }
        catch (Exception)
        {
            // L'application continue meme si MySQL/XAMPP n'est pas encore disponible.
        }
    }

    private static void EnsureBossesKilledColumnExists(ClavierDorDbContext context)
    {
        var connection = context.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText =
            """
            SELECT COUNT(*)
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = 'histories'
              AND COLUMN_NAME = 'BossesKilled';
            """;

        var existsResult = existsCommand.ExecuteScalar();
        var exists = Convert.ToInt32(existsResult) > 0;

        if (exists)
        {
            return;
        }

        using var alterCommand = connection.CreateCommand();
        alterCommand.CommandText =
            """
            ALTER TABLE histories
            ADD COLUMN BossesKilled varchar(500) NOT NULL DEFAULT '';
            """;
        alterCommand.ExecuteNonQuery();
    }
}
