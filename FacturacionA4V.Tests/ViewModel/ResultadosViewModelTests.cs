using FacturacionA4V.Domain;
using FacturacionA4V.UI.ViewModel;

namespace FacturacionA4V.Tests.ViewModel;

// TEST-063 a TEST-079 + TEST-085 (regresión BUG-001)
public class ResultadosViewModelTests
{
    private readonly Mock<IFacturacionRepository> _repoMock = new();

    private static FacturacionItem MakeItem(
        string mesAnio,
        string auspiciante = "A",
        string programa = "P",
        string periodista = "Per",
        string? nroFactura = null,
        string fechaPago = "",
        decimal? monto = 1000m,
        EstadoFactura estado = EstadoFactura.SinFactura)
    {
        return new FacturacionItem
        {
            Id = Guid.NewGuid(),
            Auspiciante = auspiciante,
            Programa = programa,
            Periodista = periodista,
            MesAnio = mesAnio,
            NroFactura = nroFactura,
            FechaPago = fechaPago,
            MontoParsed = monto,
            Estado = estado
        };
    }

    private ResultadosViewModel CreateVm(IEnumerable<FacturacionItem>? items = null)
    {
        _repoMock
            .Setup(r => r.ReadAll())
            .Returns((items ?? []).ToList());
        return new ResultadosViewModel(_repoMock.Object);
    }

    // TEST-063
    [Fact]
    public void Cargar_PopulaFiltrosConValoresUnicos()
    {
        var vm = CreateVm([
            MakeItem("01/2024", auspiciante: "A", programa: "P1", periodista: "Per1"),
            MakeItem("01/2025", auspiciante: "B", programa: "P2", periodista: "Per2"),
            MakeItem("02/2025", auspiciante: "A", programa: "P1", periodista: "Per1") // duplicados
        ]);

        Assert.Equal(2, vm.AuspiciantesDisponibles.Count);
        Assert.Equal(2, vm.ProgramasDisponibles.Count);
        Assert.Equal(2, vm.PeriodistasDisponibles.Count);
        Assert.Equal(2, vm.AniosDisponibles.Count);
        Assert.Contains(2024, vm.AniosDisponibles);
        Assert.Contains(2025, vm.AniosDisponibles);
    }

    // TEST-064
    [Fact]
    public void AplicarFiltros_SinFiltros_MuestraTodos()
    {
        var vm = CreateVm([
            MakeItem("01/2025"),
            MakeItem("02/2025"),
            MakeItem("03/2025")
        ]);

        Assert.Equal(3, vm.Filtrados.Count);
    }

    // TEST-065
    [Fact]
    public void AplicarFiltros_PorAuspiciante_MuestraSoloMatching()
    {
        var vm = CreateVm([
            MakeItem("01/2025", auspiciante: "A"),
            MakeItem("01/2025", auspiciante: "A"),
            MakeItem("01/2025", auspiciante: "B")
        ]);

        vm.AuspicianteSeleccionado = "A";

        Assert.Equal(2, vm.Filtrados.Count);
        Assert.All(vm.Filtrados, i => Assert.Equal("A", i.Auspiciante));
    }

    // TEST-066
    [Fact]
    public void AplicarFiltros_PorAnio_MuestraSoloEseAnio()
    {
        var vm = CreateVm([
            MakeItem("01/2024"),
            MakeItem("06/2024"),
            MakeItem("01/2025")
        ]);

        vm.AnioSeleccionado = 2025;

        Assert.Equal(1, vm.Filtrados.Count);
        Assert.Equal("01/2025", vm.Filtrados[0].MesAnio);
    }

    // TEST-067
    [Fact]
    public void AplicarFiltros_PorPrograma_FiltraCorrectamente()
    {
        var vm = CreateVm([
            MakeItem("01/2025", programa: "Prog1"),
            MakeItem("01/2025", programa: "Prog2")
        ]);

        vm.ProgramaSeleccionado = "Prog1";

        Assert.Single(vm.Filtrados);
        Assert.Equal("Prog1", vm.Filtrados[0].Programa);
    }

