using System;
using Microsoft.EntityFrameworkCore;

namespace clavierdor.Data;

// Prepare la base de donnees au demarrage de l'application.
public static class DatabaseInitializer
{
    // Applique les migrations Entity Framework.
    public static void Initialize()
    {
        try
        {
            using var context = new ClavierDorDbContext();
            context.Database.Migrate();
        }
        catch (Exception)
        {
            // L'application continue meme si XAMPP n'est pas ouvert.
        }
    }
}
