using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class LocationExtensions
    {
        internal static T GetEnclosing<T>(this Location value, SemanticModel semanticModel) where T : class, ISymbol
        {
            var node = value.SourceTree?.GetRoot().FindNode(value.SourceSpan);

            return node.GetEnclosingSymbol(semanticModel) as T;
        }

        internal static string GetText(this Location value)
        {
            return value.SourceTree?.GetText().GetSubText(value.SourceSpan).ToString();
        }

        internal static bool Contains(this Location value, Location other)
        {
            var valueSourceSpan = value.SourceSpan;
            var otherSourceSpan = other.SourceSpan;

            return valueSourceSpan.Start <= otherSourceSpan.Start && otherSourceSpan.End <= valueSourceSpan.End;
        }
    }
}
