using FacturacionA4V.Infrastructure;
using FacturacionA4V.UI.ViewModel;
using System.Globalization;
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

        var raw = textBox.Text;

        if (string.IsNullOrWhiteSpace(raw))
            return;

        // Evitar loops infinitos
        textBox.TextChanged -= Monto_TextChanged;

        // Quitar puntos
        raw = raw.Replace(".", "");

        // Separar decimal
        var parts = raw.Split(',');

        var entero = parts[0];
        var decimalPart = parts.Length > 1 ? parts[1] : "";

        if (decimalPart.Length > 2)
            decimalPart = decimalPart.Substring(0, 2);

        if (long.TryParse(entero, out var number))
        {
            var formatted = number.ToString("N0", new CultureInfo("es-AR"));

            if (raw.Contains(','))
                formatted += "," + decimalPart;

            textBox.Text = formatted;
            textBox.CaretIndex = textBox.Text.Length;
        }

        textBox.TextChanged += Monto_TextChanged;
    }
}