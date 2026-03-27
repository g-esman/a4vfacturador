using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// TEST-046 a TEST-050
public class AgregarFacturaViewModelTests
{
    [Fact]
    public void ValoresIniciales_NroVacioFechaHoyNotaNull()
    {
        var vm = new AgregarFacturaViewModel();

        Assert.Equal("", vm.NroFactura);
        Assert.Equal(DateTime.Today, vm.FechaFactura);
        Assert.Null(vm.Nota);
    }

    [Fact]
    public void IsValid_NroVacio_RetornaFalse()
    {
        var vm = new AgregarFacturaViewModel();
        Assert.False(vm.IsValid);
    }

    [Fact]
    public void IsValid_NroSoloEspacios_RetornaFalse()
    {
        var vm = new AgregarFacturaViewModel { NroFactura = "   " };
        Assert.False(vm.IsValid);
    }

    [Fact]
    public void IsValid_NroValido_RetornaTrue()
    {
        var vm = new AgregarFacturaViewModel { NroFactura = "F-001" };
        Assert.True(vm.IsValid);
    }

    [Fact]
    public void CambioNroFactura_DisparaPropertyChangedDeIsValid()
    {
        var vm = new AgregarFacturaViewModel();
        var properties = new List<string?>();
        vm.PropertyChanged += (_, e) => properties.Add(e.PropertyName);

        vm.NroFactura = "F-001";

        Assert.Contains(nameof(AgregarFacturaViewModel.IsValid), properties);
        Assert.Contains(nameof(AgregarFacturaViewModel.NroFactura), properties);
    }

    [Fact]
    public void CambioNroFactura_MismoValor_NoDisparaPropertyChanged()
    {
        var vm = new AgregarFacturaViewModel { NroFactura = "F-001" };
        int count = 0;
        vm.PropertyChanged += (_, _) => count++;

        vm.NroFactura = "F-001"; // mismo valor

        Assert.Equal(0, count);
    }
}