    // TEST-068
    [Fact]
    public void AplicarFiltros_PorPeriodista_FiltraCorrectamente()
    {
        var vm = CreateVm([
            MakeItem("01/2025", periodista: "Per1"),
            MakeItem("01/2025", periodista: "Per2")
        ]);

        vm.PeriodistaSeleccionado = "Per1";

        Assert.Single(vm.Filtrados);
        Assert.Equal("Per1", vm.Filtrados[0].Periodista);
    }

    // TEST-069
    [Fact]
    public void AplicarFiltros_MultiplesFiltros_InterseccionCorrecta()
    {
        var vm = CreateVm([
            MakeItem("01/2024", auspiciante: "A"),
            MakeItem("01/2025", auspiciante: "A"),
            MakeItem("01/2025", auspiciante: "B")
        ]);

        vm.AuspicianteSeleccionado = "A";
        vm.AnioSeleccionado = 2025;

        Assert.Single(vm.Filtrados);
        Assert.Equal("01/2025", vm.Filtrados[0].MesAnio);
        Assert.Equal("A", vm.Filtrados[0].Auspiciante);
    }

    // TEST-070
    [Fact]
    public void RecalcularTotales_SumaCorrectamente()
    {
        var vm = CreateVm([
            MakeItem("01/2025", monto: 1000m, fechaPago: ""),              // facturado, no cobrado
            MakeItem("01/2025", monto: 2000m, fechaPago: "15/03/2025"),    // cobrado
            MakeItem("01/2025", monto: 500m, fechaPago: "")                // facturado, no cobrado
        ]);

        Assert.Equal(3500m, vm.TotalFacturado);
        Assert.Equal(2000m, vm.TotalCobrado);
        Assert.Equal(1500m, vm.TotalPendiente);
    }

    // TEST-071
    [Fact]
    public void RecalcularTotales_ItemSinMontoParsed_NoSuma()
    {
        var vm = CreateVm([MakeItem("01/2025", monto: null)]);

        Assert.Equal(0m, vm.TotalFacturado);
        Assert.Equal(0m, vm.TotalCobrado);
    }

    // TEST-072
    [Fact]
    public void PuedeAgregarFactura_SinSeleccionados_RetornaFalse()
    {
        var vm = CreateVm([MakeItem("01/2025")]);

        Assert.False(vm.AgregarFacturaCommand.CanExecute(null));
    }

    // TEST-073
    [Fact]
    public void PuedeAgregarFactura_SeleccionadoSinFactura_RetornaTrue()
    {
        var vm = CreateVm([MakeItem("01/2025", nroFactura: null)]);
        vm.Filtrados[0].IsSelected = true;

        Assert.True(vm.AgregarFacturaCommand.CanExecute(null));
    }

    // TEST-074
    [Fact]
    public void PuedeAgregarFactura_SeleccionadoConFactura_RetornaFalse()
    {
        var vm = CreateVm([MakeItem("01/2025", nroFactura: "F-001")]);
        vm.Filtrados[0].IsSelected = true;

        Assert.False(vm.AgregarFacturaCommand.CanExecute(null));
    }

    // TEST-075
    [Fact]
    public void PuedeAgregarPago_SinSeleccionados_RetornaFalse()
    {
        var vm = CreateVm([MakeItem("01/2025", nroFactura: "F-001")]);

        Assert.False(vm.AgregarPagoCommand.CanExecute(null));
    }

    // TEST-076
    [Fact]
    public void PuedeAgregarPago_SeleccionadoConFacturaSinPago_RetornaTrue()
    {
        var vm = CreateVm([MakeItem("01/2025", nroFactura: "F-001", fechaPago: "")]);
        vm.Filtrados[0].IsSelected = true;

        Assert.True(vm.AgregarPagoCommand.CanExecute(null));
    }

