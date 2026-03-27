using FacturacionA4V.Infrastructure;
using FacturacionA4V.UI.Helpers;
using FacturacionA4V.UI.ViewModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FacturacionA4V.UI.Views;

public partial class CargaView : UserControl
{
    public CargaView()
    {
        InitializeComponent();

        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        var datosRepo = new ExcelDatosInicioRepository(basePath);
        var datosCache = datosRepo.Load();

        var factRepo = new ExcelFacturacionRepository(basePath);

        DataContext = new CargaViewModel(datosCache, factRepo);

    }

    private void Monto_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        // Solo números y coma
        e.Handled = !Regex.IsMatch(e.Text, @"[\d,]");
    }

    private void Monto_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox == null) return;

        textBox.TextChanged -= Monto_TextChanged;

        var formatted = MontoFormatter.FormatearMonto(textBox.Text);
        if (formatted != null)
        {
            textBox.Text = formatted;
            textBox.CaretIndex = textBox.Text.Length;
        }

        textBox.TextChanged += Monto_TextChanged;
    }
}