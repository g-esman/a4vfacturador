using System.Windows;
using System.Windows.Media;

namespace FacturacionA4V.Infrastructure;

public sealed class ThemeService
{
    // Paleta clara
    private static readonly Color LightBackground     = Color.FromRgb(0xFF, 0xFF, 0xFF);
    private static readonly Color LightSurface        = Color.FromRgb(0xF5, 0xF5, 0xF5);
    private static readonly Color LightSurfaceAlt     = Color.FromRgb(0xEB, 0xEB, 0xEB);
    private static readonly Color LightForeground     = Color.FromRgb(0x1A, 0x1A, 0x1A);
    private static readonly Color LightForegroundMuted= Color.FromRgb(0x66, 0x66, 0x66);
    private static readonly Color LightBorder         = Color.FromRgb(0xDD, 0xDD, 0xDD);
    private static readonly Color LightInputBg        = Color.FromRgb(0xFF, 0xFF, 0xFF);

    // Paleta oscura
    private static readonly Color DarkBackground      = Color.FromRgb(0x1E, 0x1E, 0x1E);
    private static readonly Color DarkSurface         = Color.FromRgb(0x2D, 0x2D, 0x2D);
    private static readonly Color DarkSurfaceAlt      = Color.FromRgb(0x3A, 0x3A, 0x3A);
    private static readonly Color DarkForeground      = Color.FromRgb(0xF0, 0xF0, 0xF0);
    private static readonly Color DarkForegroundMuted = Color.FromRgb(0xAA, 0xAA, 0xAA);
    private static readonly Color DarkBorder          = Color.FromRgb(0x55, 0x55, 0x55);
    private static readonly Color DarkInputBg         = Color.FromRgb(0x3C, 0x3C, 0x3C);

    public void Apply(AppUserSettings settings)
    {
        var res = Application.Current.Resources;

        // Fuentes
        res["FontSizeSmall"]  = settings.FontSizeSmall;
        res["FontSizeBase"]   = settings.FontSizeBase;
        res["FontSizeMedium"] = settings.FontSizeMedium;
        res["FontSizeLarge"]  = settings.FontSizeLarge;

        // Colores según modo
        bool dark = settings.DarkMode;

        Set(res, "AppBackgroundBrush",      dark ? DarkBackground      : LightBackground);
        Set(res, "AppSurfaceBrush",         dark ? DarkSurface         : LightSurface);
        Set(res, "AppSurfaceAltBrush",      dark ? DarkSurfaceAlt      : LightSurfaceAlt);
        Set(res, "AppForegroundBrush",      dark ? DarkForeground      : LightForeground);
        Set(res, "AppForegroundMutedBrush", dark ? DarkForegroundMuted : LightForegroundMuted);
        Set(res, "AppBorderBrush",          dark ? DarkBorder          : LightBorder);
        Set(res, "AppInputBgBrush",         dark ? DarkInputBg         : LightInputBg);
    }

    private static void Set(ResourceDictionary res, string key, Color color)
        => res[key] = new SolidColorBrush(color);
}
