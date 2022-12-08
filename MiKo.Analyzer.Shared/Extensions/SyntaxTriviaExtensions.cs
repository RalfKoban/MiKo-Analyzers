using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxTriviaExtensions
    {
        public static bool IsSpanningMultipleLines(this SyntaxTrivia value)
        {
            var count = 0;

            foreach (var syntaxTrivia in value.Token.LeadingTrivia)
            {
                if (syntaxTrivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    if (count == 1)
                    {
                        return true;
                    }

                    count++;
                }
            }

            return false;
        }
    }
}