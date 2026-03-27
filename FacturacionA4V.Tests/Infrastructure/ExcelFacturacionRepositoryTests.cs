using ClosedXML.Excel;
using FacturacionA4V.Domain;
using FacturacionA4V.Infrastructure;

namespace FacturacionA4V.Tests.Infrastructure;

public class ExcelFacturacionRepositoryTests : IDisposable
{
    private readonly string _tempDir;
    private readonly Mock<IDriveFileService> _driveMock;

    public ExcelFacturacionRepositoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);

        _driveMock = new Mock<IDriveFileService>();
        _driveMock.Setup(d => d.DownloadFile()).ThrowsAsync(new Exception("no drive en tests"));
        _driveMock.Setup(d => d.Upload(It.IsAny<Stream>())).Returns(Task.CompletedTask);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private ExcelFacturacionRepository CreateRepo()
        => new ExcelFacturacionRepository(_tempDir, _driveMock.Object);

    private void CreateExcelWithRows(params FacturacionRow[] rows)
    {
        var path = Path.Combine(_tempDir, "Facturacion.xlsx");
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Facturacion");

        ws.Cell(1, 1).Value = "ID";
        ws.Cell(1, 2).Value = "Auspiciantes";
        ws.Cell(1, 3).Value = "Programa";
        ws.Cell(1, 4).Value = "Periodista";
        ws.Cell(1, 5).Value = "Monto";
        ws.Cell(1, 6).Value = "TipoFactura";
        ws.Cell(1, 7).Value = "MesAnio";
        ws.Cell(1, 8).Value = "NroFactura";
        ws.Cell(1, 9).Value = "FechaFactura";
        ws.Cell(1, 10).Value = "Nota";
        ws.Cell(1, 11).Value = "FechaPago";

        int rowIdx = 2;
        foreach (var r in rows)
        {
            ws.Cell(rowIdx, 1).Value = r.Id.ToString();
            ws.Cell(rowIdx, 2).Value = r.Auspiciante;
            ws.Cell(rowIdx, 3).Value = r.Programa;
            ws.Cell(rowIdx, 4).Value = r.Periodista;
            ws.Cell(rowIdx, 5).Value = r.Monto;
            ws.Cell(rowIdx, 6).Value = r.TipoFactura;
            ws.Cell(rowIdx, 7).Value = r.MesAnio;
            ws.Cell(rowIdx, 8).Value = r.NroFactura ?? "";
            ws.Cell(rowIdx, 9).Value = r.FechaFactura ?? "";
            ws.Cell(rowIdx, 10).Value = r.Nota ?? "";
            ws.Cell(rowIdx, 11).Value = r.FechaPago ?? "";
            rowIdx++;
        }

        wb.SaveAs(path);
    }

    // ==================== TEST-001 a TEST-007: CalcularEstado ====================

    [Fact]
    public void CalcularEstado_SinNroFactura_RetornaSinFactura()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado("", null, null);
        Assert.Equal(EstadoFactura.SinFactura, estado);
    }

    [Fact]
    public void CalcularEstado_ConNroFacturaYFechaPago_RetornaPagada()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado(
            "F-001", DateTime.Today.AddDays(-10), DateTime.Today);
        Assert.Equal(EstadoFactura.Pagada, estado);
    }

    [Fact]
    public void CalcularEstado_ConNroFacturaSinFechaFactura_RetornaFacturadaMenor30()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado("F-001", null, null);
        Assert.Equal(EstadoFactura.FacturadaMenor30, estado);
    }

    [Fact]
    public void CalcularEstado_FechaFacturaMenos20Dias_RetornaFacturadaMenor30()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado(
            "F-001", DateTime.Today.AddDays(-20), null);
        Assert.Equal(EstadoFactura.FacturadaMenor30, estado);
    }

    [Fact]
    public void CalcularEstado_FechaFacturaMenos45Dias_RetornaFacturada30()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado(
            "F-001", DateTime.Today.AddDays(-45), null);
        Assert.Equal(EstadoFactura.Facturada30, estado);
    }

    [Fact]
    public void CalcularEstado_FechaFacturaMenos75Dias_RetornaFacturada60()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado(
            "F-001", DateTime.Today.AddDays(-75), null);
        Assert.Equal(EstadoFactura.Facturada60, estado);
    }

    [Fact]
    public void CalcularEstado_FechaFacturaMenos100Dias_RetornaFacturada90()
    {
        var estado = ExcelFacturacionRepository.CalcularEstado(
            "F-001", DateTime.Today.AddDays(-100), null);
        Assert.Equal(EstadoFactura.Facturada90, estado);
    }

    // ==================== TEST-008 a TEST-010: TryParseMontoAR ====================

    [Fact]
    public void TryParseMontoAR_MontoValido_RetornaDecimal()
    {
        var result = ExcelFacturacionRepository.TryParseMontoAR("1.500,75");
        Assert.Equal(1500.75m, result);
    }

    [Fact]
    public void TryParseMontoAR_CadenaVacia_RetornaNull()
    {
        var result = ExcelFacturacionRepository.TryParseMontoAR("");
        Assert.Null(result);
    }

    [Fact]
    public void TryParseMontoAR_CadenaInvalida_RetornaNull()
    {
        var result = ExcelFacturacionRepository.TryParseMontoAR("abc");
        Assert.Null(result);
    }

    // ==================== TEST-011 a TEST-014: TryParseFecha ====================

    [Fact]
    public void TryParseFecha_CeldaDateTime_RetornaFecha()
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("T");
        ws.Cell(1, 1).Value = new DateTime(2025, 3, 15);

        var result = ExcelFacturacionRepository.TryParseFecha(ws.Cell(1, 1));

        Assert.NotNull(result);
        Assert.Equal(new DateTime(2025, 3, 15).Date, result!.Value.Date);
    }

    [Fact]
    public void TryParseFecha_CeldaNumeroSerial_RetornaFecha()
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("T");
        // Forzar tipo Number con SetValue del OADate como double
        var oaDate = new DateTime(2025, 3, 15).ToOADate();
        ws.Cell(1, 1).SetValue(oaDate);

        var result = ExcelFacturacionRepository.TryParseFecha(ws.Cell(1, 1));

        Assert.NotNull(result);
        Assert.Equal(new DateTime(2025, 3, 15).Date, result!.Value.Date);
    }

    [Fact]
    public void TryParseFecha_CeldaStringFecha_RetornaFecha()
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("T");
        ws.Cell(1, 1).SetValue("2025-03-15");

        var result = ExcelFacturacionRepository.TryParseFecha(ws.Cell(1, 1));

        Assert.NotNull(result);
        Assert.Equal(new DateTime(2025, 3, 15).Date, result!.Value.Date);
    }

    [Fact]
    public void TryParseFecha_CeldaStringInvalido_RetornaNull()
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("T");
        ws.Cell(1, 1).SetValue("no-es-fecha");

        var result = ExcelFacturacionRepository.TryParseFecha(ws.Cell(1, 1));

        Assert.Null(result);
    }

    // ==================== TEST-015 a TEST-017: InsertMany ====================

    [Fact]
    public void InsertMany_ArchivoNuevo_CreaHojaConHeaderYFila()
    {
        var repo = CreateRepo();
        var id = Guid.NewGuid();
        var row = new FacturacionRow
        {
            Id = id,
            Auspiciante = "AuspA",
            Programa = "ProgA",
            Periodista = "PerA",
            Monto = "1.000",
            TipoFactura = "Factura A",
            MesAnio = "03/2025"
        };

        repo.InsertMany([row]);

        var path = Path.Combine(_tempDir, "Facturacion.xlsx");
        Assert.True(File.Exists(path));

        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet("Facturacion");

        Assert.Equal("ID", ws.Cell(1, 1).GetString());
        Assert.Equal(id.ToString(), ws.Cell(2, 1).GetString());
        Assert.Equal("AuspA", ws.Cell(2, 2).GetString());
        Assert.Equal("ProgA", ws.Cell(2, 3).GetString());
        Assert.Equal("PerA", ws.Cell(2, 4).GetString());
        Assert.Equal("1.000", ws.Cell(2, 5).GetString());
        Assert.Equal("Factura A", ws.Cell(2, 6).GetString());
        Assert.Equal("03/2025", ws.Cell(2, 7).GetString());
    }

    [Fact]
    public void InsertMany_ArchivoExistente_AgregaFilaSinPisarDatos()
    {
        CreateExcelWithRows(new FacturacionRow
        {
            Id = Guid.NewGuid(), Auspiciante = "A1", Programa = "P1",
            Periodista = "Per1", Monto = "100", TipoFactura = "Factura A", MesAnio = "01/2025"
        });

        var repo = CreateRepo();
        repo.InsertMany([new FacturacionRow
        {
            Id = Guid.NewGuid(), Auspiciante = "A2", Programa = "P2",
            Periodista = "Per2", Monto = "200", TipoFactura = "Factura A", MesAnio = "02/2025"
        }]);

        using var wb = new XLWorkbook(Path.Combine(_tempDir, "Facturacion.xlsx"));
        var ws = wb.Worksheet("Facturacion");
        Assert.Equal(3, ws.RowsUsed().Count()); // header + 2 rows
    }

    [Fact]
    public void InsertMany_VariasRows_InsertaTodasEnOrden()
    {
        var repo = CreateRepo();
        var rows = Enumerable.Range(1, 3).Select(i => new FacturacionRow
        {
            Id = Guid.NewGuid(), Auspiciante = $"A{i}", Programa = $"P{i}",
            Periodista = $"Per{i}", Monto = $"{i * 100}", TipoFactura = "Factura A",
            MesAnio = $"0{i}/2025"
        }).ToList();

        repo.InsertMany(rows);

        using var wb = new XLWorkbook(Path.Combine(_tempDir, "Facturacion.xlsx"));
        var ws = wb.Worksheet("Facturacion");

        for (int i = 0; i < 3; i++)
            Assert.Equal(rows[i].Auspiciante, ws.Cell(i + 2, 2).GetString());
    }

    // ==================== TEST-018 a TEST-020: ReadAll ====================

    [Fact]
    public void ReadAll_ArchivoInexistente_RetornaListaVacia()
    {
        var items = CreateRepo().ReadAll();
        Assert.Empty(items);
    }

    [Fact]
    public void ReadAll_ConDatos_MapeaTodosLosCamposCorrectamente()
    {
        var id = Guid.NewGuid();
        CreateExcelWithRows(new FacturacionRow
        {
            Id = id,
            Auspiciante = "AuspA",
            Programa = "ProgA",
            Periodista = "PerA",
            Monto = "1.500",
            TipoFactura = "Factura A",
            MesAnio = "03/2025",
            Nota = "una nota"
        });

        var items = CreateRepo().ReadAll();

        Assert.Single(items);
        var item = items[0];
        Assert.Equal(id, item.Id);
        Assert.Equal("AuspA", item.Auspiciante);
        Assert.Equal("ProgA", item.Programa);
        Assert.Equal("PerA", item.Periodista);
        Assert.Equal("1.500", item.MontoTexto);
        Assert.Equal("03/2025", item.MesAnio);
        Assert.Equal("una nota", item.Nota);
        Assert.Equal(EstadoFactura.SinFactura, item.Estado);
    }

    [Fact]
    public void ReadAll_CalculaEstadoCorrectamente()
    {
        var idSin = Guid.NewGuid();
        var idPagada = Guid.NewGuid();

        CreateExcelWithRows(
            new FacturacionRow
            {
                Id = idSin, Auspiciante = "A", Programa = "P", Periodista = "Per",
                Monto = "100", TipoFactura = "A", MesAnio = "01/2025"
            },
            new FacturacionRow
            {
                Id = idPagada, Auspiciante = "A", Programa = "P", Periodista = "Per",
                Monto = "200", TipoFactura = "A", MesAnio = "01/2025",
                NroFactura = "F-001",
                FechaFactura = DateTime.Today.AddDays(-10).ToString("yyyy-MM-dd"),
                FechaPago = DateTime.Today.ToString("yyyy-MM-dd")
            }
        );

        var items = CreateRepo().ReadAll();

        Assert.Equal(EstadoFactura.SinFactura, items.First(i => i.Id == idSin).Estado);
        Assert.Equal(EstadoFactura.Pagada, items.First(i => i.Id == idPagada).Estado);
    }

    // ==================== TEST-021 a TEST-022: UpdateFactura ====================

    [Fact]
    public void UpdateFactura_ActualizaCamposCorrectosEnExcel()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        CreateExcelWithRows(
            new FacturacionRow { Id = id1, Auspiciante = "A", Programa = "P", Periodista = "Per", Monto = "100", TipoFactura = "A", MesAnio = "01/2025" },
            new FacturacionRow { Id = id2, Auspiciante = "B", Programa = "P", Periodista = "Per", Monto = "200", TipoFactura = "A", MesAnio = "02/2025" }
        );

        var updateDate = new DateTime(2025, 3, 15);
        CreateRepo().UpdateFactura([new FacturaUpdate
        {
            Id = id2,
            NroFactura = "F-002",
            FechaFactura = updateDate,
            Nota = "nota actualizada"
        }]);

        using var wb = new XLWorkbook(Path.Combine(_tempDir, "Facturacion.xlsx"));
        var ws = wb.Worksheet("Facturacion");

        // Fila 2 (id1) no cambió en NroFactura
        Assert.Equal("", ws.Cell(2, 8).GetString());

        // Fila 3 (id2) actualizada
        Assert.Equal("F-002", ws.Cell(3, 8).GetString());
        Assert.Equal("nota actualizada", ws.Cell(3, 10).GetString());
    }

    [Fact]
    public void UpdateFactura_ArchivoInexistente_LanzaInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CreateRepo().UpdateFactura([new FacturaUpdate
            {
                Id = Guid.NewGuid(),
                NroFactura = "F-001",
                FechaFactura = DateTime.Today
            }]));
    }

    // ==================== TEST-023 a TEST-024: UpdatePago ====================

    [Fact]
    public void UpdatePago_ActualizaFechaPagoEnColumna11()
    {
        var id = Guid.NewGuid();
        CreateExcelWithRows(new FacturacionRow
        {
            Id = id, Auspiciante = "A", Programa = "P", Periodista = "Per",
            Monto = "100", TipoFactura = "A", MesAnio = "01/2025", NroFactura = "F-001"
        });

        var fechaPago = new DateTime(2025, 4, 1);
        CreateRepo().UpdatePago([new PagoUpdate { Id = id, FechaPago = fechaPago }]);

        using var wb = new XLWorkbook(Path.Combine(_tempDir, "Facturacion.xlsx"));
        var ws = wb.Worksheet("Facturacion");
        var cell = ws.Cell(2, 11);

        // Fecha debe estar guardada (como DateTime o como string con valor)
        Assert.True(
            cell.DataType == XLDataType.DateTime ||
            !string.IsNullOrWhiteSpace(cell.GetString()));
    }

    [Fact]
    public void UpdatePago_ArchivoInexistente_LanzaInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CreateRepo().UpdatePago([new PagoUpdate
            {
                Id = Guid.NewGuid(),
                FechaPago = DateTime.Today
            }]));
    }
}
