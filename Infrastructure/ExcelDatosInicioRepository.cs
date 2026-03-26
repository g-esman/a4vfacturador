using ClosedXML.Excel;
using FacturacionA4V.Domain;
using System.IO;

namespace FacturacionA4V.Infrastructure;

public sealed class ExcelDatosInicioRepository : IDatosInicioRepository
{
    private readonly string _basePath;

    public ExcelDatosInicioRepository(string basePath)
    {
        _basePath = basePath;
    }

    public DatosInicioCache Load()
    {
        var filePath = Path.Combine(_basePath, "Datos inicio.xlsx");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"No se encontró el archivo: {filePath}");

        using var wb = new XLWorkbook(filePath);

        var ausp = ReadColumnA(wb, "Auspiciantes");
        var prog = ReadColumnA(wb, "Programas");
        var per = ReadColumnA(wb, "Periodistas");

        return new DatosInicioCache(ausp, prog, per);
    }

    private static IReadOnlyList<string> ReadColumnA(XLWorkbook wb, string sheetName)
    {
        var ws = wb.Worksheets.FirstOrDefault(s => s.Name.Equals(sheetName, StringComparison.OrdinalIgnoreCase));
        if (ws is null)
            throw new InvalidOperationException($"No existe la hoja '{sheetName}' en Datos inicio.xlsx");

        // Columna A, desde fila 1 hasta la última con datos
        var lastRow = ws.Column(1).LastCellUsed()?.Address.RowNumber ?? 0;
        if (lastRow == 0) return Array.Empty<string>();

        var list = new List<string>(capacity: lastRow);
        for (int r = 1; r <= lastRow; r++)
        {
            var raw = ws.Cell(r, 1).GetString()?.Trim();
            if (!string.IsNullOrWhiteSpace(raw))
                list.Add(raw);
        }

        // Normalizamos: sin duplicados, ordenado para autocomplete
        return list
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
