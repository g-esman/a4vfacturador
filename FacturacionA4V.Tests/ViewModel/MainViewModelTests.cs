using FacturacionA4V.Domain;
using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// TEST-054
public class MainViewModelTests
{
    [Fact]
    public void Constructor_ExponeCantidadesDesdeCache()
    {
        var cache = new DatosInicioCache(
            new[] { "A1", "A2", "A3" },
            new[] { "P1", "P2" },
            new[] { "Per1", "Per2", "Per3", "Per4" }
        );

        var vm = new MainViewModel(cache);

        Assert.Equal(3, vm.AuspiciantesCount);
        Assert.Equal(2, vm.ProgramasCount);
        Assert.Equal(4, vm.PeriodistasCount);
    }

    [Fact]
    public void Constructor_CacheVacia_CuentasCero()
    {
        var cache = new DatosInicioCache([], [], []);
        var vm = new MainViewModel(cache);

        Assert.Equal(0, vm.AuspiciantesCount);
        Assert.Equal(0, vm.ProgramasCount);
        Assert.Equal(0, vm.PeriodistasCount);
    }
}
