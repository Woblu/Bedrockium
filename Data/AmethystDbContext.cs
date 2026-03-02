using System.IO;
using AmethystLauncher.Models;
using Microsoft.EntityFrameworkCore;

namespace AmethystLauncher.Data;

public class AmethystDbContext : DbContext
{
    public DbSet<AppSettings> Settings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var launcherPath = Path.Combine(appDataPath, "AmethystLauncher");
        Directory.CreateDirectory(launcherPath);
        var dbPath = Path.Combine(launcherPath, "amethyst.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
