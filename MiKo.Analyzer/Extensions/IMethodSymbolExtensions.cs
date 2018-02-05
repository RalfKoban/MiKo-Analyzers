using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Extensions
{
    internal static class IMethodSymbolExtensions
    {
        internal static bool IsEventHandler(this IMethodSymbol method)
        {
            var parameters = method.Parameters;
            return parameters.Length == 2 && parameters[0].Type.ToString() == "object" && parameters[1].Type.InheritsFrom<System.EventArgs>();
        }
    }
}