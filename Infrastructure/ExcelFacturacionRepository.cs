using ClosedXML.Excel;
using FacturacionA4V.Domain;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace FacturacionA4V.Infrastructure;

public sealed class ExcelFacturacionRepository : IFacturacionRepository
{
    private readonly string _filePath;
    private GoogleDriveFileService _drive;

    public ExcelFacturacionRepository(string basePath)
    {
        _filePath = Path.Combine(basePath, "Facturacion.xlsx");

        var driveSettings = App.Configuration
            .GetSection("GoogleDrive")
            .Get<GoogleDriveSettings>();

        _drive = new GoogleDriveFileService(
            driveSettings.ServiceAccountPath,
            driveSettings.FileId);
    }

    private void UploadToDrive()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var fs = System.IO.File.OpenRead(_filePath);
                await _drive.Upload(fs);
            }
            catch (Exception ex)
            {
                // Loguear mínimo
                throw new("No se pudo guardar, reinicia la aplicacion y volve a intentarlo, a pesar de que lo vea en su copia local");
            }
        });
    }

    public void InsertMany(IEnumerable<FacturacionRow> rows)
    {
        using var wb = System.IO.File.Exists(_filePath)
            ? new XLWorkbook(_filePath)
            : new XLWorkbook();

        var ws = wb.Worksheets.FirstOrDefault(w => w.Name == "Facturacion")
                 ?? wb.AddWorksheet("Facturacion");

        if (ws.LastRowUsed() == null)
            WriteHeader(ws);

        var rowIdx = ws.LastRowUsed()!.RowNumber() + 1;

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

        wb.SaveAs(_filePath);
        UploadToDrive();
    }

    private static void WriteHeader(IXLWorksheet ws)
    {
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
    }

    public IReadOnlyList<FacturacionItem> ReadAll()
    {
        SaveLocal();

        if (!System.IO.File.Exists(_filePath))
            return Array.Empty<FacturacionItem>();

        using var wb = new XLWorkbook(_filePath);
        var ws = wb.Worksheet("Facturacion");

        var items = new List<FacturacionItem>();

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var id = Guid.Parse(row.Cell(1).GetString());

            var montoTxt = row.Cell(5).GetString();
            var montoParsed = TryParseMontoAR(montoTxt);

            var fechaFactura = TryParseFecha(row.Cell(9));
            var fechaPago = TryParseFecha(row.Cell(11));

            items.Add(new FacturacionItem
            {
                Id = id,
                Auspiciante = row.Cell(2).GetString(),
                Programa = row.Cell(3).GetString(),
                Periodista = row.Cell(4).GetString(),
                MontoTexto = montoTxt,
                MesAnio = row.Cell(7).GetString(),
                NroFactura = row.Cell(8).GetString(),
                FechaFactura = fechaFactura.HasValue ? fechaFactura.Value.ToShortDateString() : "",
                FechaPago = fechaPago.HasValue ? fechaPago.Value.ToShortDateString() : "",
                MontoParsed = montoParsed,
                Nota = row.Cell(10).GetString(),
                Estado = CalcularEstado(row.Cell(8).GetString(), fechaFactura, fechaPago)
            });
        }

        return items;
    }

    private async Task SaveLocal()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                var stream = await _drive.DownloadFile();

                var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Facturacion.xlsx");

                System.IO.File.WriteAllBytes(localPath, stream.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando desde Drive: {ex.Message}");
            }
        });
      
    }

    private static DateTime? TryParseFecha(IXLCell cell)
    {
        // 1️⃣ Excel ya lo tiene como DateTime
        if (cell.DataType == XLDataType.DateTime)
            return cell.GetDateTime();

        // 2️⃣ Excel lo tiene como número serial
        if (cell.DataType == XLDataType.Number)
            return DateTime.FromOADate(cell.GetDouble());

        // 3️⃣ Fallback: string (último recurso)
        var raw = cell.GetString();

        if (DateTime.TryParse(
            raw,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var dt))
            return dt;

        return null;
    }

    private static decimal? TryParseMontoAR(string raw)
    {
        if (decimal.TryParse(
            raw,
            NumberStyles.Number,
            new CultureInfo("es-AR"),
            out var value))
            return value;

        return null;
    }

    private static EstadoFactura CalcularEstado(
        string nroFactura,
        DateTime? fechaFactura,
        DateTime? fechaPago)
    {
        if (string.IsNullOrWhiteSpace(nroFactura))
            return EstadoFactura.SinFactura;

        if (fechaPago.HasValue)
            return EstadoFactura.Pagada;

        if (!fechaFactura.HasValue)
            return EstadoFactura.FacturadaMenor30;

        var dias = (DateTime.Today - fechaFactura.Value.Date).Days;

        return dias switch
        {
            < 30 => EstadoFactura.FacturadaMenor30,
            < 60 => EstadoFactura.Facturada30,
            < 90 => EstadoFactura.Facturada60,
            >= 90 => EstadoFactura.Facturada90
        };
    }

    public void UpdateFactura(IEnumerable<FacturaUpdate> updates)
    {
        if (!System.IO.File.Exists(_filePath))
            throw new InvalidOperationException("No existe Facturacion.xlsx");

        using var wb = new XLWorkbook(_filePath);
        var ws = wb.Worksheet("Facturacion");

        var byId = updates.ToDictionary(u => u.Id);

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var id = Guid.Parse(row.Cell(1).GetString());
            if (!byId.TryGetValue(id, out var upd))
                continue;

            row.Cell(8).Value = upd.NroFactura;
            row.Cell(9).Value = upd.FechaFactura;   // DateTime real
            row.Cell(10).Value = upd.Nota ?? "";
            row.Cell(11).Value = "";                 // FechaPago vacío
        }

        wb.Save();
        UploadToDrive();
    }

    public void UpdatePago(IEnumerable<PagoUpdate> updates)
    {
        if (!System.IO.File.Exists(_filePath))
            throw new InvalidOperationException("No existe Facturacion.xlsx");

        using var wb = new XLWorkbook(_filePath);
        var ws = wb.Worksheet("Facturacion");

        var byId = updates.ToDictionary(x => x.Id);

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var id = Guid.Parse(row.Cell(1).GetString());
            if (!byId.TryGetValue(id, out var upd))
                continue;

            row.Cell(11).Value = upd.FechaPago; // FechaPago
        }

        wb.Save();
        UploadToDrive();
    }

}
