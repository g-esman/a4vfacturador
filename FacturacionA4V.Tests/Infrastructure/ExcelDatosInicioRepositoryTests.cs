using ClosedXML.Excel;
using FacturacionA4V.Infrastructure;

namespace FacturacionA4V.Tests.Infrastructure;

public class ExcelDatosInicioRepositoryTests : IDisposable
{
    private readonly string _tempDir;

    public ExcelDatosInicioRepositoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private void CreateDatosInicioExcel(
        string[]? auspiciantes = null,
        string[]? programas = null,
        string[]? periodistas = null,
        bool omitirPeriodistas = false)
    {
        var path = Path.Combine(_tempDir, "Datos inicio.xlsx");
        using var wb = new XLWorkbook();

        var wsAusp = wb.AddWorksheet("Auspiciantes");
        foreach (var (v, i) in (auspiciantes ?? []).Select((v, i) => (v, i)))
            wsAusp.Cell(i + 1, 1).Value = v;

        var wsProg = wb.AddWorksheet("Programas");
        foreach (var (v, i) in (programas ?? []).Select((v, i) => (v, i)))
            wsProg.Cell(i + 1, 1).Value = v;

        if (!omitirPeriodistas)
        {
            var wsPer = wb.AddWorksheet("Periodistas");
            foreach (var (v, i) in (periodistas ?? []).Select((v, i) => (v, i)))
                wsPer.Cell(i + 1, 1).Value = v;
        }

        wb.SaveAs(path);
    }

    // TEST-025
    [Fact]
    public void Load_ArchivoInexistente_LanzaFileNotFoundException()
    {
        var repo = new ExcelDatosInicioRepository(_tempDir);
        Assert.Throws<FileNotFoundException>(() => repo.Load());
    }

    // TEST-026
    [Fact]
    public void Load_HojaFaltante_LanzaInvalidOperationException()
    {
        CreateDatosInicioExcel(
            auspiciantes: ["A"],
            programas: ["P"],
            omitirPeriodistas: true);

        var repo = new ExcelDatosInicioRepository(_tempDir);
        var ex = Assert.Throws<InvalidOperationException>(() => repo.Load());

        Assert.Contains("Periodistas", ex.Message);
    }

    // TEST-027
    [Fact]
    public void Load_RetornaDatosOrdenadosSinDuplicados()
    {
        CreateDatosInicioExcel(
            auspiciantes: ["Beta", "Alfa", "Beta", "Gamma"],
            programas: ["Prog1"],
            periodistas: ["Per1"]);

        var cache = new ExcelDatosInicioRepository(_tempDir).Load();

        Assert.Equal(new[] { "Alfa", "Beta", "Gamma" }, cache.Auspiciantes.ToArray());
    }

    // TEST-028
    [Fact]
    public void Load_HojaVacia_RetornaListaVacia()
    {
        CreateDatosInicioExcel(
            auspiciantes: [],
            programas: ["P"],
            periodistas: ["Per"]);

        var cache = new ExcelDatosInicioRepository(_tempDir).Load();

        Assert.Empty(cache.Auspiciantes);
    }
}
