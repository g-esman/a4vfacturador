using FacturacionA4V.Domain;
using FacturacionA4V.Infrastructure;
using FacturacionA4V.UI.ViewModel;
using FacturacionA4V.UI.Views;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace FacturacionA4V;

public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var basePath = AppDomain.CurrentDomain.BaseDirectory;

        IDatosInicioRepository repo = new ExcelDatosInicioRepository(basePath);
        DatosInicioCache cache = repo.Load(); // <- si falla, que falle acá (mejor temprano)

        var vm = new MainViewModel(cache);
        var window = new MainWindow { DataContext = vm };
        window.Show();
    }
}
