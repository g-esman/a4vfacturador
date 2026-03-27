using FacturacionA4V.UI.Helpers;

namespace FacturacionA4V.Tests.ViewModel;

public class MontoFormatterTests
{
    [Fact]
    public void FormatearMonto_CadenaVacia_RetornaNull()
    {
        Assert.Null(MontoFormatter.FormatearMonto(""));
    }

    [Fact]
    public void FormatearMonto_SoloEspacios_RetornaNull()
    {
        Assert.Null(MontoFormatter.FormatearMonto("   "));
    }

    [Fact]
    public void FormatearMonto_EnteroSimple_AgregaSeparadorMiles()
    {
        Assert.Equal("1.000", MontoFormatter.FormatearMonto("1000"));
    }

    [Fact]
    public void FormatearMonto_NumeroGrande_AgregaSeparadoresMiles()
    {
        Assert.Equal("1.000.000", MontoFormatter.FormatearMonto("1000000"));
    }

    [Fact]
    public void FormatearMonto_ConDecimales_FormateaCorrectamente()
    {
        Assert.Equal("1.000,75", MontoFormatter.FormatearMonto("1000,75"));
    }

    [Fact]
    public void FormatearMonto_MasDeDosDecimales_TruncaADos()
    {
        Assert.Equal("1.000,75", MontoFormatter.FormatearMonto("1000,756"));
    }

    [Fact]
    public void FormatearMonto_YaTienePuntosDeMiles_ReformateaCorrectamente()
    {
        // "1.000" → quita puntos → "1000" → formatea → "1.000"
        Assert.Equal("1.000", MontoFormatter.FormatearMonto("1.000"));
    }

    [Fact]
    public void FormatearMonto_Cero_RetornaCero()
    {
        Assert.Equal("0", MontoFormatter.FormatearMonto("0"));
    }

    [Fact]
    public void FormatearMonto_CeroConDecimal_RetornaConDecimal()
    {
        Assert.Equal("0,5", MontoFormatter.FormatearMonto("0,5"));
    }

    [Fact]
    public void FormatearMonto_TextoNoNumerico_RetornaNull()
    {
        Assert.Null(MontoFormatter.FormatearMonto("abc"));
    }

    [Fact]
    public void FormatearMonto_SoloComaDecimal_RetornaNull()
    {
        // ",50" → entero="" → long.TryParse falla → null
        Assert.Null(MontoFormatter.FormatearMonto(",50"));
    }
}
