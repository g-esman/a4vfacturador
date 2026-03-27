using FacturacionA4V.Domain;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FacturacionA4V.UI.ViewModel;

public sealed class CargaViewModel : ObservableObject
{
    private readonly DatosInicioCache _cache;
    private readonly IFacturacionRepository _repo;
    public ICommand ToggleMesCommand { get; }

    public ObservableCollection<MesItem> Meses { get; } =
        new ObservableCollection<MesItem>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }
            .Select(m => new MesItem(m)));

    public HashSet<string> MesesSeleccionados { get; } = [];

    public ObservableCollection<string> Auspiciantes { get; }
    private string? _auspicianteSeleccionado;
    public string? AuspicianteSeleccionado
    {
        get => _auspicianteSeleccionado;
        set
        {
            _auspicianteSeleccionado = value;
            OnPropertyChanged();
            ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<string> Periodistas { get; }
    private string? _periodistaSeleccionado;
    public string? PeriodistaSeleccionado
    {
        get => _periodistaSeleccionado;
        set
        {
            _periodistaSeleccionado = value;
            OnPropertyChanged();
            ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
        }
    }
    public ObservableCollection<string> Programas { get; }
    private string? _programaSeleccionado;
    public string? ProgramaSeleccionado
    {
        get => _programaSeleccionado;
        set
        {
            _programaSeleccionado = value;
            OnPropertyChanged();
            ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
        }
    }


    private string? _montoTexto;
    public string? MontoTexto
    {
        get => _montoTexto;
        set
        {
            _montoTexto = value;
            OnPropertyChanged();
            ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
        }
    }

    private string _tipoFactura = "Factura A";
    public string TipoFactura
    {
        get => _tipoFactura;
        set { _tipoFactura = value; OnPropertyChanged(); }
    }

    private string _anioTexto = DateTime.Now.Year.ToString();
    public string AnioTexto
    {
        get => _anioTexto;
        set
        {
            _anioTexto = value;
            OnPropertyChanged();
            ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
        }
    }

    public int Anio
    {
        get
        {
            if (int.TryParse(_anioTexto, out var y))
                return y;

            return DateTime.Now.Year;
        }
    }
    public ICommand ProcesarCommand { get; }

    public CargaViewModel(DatosInicioCache cache, IFacturacionRepository repo)
    {
        _cache = cache;
        _repo = repo;
        ProcesarCommand = new RelayCommand(Procesar, PuedeProcesar);
        ToggleMesCommand = new RelayCommand<string>(ToggleMes);
        Auspiciantes = new ObservableCollection<string>(_cache.Auspiciantes);
        Programas = new ObservableCollection<string>(_cache.Programas);
        Periodistas = new ObservableCollection<string>(_cache.Periodistas);

        foreach (var mes in Meses)
        {
            mes.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(MesItem.Seleccionado))
                    ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
            };
        }
    }

    private bool PuedeProcesar()
    {
        return
            !string.IsNullOrWhiteSpace(AuspicianteSeleccionado) &&
            !string.IsNullOrWhiteSpace(ProgramaSeleccionado) &&
            !string.IsNullOrWhiteSpace(PeriodistaSeleccionado) &&
            !string.IsNullOrWhiteSpace(MontoTexto) &&
            Meses.Any(m => m.Seleccionado) &&
            _cache.Auspiciantes.Contains(AuspicianteSeleccionado) &&
            _cache.Programas.Contains(ProgramaSeleccionado) &&
            _cache.Periodistas.Contains(PeriodistaSeleccionado);
    }

    private void Procesar()
    {
        var rows = new List<FacturacionRow>();
        foreach (var mes in Meses.Where(m => m.Seleccionado))
        {
            var mesAnio = $"{mes.Numero:D2}/{Anio}";
            rows.Add(new FacturacionRow
            {
                Id = Guid.NewGuid(),
                Auspiciante = AuspicianteSeleccionado!,
                Programa = ProgramaSeleccionado!,
                Periodista = PeriodistaSeleccionado!,
                Monto = MontoTexto!,
                TipoFactura = TipoFactura,
                MesAnio = mesAnio,
            });
        }

        _repo.InsertMany(rows);
        Limpiar();
        ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
    }

    private void Limpiar()
    {
        AuspicianteSeleccionado = ProgramaSeleccionado = PeriodistaSeleccionado = MontoTexto = null;
        MesesSeleccionados.Clear();
        foreach (var mes in Meses)
            mes.Seleccionado = false;
        OnPropertyChanged(nameof(MesesSeleccionados));
    }

    // Lo vamos a llamar cuando conectemos los toggles
    public void ToggleMes(string mes)
    {
        if (MesesSeleccionados.Contains(mes))
            MesesSeleccionados.Remove(mes);
        else
            MesesSeleccionados.Add(mes);

        ((RelayCommand)ProcesarCommand).RaiseCanExecuteChanged();
    }
}
