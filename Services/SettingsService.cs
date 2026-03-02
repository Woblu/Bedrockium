using AmethystLauncher.Data;
using AmethystLauncher.Models;
using Microsoft.EntityFrameworkCore;

namespace AmethystLauncher.Services;

public class SettingsService(AmethystDbContext db) : ISettingsService
{
    public async Task<AppSettings> GetSettingsAsync()
    {
        var settings = await db.Settings.FirstOrDefaultAsync();
        if (settings is not null)
            return settings;
        var defaultSettings = new AppSettings();
        db.Settings.Add(defaultSettings);
        await db.SaveChangesAsync();
        return defaultSettings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        db.Settings.Update(settings);
        await db.SaveChangesAsync();
    }
}
