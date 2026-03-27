using System.Windows;

namespace FacturacionA4V.UI.Views;

public partial class AgregarInfoAdicionalWindow : Window
{
    public AgregarInfoAdicionalWindow()
    {
        InitializeComponent();
    }

    private void OnAceptar(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
