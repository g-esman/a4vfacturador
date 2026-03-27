using FacturacionA4V.UI.ViewModel;
using System.Globalization;

namespace FacturacionA4V.Domain;

public class MesItem : ObservableObject
{
    public int Numero { get; }
    public string Nombre { get; }

    private bool _seleccionado;
    public bool Seleccionado
    {
        get => _seleccionado;
        set
        {
            _seleccionado = value;
            OnPropertyChanged();
        }
    }

    public MesItem(int numero)
    {
        Numero = numero;
        Nombre = new DateTime(2000, numero, 1)
                     .ToString("MMM", new CultureInfo("es-AR"));
    }
}