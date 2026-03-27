
using FacturacionA4V.Domain;

namespace FacturacionA4V.UI.ViewModel;

public sealed class MainViewModel
{
    public int AuspiciantesCount { get; }
    public int ProgramasCount { get; }
    public int PeriodistasCount { get; }

    public MainViewModel(DatosInicioCache cache)
    {
        AuspiciantesCount = cache.Auspiciantes.Count;
        ProgramasCount = cache.Programas.Count;
        PeriodistasCount = cache.Periodistas.Count;
    }
}
