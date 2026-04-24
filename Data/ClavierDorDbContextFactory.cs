using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace clavierdor.Data;

public class ClavierDorDbContextFactory : IDesignTimeDbContextFactory<ClavierDorDbContext>
{
    public ClavierDorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClavierDorDbContext>();
        optionsBuilder.UseMySql(
            DatabaseSettings.DefaultConnectionString,
            new MariaDbServerVersion(DatabaseSettings.XamppMariaDbVersion));

        return new ClavierDorDbContext(optionsBuilder.Options);
    }
}
