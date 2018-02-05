using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Extensions
{
    internal static class ITypeSymbolExtensions
    {
        internal static bool InheritsFrom<T>(this ITypeSymbol symbol)
        {
            var baseClass = typeof(T).FullName;

            while (true)
            {
                if (symbol.ToString() == baseClass) return true;
                if (symbol.BaseType == null) return false;

                symbol = symbol.BaseType;
            }
        }
    }
}