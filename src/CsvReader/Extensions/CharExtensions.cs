using CsvReader.Enums;

namespace CsvReader.Extensions
{
    internal static class CharExtensions
    {
        public static SymbolType ResolveSymbolType(this char self)
            => self switch
            {
                ',' => SymbolType.Comma,
                '"' => SymbolType.Quotes,
                '\r' => SymbolType.Cr,
                '\n' => SymbolType.Lf,
                _ => SymbolType.Text
            };

        public static bool IsControlSymbol(this SymbolType self)
            => self == SymbolType.Cr || self == SymbolType.Lf;
    }
}
