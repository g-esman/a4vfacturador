namespace FacturacionA4V.UI.ViewModel;

public sealed class AgregarPagoViewModel : ObservableObject
{
    private DateTime _fechaPago = DateTime.Today;
    public DateTime FechaPago
    {
        get => _fechaPago;
        set
        {
            _fechaPago = value;
            OnPropertyChanged();
        }
    }

    public bool IsValid => FechaPago != default;
}