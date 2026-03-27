using FacturacionA4V.Infrastructure;
using System.Windows.Input;
using System.Windows.Threading;

namespace FacturacionA4V.UI.ViewModel;

public sealed class ConfigViewModel : ObservableObject
{
    private readonly AppSettingsService _service;
    private readonly ThemeService _theme;
    private readonly DispatcherTimer _guardadoTimer;

    private bool _isGuardado;
    public bool IsGuardado
    {
        get => _isGuardado;
        private set { _isGuardado = value; OnPropertyChanged(); }
    }

    // --- Fuentes ---

    private double _fontSizeBase;
    public double FontSizeBase
    {
        get => _fontSizeBase;
        set { _fontSizeBase = value; OnPropertyChanged(); PreviewTheme(); }
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

        _guardadoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _guardadoTimer.Tick += (_, _) => { _guardadoTimer.Stop(); IsGuardado = false; };

        var s = _service.Load();
        LoadFrom(s);

        GuardarCommand = new RelayCommand(Guardar);
        RestaurarCommand = new RelayCommand(Restaurar);
    }

    private void LoadFrom(AppUserSettings s)
    {
        _fontSizeBase = s.FontSizeBase;
        _darkMode     = s.DarkMode;

        OnPropertyChanged(nameof(FontSizeBase));
        OnPropertyChanged(nameof(DarkMode));
    }

    private void PreviewTheme()
        => _theme.Apply(BuildSettings());

    private void Guardar()
    {
        var s = BuildSettings();
        _service.Save(s);
        _theme.Apply(s);
        IsGuardado = true;
        _guardadoTimer.Stop();
        _guardadoTimer.Start();
    }

    private void Restaurar()
    {
        LoadFrom(AppUserSettings.Default);
        PreviewTheme();
    }

    private AppUserSettings BuildSettings() => new()
    {
        FontSizeBase = FontSizeBase,
        DarkMode     = DarkMode,
    };
}
