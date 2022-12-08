using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

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
            return value.SourceTree?.GetText().ToString(value.SourceSpan);
        }

        internal static string GetSurroundingWord(this Location value)
        {
            var tree = value.SourceTree;

            if (tree != null)
            {
                var sourceText = tree.GetText();
                var text = sourceText.ToString(TextSpan.FromBounds(0, value.SourceSpan.End));

                var lastIndexOfFirstSpace = text.LastIndexOfAny(Constants.WhiteSpaceCharacters);

                if (lastIndexOfFirstSpace != -1)
                {
                    var followUpText = sourceText.GetSubText(value.SourceSpan.End).ToString();

                    var firstIndexOfNextSpace = followUpText.StartsWith('<') // seems like the comment finished
                                                    ? 0
                                                    : followUpText.IndexOfAny(Constants.WhiteSpaceCharacters);

                    if (firstIndexOfNextSpace != -1)
                    {
                        var result = sourceText.ToString(TextSpan.FromBounds(lastIndexOfFirstSpace + 1, text.Length + firstIndexOfNextSpace));

                        return result;
                    }
                }
            }

            return null;
        }

        internal static bool Contains(this Location value, Location other)
        {
            var valueSourceSpan = value.SourceSpan;
            var otherSourceSpan = other.SourceSpan;

            return valueSourceSpan.Start <= otherSourceSpan.Start && otherSourceSpan.End <= valueSourceSpan.End;
        }
    }
}