    // TEST-077
    [Fact]
    public void PuedeAgregarPago_SeleccionadoConPago_RetornaFalse()
    {
        var vm = CreateVm([MakeItem("01/2025", nroFactura: "F-001", fechaPago: "15/03/2025")]);
        vm.Filtrados[0].IsSelected = true;

        Assert.False(vm.AgregarPagoCommand.CanExecute(null));
    }

    // TEST-078
    [Fact]
    public void LimpiarFiltros_ReseteatodoYMuestraTodos()
    {
        var vm = CreateVm([
            MakeItem("01/2024", auspiciante: "A"),
            MakeItem("01/2025", auspiciante: "B")
        ]);

        vm.AuspicianteSeleccionado = "A";
        Assert.Equal(1, vm.Filtrados.Count);

        vm.LimpiarFiltrosCommand.Execute(null);

        Assert.Null(vm.AuspicianteSeleccionado);
        Assert.Null(vm.AnioSeleccionado);
        Assert.Null(vm.ProgramaSeleccionado);
        Assert.Null(vm.PeriodistaSeleccionado);
        Assert.Equal(2, vm.Filtrados.Count);
    }

    // TEST-079
    [Fact]
    public void Seleccion_ActualizaCanExecuteDeAmbosComandos()
    {
        var vm = CreateVm([MakeItem("01/2025", nroFactura: null)]);

        bool facturaChanged = false;
        bool pagoChanged = false;
        vm.AgregarFacturaCommand.CanExecuteChanged += (_, _) => facturaChanged = true;
        vm.AgregarPagoCommand.CanExecuteChanged += (_, _) => pagoChanged = true;

        vm.Filtrados[0].IsSelected = true;

        Assert.True(facturaChanged);
        Assert.True(pagoChanged);
    }

    // TEST-085: Regresión BUG-001 — Cargar() debe respetar filtros activos
    [Fact]
    public void Cargar_MantieneElFiltroActivoTrasRecargar_BUG001()
    {
        var items = new List<FacturacionItem>
        {
            MakeItem("01/2024"),
            MakeItem("06/2024"),
            MakeItem("01/2025")
        };

        _repoMock.Setup(r => r.ReadAll()).Returns(items);
        var vm = new ResultadosViewModel(_repoMock.Object);

        vm.AnioSeleccionado = 2025;
        Assert.Equal(1, vm.Filtrados.Count);

        vm.Cargar(); // no debe resetear el filtro

        Assert.Equal(1, vm.Filtrados.Count);
        Assert.Equal("01/2025", vm.Filtrados[0].MesAnio);
    }

    // TEST-086
    [Fact]
    public void AplicarFiltros_PorMes_FiltraCorrectamente()
    {
        var items = new List<FacturacionItem>
        {
            MakeItem("03/2025"),
            MakeItem("06/2025"),
            MakeItem("03/2024")
        };

        var vm = CreateVm(items);

        vm.MesSeleccionado = vm.MesesDisponibles.First(m => m.Numero == 3);

        Assert.Equal(2, vm.Filtrados.Count);
        Assert.All(vm.Filtrados, x => Assert.StartsWith("03/", x.MesAnio));
    }

    // TEST-087
    [Fact]
    public void LimpiarFiltros_ReseteaMesTambien()
    {
        var items = new List<FacturacionItem>
        {
            MakeItem("03/2025"),
            MakeItem("06/2025")
        };

        var vm = CreateVm(items);
        vm.MesSeleccionado = vm.MesesDisponibles.First(m => m.Numero == 3);
        Assert.Equal(1, vm.Filtrados.Count);

        vm.LimpiarFiltrosCommand.Execute(null);

        Assert.Null(vm.MesSeleccionado);
        Assert.Equal(2, vm.Filtrados.Count);
    }
}
