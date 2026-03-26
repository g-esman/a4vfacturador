
namespace FacturacionA4V.UI.ViewModel;

public sealed class AgregarFacturaViewModel : ObservableObject
{
    private string _nroFactura = "";
    public string NroFactura
    {
        get => _nroFactura;
        set
        {
            if (_nroFactura == value) return;
            _nroFactura = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsValid)); // 🔥 CLAVE
        }
    }

    private DateTime _fechaFactura = DateTime.Today;
    public DateTime FechaFactura
    {
        get => _fechaFactura;
        set
        {
            _fechaFactura = value;
            OnPropertyChanged();
        }
    }

    private string? _nota;
    public string? Nota
    {
        get => _nota;
        set
        {
            _nota = value;
            OnPropertyChanged();
        }
    }

    public bool IsValid =>
        !string.IsNullOrWhiteSpace(NroFactura);
}