using System;
using System.Windows;
using System.Windows.Media;

namespace FacturacionA4V.Infrastructure;

public sealed class ThemeService
{
    // Paleta clara
    private static readonly Color LightBackground      = Color.FromRgb(0xFF, 0xFF, 0xFF);
    private static readonly Color LightSurface         = Color.FromRgb(0xF5, 0xF5, 0xF5);
    private static readonly Color LightSurfaceAlt      = Color.FromRgb(0xEB, 0xEB, 0xEB);
    private static readonly Color LightForeground      = Color.FromRgb(0x1A, 0x1A, 0x1A);
    private static readonly Color LightForegroundMuted = Color.FromRgb(0x66, 0x66, 0x66);
    private static readonly Color LightBorder          = Color.FromRgb(0xDD, 0xDD, 0xDD);
    private static readonly Color LightInputBg         = Color.FromRgb(0xFF, 0xFF, 0xFF);
    private static readonly Color LightButtonBg        = Color.FromRgb(0xE0, 0xE0, 0xE0);
    private static readonly Color LightButtonHover     = Color.FromRgb(0xCC, 0xCC, 0xCC);
    private static readonly Color LightButtonPress     = Color.FromRgb(0xB8, 0xB8, 0xB8);
    private static readonly Color LightSelection       = Color.FromRgb(0x33, 0x99, 0xFF);

    // Paleta oscura
    private static readonly Color DarkBackground       = Color.FromRgb(0x1E, 0x1E, 0x1E);
    private static readonly Color DarkSurface          = Color.FromRgb(0x2D, 0x2D, 0x2D);
    private static readonly Color DarkSurfaceAlt       = Color.FromRgb(0x3A, 0x3A, 0x3A);
    private static readonly Color DarkForeground       = Color.FromRgb(0xF0, 0xF0, 0xF0);
    private static readonly Color DarkForegroundMuted  = Color.FromRgb(0xAA, 0xAA, 0xAA);
    private static readonly Color DarkBorder           = Color.FromRgb(0x55, 0x55, 0x55);
    private static readonly Color DarkInputBg          = Color.FromRgb(0x3C, 0x3C, 0x3C);
    private static readonly Color DarkButtonBg         = Color.FromRgb(0x3A, 0x3A, 0x3A);
    private static readonly Color DarkButtonHover      = Color.FromRgb(0x4A, 0x4A, 0x4A);
    private static readonly Color DarkButtonPress      = Color.FromRgb(0x55, 0x55, 0x55);
    private static readonly Color DarkSelection        = Color.FromRgb(0x26, 0x4F, 0x78);

    public void Apply(AppUserSettings settings)
    {
        var res = Application.Current.Resources;
        bool dark = settings.DarkMode;

        // --- Fuentes (Small/Medium/Large se derivan del Base) ---
        res["FontSizeBase"]   = settings.FontSizeBase;
        res["FontSizeSmall"]  = Math.Max(8.0,  settings.FontSizeBase - 2);
        res["FontSizeMedium"] = settings.FontSizeBase + 1;
        res["FontSizeLarge"]  = settings.FontSizeBase + 2;

        // --- Brushes de la app ---
        Set(res, "AppBackgroundBrush",      dark ? DarkBackground      : LightBackground);
        Set(res, "AppSurfaceBrush",         dark ? DarkSurface         : LightSurface);
        Set(res, "AppSurfaceAltBrush",      dark ? DarkSurfaceAlt      : LightSurfaceAlt);
        Set(res, "AppForegroundBrush",      dark ? DarkForeground      : LightForeground);
        Set(res, "AppForegroundMutedBrush", dark ? DarkForegroundMuted : LightForegroundMuted);
        Set(res, "AppBorderBrush",          dark ? DarkBorder          : LightBorder);
        Set(res, "AppInputBgBrush",         dark ? DarkInputBg         : LightInputBg);
        Set(res, "AppButtonBgBrush",        dark ? DarkButtonBg        : LightButtonBg);
        Set(res, "AppButtonFgBrush",        dark ? DarkForeground      : LightForeground);
        Set(res, "AppButtonHoverBrush",     dark ? DarkButtonHover     : LightButtonHover);
        Set(res, "AppButtonPressBrush",     dark ? DarkButtonPress     : LightButtonPress);

        // --- Override de SystemColors para los internals de ComboBox, DatePicker, DataGrid ---
        // Estos son los colores que WPF usa dentro de sus ControlTemplates por defecto
        res[SystemColors.WindowBrushKey]      = new SolidColorBrush(dark ? DarkInputBg     : LightInputBg);
        res[SystemColors.WindowTextBrushKey]  = new SolidColorBrush(dark ? DarkForeground  : LightForeground);
        res[SystemColors.ControlBrushKey]     = new SolidColorBrush(dark ? DarkSurface     : LightSurface);
        res[SystemColors.ControlTextBrushKey] = new SolidColorBrush(dark ? DarkForeground  : LightForeground);
        res[SystemColors.ControlLightBrushKey]= new SolidColorBrush(dark ? DarkSurfaceAlt  : LightSurfaceAlt);
        res[SystemColors.ControlDarkBrushKey] = new SolidColorBrush(dark ? DarkBorder      : LightBorder);
        res[SystemColors.GrayTextBrushKey]    = new SolidColorBrush(dark ? DarkForegroundMuted : LightForegroundMuted);
        res[SystemColors.MenuBrushKey]        = new SolidColorBrush(dark ? DarkSurface     : LightSurface);
        res[SystemColors.MenuTextBrushKey]    = new SolidColorBrush(dark ? DarkForeground  : LightForeground);
        res[SystemColors.HighlightBrushKey]               = new SolidColorBrush(dark ? DarkSelection   : LightSelection);
        res[SystemColors.HighlightTextBrushKey]            = new SolidColorBrush(Colors.White);
        res[SystemColors.InactiveSelectionHighlightBrushKey]     = new SolidColorBrush(dark ? DarkSurfaceAlt : LightSurfaceAlt);
        res[SystemColors.InactiveSelectionHighlightTextBrushKey] = new SolidColorBrush(dark ? DarkForeground : LightForeground);
    }

    private static void Set(ResourceDictionary res, string key, Color color)
        => res[key] = new SolidColorBrush(color);
}
