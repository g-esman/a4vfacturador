using FacturacionA4V.Domain;
using FacturacionA4V.UI.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FacturacionA4V.UI.ViewModel;

public sealed class ResultadosViewModel : ObservableObject
{
    public ObservableCollection<string> AuspiciantesDisponibles { get; } = [];
    public ObservableCollection<string> ProgramasDisponibles { get; } = [];
    public ObservableCollection<string> PeriodistasDisponibles { get; } = [];
    public ObservableCollection<int> AniosDisponibles { get; } = [];

    private string? _auspicianteSeleccionado;
    public string? AuspicianteSeleccionado
    {
        get => _auspicianteSeleccionado;
        set
        {
            _auspicianteSeleccionado = value;
            OnPropertyChanged();
            AplicarFiltros();
        }
    }

    private string? _programaSeleccionado;
    public string? ProgramaSeleccionado
    {
        get => _programaSeleccionado;
        set
        {
            _programaSeleccionado = value;
            OnPropertyChanged();
            AplicarFiltros();
        }
    }

    private string? _periodistaSeleccionado;
    public string? PeriodistaSeleccionado
    {
        get => _periodistaSeleccionado;
        set
        {
            _periodistaSeleccionado = value;
            OnPropertyChanged();
            AplicarFiltros();
        }
    }

    private int? _anioSeleccionado;
    public int? AnioSeleccionado
    {
        get => _anioSeleccionado;
        set
        {
            _anioSeleccionado = value;
            OnPropertyChanged();
            AplicarFiltros();
        }
    }

    private readonly IFacturacionRepository _repo;

    private IReadOnlyList<FacturacionItem> _all = [];

    public ObservableCollection<FacturacionItem> Filtrados { get; } = [];

    public decimal TotalFacturado { get; private set; }
    public decimal TotalCobrado { get; private set; }
    public decimal TotalPendiente { get; private set; }

    public ICommand FiltrarCommand { get; }
    private IEnumerable<FacturacionItem> Seleccionados =>
    Filtrados.Where(x => x.IsSelected);

    public ICommand AgregarFacturaCommand { get; }

    public ICommand AgregarPagoCommand { get; }

    public ICommand LimpiarFiltrosCommand { get; }

    public ResultadosViewModel(IFacturacionRepository repo)
    {
        _repo = repo;
        FiltrarCommand = new RelayCommand(AplicarFiltros);
        AgregarFacturaCommand = new RelayCommand(AbrirAgregarFactura, PuedeAgregarFactura);
        AgregarPagoCommand = new RelayCommand(AbrirAgregarPago, PuedeAgregarPago);
        LimpiarFiltrosCommand = new RelayCommand(LimpiarFiltros);
        Cargar();
    }

    private void LimpiarFiltros()
    {
        AuspicianteSeleccionado = null;
        ProgramaSeleccionado = null;
        PeriodistaSeleccionado = null;
        AnioSeleccionado = null;

        AplicarFiltros();
    }

    private void AbrirAgregarPago()
    {
        var seleccionados = Filtrados.Where(x => x.IsSelected).ToList();
        if (!seleccionados.Any())
            return;

        var vm = new AgregarPagoViewModel();

        var dlg = new AgregarPagoWindow { DataContext = vm };
        if (dlg.ShowDialog() != true)
            return;

        var updates = seleccionados.Select(x => new PagoUpdate
        {
            Id = x.Id,
            FechaPago = vm.FechaPago
        });

        _repo.UpdatePago(updates);
        Cargar();
    }

    private bool PuedeAgregarPago()
    {
        var seleccionados = Filtrados.Where(x => x.IsSelected).ToList();

        return seleccionados.Count > 0
            && seleccionados.All(x =>
                !string.IsNullOrWhiteSpace(x.NroFactura) &&
                 string.IsNullOrWhiteSpace(x.FechaPago));
    }
    private bool PuedeAgregarFactura()
    {
        var selected = Seleccionados.ToList();

        return selected.Count > 0 &&
               selected.All(x => string.IsNullOrWhiteSpace(x.NroFactura));
    }

    private void AbrirAgregarFactura()
    {
        var seleccionados = Seleccionados.ToList();
        if (!seleccionados.Any())
            return;

        var vm = new AgregarFacturaViewModel();

        var dlg = new AgregarFacturaWindow { DataContext = vm };
        if (dlg.ShowDialog() != true)
            return;

        var updates = seleccionados.Select(x => new FacturaUpdate
        {
            Id = x.Id,
            NroFactura = vm.NroFactura,
            FechaFactura = vm.FechaFactura,
            Nota = vm.Nota
        });

        _repo.UpdateFactura(updates);
        Cargar();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FacturacionItem.IsSelected))
        {
            ((RelayCommand)AgregarFacturaCommand).RaiseCanExecuteChanged();
            ((RelayCommand)AgregarPagoCommand).RaiseCanExecuteChanged();
        }
    }

    public void Cargar()
    {
        _all = _repo.ReadAll();
        AuspiciantesDisponibles.Clear();
        ProgramasDisponibles.Clear();
        PeriodistasDisponibles.Clear();
        AniosDisponibles.Clear();

        foreach (var item in _all)
        {
            item.PropertyChanged += OnItemPropertyChanged; // BUG-001 fix: suscribir acá, no en el loop separado

            if (!AuspiciantesDisponibles.Contains(item.Auspiciante))
                AuspiciantesDisponibles.Add(item.Auspiciante);

            if (!ProgramasDisponibles.Contains(item.Programa))
                ProgramasDisponibles.Add(item.Programa);

            if (!PeriodistasDisponibles.Contains(item.Periodista))
                PeriodistasDisponibles.Add(item.Periodista);

            var year = int.Parse(item.MesAnio.Split('/')[1]);
            if (!AniosDisponibles.Contains(year))
                AniosDisponibles.Add(year);
        }

        AplicarFiltros(); // maneja Filtrados + RecalcularTotales internamente
        ((RelayCommand)AgregarFacturaCommand).RaiseCanExecuteChanged();
    }

    private void AplicarFiltros()
    {
        Filtrados.Clear();

        foreach (var item in _all)
        {
            if (AuspicianteSeleccionado != null &&
                item.Auspiciante != AuspicianteSeleccionado)
                continue;

            if (ProgramaSeleccionado != null &&
                item.Programa != ProgramaSeleccionado)
                continue;

            if (PeriodistaSeleccionado != null &&
                item.Periodista != PeriodistaSeleccionado)
                continue;

            if (AnioSeleccionado != null)
            {
                var year = int.Parse(item.MesAnio.Split('/')[1]);
                if (year != AnioSeleccionado)
                    continue;
            }

            Filtrados.Add(item);
        }

        RecalcularTotales();
    }


    private void RecalcularTotales()
    {
        TotalFacturado = Filtrados
            .Where(x => x.MontoParsed.HasValue)
            .Sum(x => x.MontoParsed!.Value);

        TotalCobrado = Filtrados
            .Where(x => !string.IsNullOrWhiteSpace(x.FechaPago) && x.MontoParsed.HasValue)
            .Sum(x => x.MontoParsed!.Value);

        TotalPendiente = TotalFacturado - TotalCobrado;

        OnPropertyChanged(nameof(TotalFacturado));
        OnPropertyChanged(nameof(TotalCobrado));
        OnPropertyChanged(nameof(TotalPendiente));
    }


}
