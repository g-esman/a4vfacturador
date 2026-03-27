using System.Globalization;

namespace FacturacionA4V.UI.Helpers;

internal static class MontoFormatter
{
    /// <summary>
    /// Formatea un string de monto al estilo es-AR (puntos de miles, coma decimal, máx 2 decimales).
    /// Retorna null si el input está vacío o no es parseable como entero.
    /// </summary>
    internal static string? FormatearMonto(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        // Quitar puntos (separadores de miles existentes)
        raw = raw.Replace(".", "");

        // Separar parte entera y decimal
        var parts = raw.Split(',');
        var entero = parts[0];
        var decimalPart = parts.Length > 1 ? parts[1] : "";

        // Limitar decimales a 2 dígitos
        if (decimalPart.Length > 2)
            decimalPart = decimalPart.Substring(0, 2);

        if (!long.TryParse(entero, out var number))
            return null;

        var formatted = number.ToString("N0", new CultureInfo("es-AR"));

        if (raw.Contains(','))
            formatted += "," + decimalPart;

        return formatted;
    }
}
