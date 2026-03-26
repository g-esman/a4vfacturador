using System.Windows;

namespace FacturacionA4V.UI.Views;

public partial class AgregarFacturaWindow : Window
{
    public AgregarFacturaWindow()
    {
        InitializeComponent();
    }

    private void OnAceptar(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}