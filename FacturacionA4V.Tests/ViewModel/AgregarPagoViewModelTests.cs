using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// TEST-051 a TEST-053
public class AgregarPagoViewModelTests
{
    [Fact]
    public void ValoresIniciales_FechaPagoEsHoy()
    {
        var vm = new AgregarPagoViewModel();
        Assert.Equal(DateTime.Today, vm.FechaPago);
    }

    [Fact]
    public void IsValid_FechaDefault_RetornaFalse()
    {
        var vm = new AgregarPagoViewModel { FechaPago = default };
        Assert.False(vm.IsValid);
    }

    [Fact]
    public void IsValid_FechaValida_RetornaTrue()
    {
        var vm = new AgregarPagoViewModel { FechaPago = DateTime.Today };
        Assert.True(vm.IsValid);
    }

    [Fact]
    public void CambioFechaPago_DisparaPropertyChanged()
    {
        var vm = new AgregarPagoViewModel();
        string? propertyName = null;
        vm.PropertyChanged += (_, e) => propertyName = e.PropertyName;

        vm.FechaPago = DateTime.Today.AddDays(-1);

        Assert.Equal(nameof(AgregarPagoViewModel.FechaPago), propertyName);
    }
}
