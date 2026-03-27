using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Domain;

public sealed class FacturacionItem : ObservableObject
{
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public Guid Id { get; init; }

    public string Auspiciante { get; init; } = "";
    public string Programa { get; init; } = "";
    public string Periodista { get; init; } = "";
    public string MontoTexto { get; init; } = "";
    public string MesAnio { get; init; } = "";

    public string? NroFactura { get; init; }
    public string FechaFactura { get; init; } = "";
    public string FechaPago { get; init; } = "";
    public string Nota { get; init; } = "";

    // Derivados (NO vienen de Excel)
    public decimal? MontoParsed { get; init; }
    public EstadoFactura Estado { get; init; }

    public DateTime? FechaPagoDate =>
        DateTime.TryParseExact(FechaPago, "dd/MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var d) ? d : null;
}
