using clavierdor.Models;
using Microsoft.EntityFrameworkCore;

namespace clavierdor.Data;

public class ClavierDorDbContext : DbContext
{
    public ClavierDorDbContext()
    {
    }

    public ClavierDorDbContext(DbContextOptions<ClavierDorDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Partie> Parties => Set<Partie>();
    public DbSet<History> Histories => Set<History>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        optionsBuilder.UseMySql(
            DatabaseSettings.DefaultConnectionString,
            new MariaDbServerVersion(DatabaseSettings.XamppMariaDbVersion));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("players");
            entity.HasIndex(x => x.Name);
            entity.Property(x => x.Pouvoir).HasMaxLength(50);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("questions");
            entity.Property(x => x.CorrectAnswer).HasMaxLength(1);
            entity.Property(x => x.BossName).HasMaxLength(120);
        });

        modelBuilder.Entity<Partie>(entity =>
        {
            entity.ToTable("parties");
            entity.Property(x => x.Pouvoir).HasMaxLength(50);
            entity.HasOne(x => x.Player)
                .WithMany(x => x.Parties)
                .HasForeignKey(x => x.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.ToTable("histories");
            entity.Property(x => x.Pouvoir).HasMaxLength(50);
            entity.Property(x => x.BossesKilled).HasMaxLength(500);
            entity.HasOne(x => x.Partie)
                .WithMany(x => x.HistoryEntries)
                .HasForeignKey(x => x.PartieId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
