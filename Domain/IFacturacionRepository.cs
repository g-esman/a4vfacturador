namespace FacturacionA4V.Domain
{
    public interface IFacturacionRepository
    {
        void InsertMany(IEnumerable<FacturacionRow> rows);
        IReadOnlyList<FacturacionItem> ReadAll();
        void UpdateFactura(IEnumerable<FacturaUpdate> updates);
        void UpdatePago(IEnumerable<PagoUpdate> updates);
    }
}
