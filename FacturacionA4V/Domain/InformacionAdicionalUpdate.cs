namespace FacturacionA4V.Domain;

public sealed class InformacionAdicionalUpdate
{
    public Guid Id { get; init; }
    public string InformacionAdicional { get; init; } = "";
}
