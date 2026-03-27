using FacturacionA4V.Domain;
using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// TEST-055 a TEST-062
public class CargaViewModelTests
{
    private readonly DatosInicioCache _cache = new DatosInicioCache(
        new[] { "Ausp1", "Ausp2" },
        new[] { "Prog1" },
        new[] { "Per1" }
    );

    private readonly Mock<IFacturacionRepository> _repoMock = new();

    private CargaViewModel CreateVm() => new CargaViewModel(_cache, _repoMock.Object);

    private void SetearCompleto(CargaViewModel vm, int[] meses)
    {
        vm.AuspicianteSeleccionado = "Ausp1";
        vm.ProgramaSeleccionado = "Prog1";
        vm.PeriodistaSeleccionado = "Per1";
        vm.MontoTexto = "1.000";
        foreach (var m in meses)
            vm.Meses[m - 1].Seleccionado = true;
    }

    // TEST-055
    [Fact]
    public void ValoresIniciales_AnioActualTipoFacturaASinMeses()
    {
        var vm = CreateVm();

        Assert.Equal(DateTime.Now.Year.ToString(), vm.AnioTexto);
        Assert.Equal("Factura A", vm.TipoFactura);
        Assert.False(vm.Meses.Any(m => m.Seleccionado));
    }

    // TEST-056
    [Fact]
    public void PuedeProcesar_SinSelecciones_RetornaFalse()
    {
        Assert.False(CreateVm().ProcesarCommand.CanExecute(null));
    }

    // TEST-057
    [Fact]
    public void PuedeProcesar_TodoCompleto_RetornaTrue()
    {
        var vm = CreateVm();
        SetearCompleto(vm, [1]);

        Assert.True(vm.ProcesarCommand.CanExecute(null));
    }

    // TEST-058
    [Fact]
    public void PuedeProcesar_AuspicianteNoEnCache_RetornaFalse()
    {
        var vm = CreateVm();
        vm.AuspicianteSeleccionado = "NoExiste";
        vm.ProgramaSeleccionado = "Prog1";
        vm.PeriodistaSeleccionado = "Per1";
        vm.MontoTexto = "1.000";
        vm.Meses[0].Seleccionado = true;

        Assert.False(vm.ProcesarCommand.CanExecute(null));
    }

    [Fact]
    public void PuedeProcesar_SinMesSeleccionado_RetornaFalse()
    {
        var vm = CreateVm();
        vm.AuspicianteSeleccionado = "Ausp1";
        vm.ProgramaSeleccionado = "Prog1";
        vm.PeriodistaSeleccionado = "Per1";
        vm.MontoTexto = "1.000";
        // ningún mes seleccionado

        Assert.False(vm.ProcesarCommand.CanExecute(null));
    }

    // TEST-059
    [Fact]
    public void Procesar_LlamaInsertManyConRowsCorrectas()
    {
        var vm = CreateVm();
        vm.AnioTexto = "2025";
        SetearCompleto(vm, [1, 3]); // meses 1 y 3

        List<FacturacionRow>? captured = null;
        _repoMock
            .Setup(r => r.InsertMany(It.IsAny<IEnumerable<FacturacionRow>>()))
            .Callback<IEnumerable<FacturacionRow>>(rows => captured = rows.ToList());

        vm.ProcesarCommand.Execute(null);

        Assert.NotNull(captured);
        Assert.Equal(2, captured!.Count);
        Assert.Contains(captured, r => r.MesAnio == "01/2025");
        Assert.Contains(captured, r => r.MesAnio == "03/2025");
        Assert.All(captured, r => Assert.Equal("Ausp1", r.Auspiciante));
        Assert.All(captured, r => Assert.Equal("Prog1", r.Programa));
        Assert.All(captured, r => Assert.Equal("Per1", r.Periodista));
        Assert.All(captured, r => Assert.Equal("1.000", r.Monto));
    }

    // TEST-060
    [Fact]
    public void Procesar_LimpiaFormularioDespues()
    {
        var vm = CreateVm();
        SetearCompleto(vm, [1]);

        vm.ProcesarCommand.Execute(null);

        Assert.Null(vm.AuspicianteSeleccionado);
        Assert.Null(vm.ProgramaSeleccionado);
        Assert.Null(vm.PeriodistaSeleccionado);
        Assert.Null(vm.MontoTexto);
        Assert.False(vm.Meses.Any(m => m.Seleccionado));
    }

    // TEST-061
    [Fact]
    public void ToggleMes_AgregaMesAlHashSet()
    {
        var vm = CreateVm();
        vm.ToggleMes("01");
        Assert.Contains("01", vm.MesesSeleccionados);
    }

    // TEST-062
    [Fact]
    public void ToggleMes_MismoMesDosVeces_LoQuitaDelHashSet()
    {
        var vm = CreateVm();
        vm.ToggleMes("01");
        vm.ToggleMes("01");
        Assert.DoesNotContain("01", vm.MesesSeleccionados);
    }
}
