namespace FacturacionA4V.UI.ViewModel;

public sealed class AgregarInfoAdicionalViewModel : ObservableObject
{
    private string _informacionAdicional = "";
    public string InformacionAdicional
    {
        get => _informacionAdicional;
        set
        {
            _informacionAdicional = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsValid));
        }
    }

    public bool IsValid => !string.IsNullOrWhiteSpace(InformacionAdicional);
}
