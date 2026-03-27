using FacturacionA4V.Domain;
using System.Globalization;

namespace FacturacionA4V.Tests.Domain;

// TEST-029 a TEST-031
public class MesItemTests
{
    [Fact]
    public void Constructor_Mes1_NumeroYNombreCorrectos()
    {
        var item = new MesItem(1);
        var esperado = new DateTime(2000, 1, 1).ToString("MMM", new CultureInfo("es-AR"));

        Assert.Equal(1, item.Numero);
        Assert.Equal(esperado, item.Nombre);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    public void Constructor_TodosLosMeses_NombresEnEspaniolArgentino(int mes)
    {
        var item = new MesItem(mes);
        var esperado = new DateTime(2000, mes, 1).ToString("MMM", new CultureInfo("es-AR"));

        Assert.Equal(mes, item.Numero);
        Assert.Equal(esperado, item.Nombre);
    }

    [Fact]
    public void Seleccionado_CambiaATrue_DisparaPropertyChanged()
    {
        var item = new MesItem(1);
        string? propertyName = null;
        item.PropertyChanged += (_, e) => propertyName = e.PropertyName;

        item.Seleccionado = true;

        Assert.Equal(nameof(MesItem.Seleccionado), propertyName);
    }
}

// TEST-032
public class DatosInicioCacheTests
{
    [Fact]
    public void Constructor_ExponePropiedadesCorrectas()
    {
        var ausp = new[] { "A1", "A2" };
        var prog = new[] { "P1" };
        var per = new[] { "Per1", "Per2", "Per3" };

        var cache = new DatosInicioCache(ausp, prog, per);

        Assert.Equal(ausp, cache.Auspiciantes);
        Assert.Equal(prog, cache.Programas);
        Assert.Equal(per, cache.Periodistas);
    }
}

// TEST-033 a TEST-034
public class FacturacionItemTests
{
    [Fact]
    public void IsSelected_CambiaATrue_DisparaPropertyChanged()
    {
        var item = new FacturacionItem { Id = Guid.NewGuid(), MesAnio = "01/2025" };
        string? propertyName = null;
        item.PropertyChanged += (_, e) => propertyName = e.PropertyName;

        item.IsSelected = true;

        Assert.Equal(nameof(FacturacionItem.IsSelected), propertyName);
    }

    [Fact]
    public void IsSelected_MismoValor_NoDisparaPropertyChanged()
    {
        var item = new FacturacionItem { Id = Guid.NewGuid(), MesAnio = "01/2025" };
        int count = 0;
        item.PropertyChanged += (_, _) => count++;

        item.IsSelected = false; // ya es false por defecto

        Assert.Equal(0, count);
    }
}
