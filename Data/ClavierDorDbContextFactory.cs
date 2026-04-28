using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace clavierdor.Data;

public class ClavierDorDbContextFactory : IDesignTimeDbContextFactory<ClavierDorDbContext>
{
    // Cree un DbContext avec la meme connexion que l'application
    public ClavierDorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClavierDorDbContext>();
        optionsBuilder.UseMySql(
            DatabaseSettings.DefaultConnectionString,
            new MariaDbServerVersion(DatabaseSettings.XamppMariaDbVersion));

        return new ClavierDorDbContext(optionsBuilder.Options);
    }
}
