namespace FacturacionA4V.Infrastructure;

public sealed class AppUserSettings
{
    // --- Tipografía ---
    public double FontSizeBase { get; set; } = 14;

    // --- Tema ---
    public bool DarkMode { get; set; } = false;

    public static AppUserSettings Default => new();
}
