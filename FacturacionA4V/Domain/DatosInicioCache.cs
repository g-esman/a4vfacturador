namespace FacturacionA4V.Domain
{
    public sealed class DatosInicioCache
    {
        public IReadOnlyList<string> Auspiciantes { get; }
        public IReadOnlyList<string> Programas { get; }
        public IReadOnlyList<string> Periodistas { get; }

        public DatosInicioCache(
            IReadOnlyList<string> auspiciantes,
            IReadOnlyList<string> programas,
            IReadOnlyList<string> periodistas)
        {
            Auspiciantes = auspiciantes;
            Programas = programas;
            Periodistas = periodistas;
        }
    }
}

