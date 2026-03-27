namespace FacturacionA4V.Infrastructure;

public sealed class AppUserSettings
{
    // --- Tipografía ---
    public double FontSizeSmall  { get; set; } = 12;
    public double FontSizeBase   { get; set; } = 14;
    public double FontSizeMedium { get; set; } = 15;
    public double FontSizeLarge  { get; set; } = 16;

    // --- Tema ---
    public bool DarkMode { get; set; } = false;

    public static AppUserSettings Default => new();
}
