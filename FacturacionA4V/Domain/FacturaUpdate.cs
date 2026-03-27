public sealed class FacturaUpdate
{
    public Guid Id { get; init; }
    public string NroFactura { get; init; } = null!;
    public DateTime FechaFactura { get; init; }
    public string? Nota { get; init; }
}

