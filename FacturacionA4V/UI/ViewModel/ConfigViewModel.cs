using FacturacionA4V.Infrastructure;
using System.Windows.Input;

namespace FacturacionA4V.UI.ViewModel;

public sealed class ConfigViewModel : ObservableObject
{
    private readonly AppSettingsService _service;
    private readonly ThemeService _theme;

    // --- Fuentes ---

    private double _fontSizeSmall;
    public double FontSizeSmall
    {
        get => _fontSizeSmall;
        set { _fontSizeSmall = value; OnPropertyChanged(); PreviewTheme(); }
    }

    private double _fontSizeBase;
    public double FontSizeBase
    {
        get => _fontSizeBase;
        set { _fontSizeBase = value; OnPropertyChanged(); PreviewTheme(); }
    }

    private double _fontSizeMedium;
    public double FontSizeMedium
    {
        get => _fontSizeMedium;
        set { _fontSizeMedium = value; OnPropertyChanged(); PreviewTheme(); }
    }

    private double _fontSizeLarge;
    public double FontSizeLarge
    {
        get => _fontSizeLarge;
        set { _fontSizeLarge = value; OnPropertyChanged(); PreviewTheme(); }
    }

    // --- Tema ---

    private bool _darkMode;
    public bool DarkMode
    {
        get => _darkMode;
        set { _darkMode = value; OnPropertyChanged(); PreviewTheme(); }
    }

    // --- Comandos ---

    public ICommand GuardarCommand { get; }
    public ICommand RestaurarCommand { get; }

    public ConfigViewModel(AppSettingsService service, ThemeService theme)
    {
        _service = service;
        _theme = theme;

        var s = _service.Load();
        LoadFrom(s);

        GuardarCommand = new RelayCommand(Guardar);
        RestaurarCommand = new RelayCommand(Restaurar);
    }

    private void LoadFrom(AppUserSettings s)
    {
        _fontSizeSmall  = s.FontSizeSmall;
        _fontSizeBase   = s.FontSizeBase;
        _fontSizeMedium = s.FontSizeMedium;
        _fontSizeLarge  = s.FontSizeLarge;
        _darkMode       = s.DarkMode;

        OnPropertyChanged(nameof(FontSizeSmall));
        OnPropertyChanged(nameof(FontSizeBase));
        OnPropertyChanged(nameof(FontSizeMedium));
        OnPropertyChanged(nameof(FontSizeLarge));
        OnPropertyChanged(nameof(DarkMode));
    }

    private void PreviewTheme()
        => _theme.Apply(BuildSettings());

    private void Guardar()
    {
        var s = BuildSettings();
        _service.Save(s);
        _theme.Apply(s);
    }

    private void Restaurar()
    {
        LoadFrom(AppUserSettings.Default);
        PreviewTheme();
    }

    private AppUserSettings BuildSettings() => new()
    {
        FontSizeSmall  = FontSizeSmall,
        FontSizeBase   = FontSizeBase,
        FontSizeMedium = FontSizeMedium,
        FontSizeLarge  = FontSizeLarge,
        DarkMode       = DarkMode,
    };
}
