using System.Windows.Controls;
using FacturacionA4V.Domain;
using FacturacionA4V.Infrastructure;
using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.UI.Views;

public partial class ResultadosView : UserControl
{
    public ResultadosView()
    {
        InitializeComponent();

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var repo = new ExcelFacturacionRepository(basePath);

        DataContext = new ResultadosViewModel(repo);
    }

}