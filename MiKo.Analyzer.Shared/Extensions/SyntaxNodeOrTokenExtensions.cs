using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxNodeOrTokenExtensions
    {
        internal static int GetPositionWithinStartLine(this SyntaxNodeOrToken value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetStartingLine(this SyntaxNodeOrToken value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxNodeOrToken value) => value.GetLocation().GetEndingLine();

        internal static LinePosition GetStartPosition(this SyntaxNodeOrToken value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxNodeOrToken value) => value.GetLocation().GetEndPosition();
    }
}