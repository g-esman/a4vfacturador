namespace FacturacionA4V.Domain;

public sealed class FacturacionRow
{
    public Guid Id { get; init; }

    public string Auspiciante { get; init; } = null!;
    public string Programa { get; init; } = null!;
    public string Periodista { get; init; } = null!;
    public string Monto { get; init; } = null!;
    public string TipoFactura { get; init; } = null!;
    public string MesAnio { get; init; } = null!;

    public string? NroFactura { get; set; }
    public string? FechaFactura { get; set; }
    public string? Nota { get; set; }
    public string? FechaPago { get; set; }
}
