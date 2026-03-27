using System.IO;
using System.Text.Json;

namespace FacturacionA4V.Infrastructure;

public sealed class AppSettingsService
{
    private static readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "A4VFacturador",
        "user-settings.json");

    private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public AppUserSettings Load()
    {
        try
        {
            if (!File.Exists(_path))
                return AppUserSettings.Default;

            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<AppUserSettings>(json) ?? AppUserSettings.Default;
        }
        catch
        {
            return AppUserSettings.Default;
        }
    }

    public void Save(AppUserSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(settings, _json));
    }
}
