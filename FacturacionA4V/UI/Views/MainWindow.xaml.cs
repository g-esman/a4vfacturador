using FacturacionA4V.Domain;
using FacturacionA4V.Infrastructure;
using FacturacionA4V.UI.ViewModel;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;

namespace FacturacionA4V.UI.Views;

public partial class MainWindow : Window
{
    private readonly CargaView _cargaView;
    private readonly ResultadosView _resultadosView;

    public MainWindow()
    {
        InitializeComponent();

        SetLocalVersion();
        // Crear instancias de las vistas (UserControls)
        _cargaView = new CargaView();
        _resultadosView = new ResultadosView();

        // Pantalla inicial
        ShellContent.Content = _cargaView;
    }

    private async Task SetLocalVersion()
    {
        var driveSettings = App.Configuration
    .GetSection("GoogleDrive")
    .Get<GoogleDriveSettings>();

        var drive = new GoogleDriveFileService(
            driveSettings.ServiceAccountPath,
            driveSettings.FileId);

        var stream = await drive.DownloadFile();

        var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Facturacion.xlsx");

        System.IO.File.WriteAllBytes(localPath, stream.ToArray());

    }

    private void OnCargaClick(object sender, RoutedEventArgs e)
    {
        ShellContent.Content = _cargaView;
    }

    private void OnResultadosClick(object sender, RoutedEventArgs e)
    {
        ShellContent.Content = _resultadosView;

        if (_resultadosView.DataContext is ResultadosViewModel vm)
        {
            vm.Cargar();
        }
    }
}