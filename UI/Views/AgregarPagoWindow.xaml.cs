using System.Windows;

namespace FacturacionA4V.UI.Views;

public partial class AgregarPagoWindow : Window
{
    public AgregarPagoWindow()
    {
        InitializeComponent();
    }

    private void OnAceptar(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}