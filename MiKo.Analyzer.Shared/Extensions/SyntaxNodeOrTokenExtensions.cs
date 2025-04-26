using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxNodeOrTokenExtensions
    {
        internal static int GetPositionWithinStartLine(this in SyntaxNodeOrToken value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetStartingLine(this in SyntaxNodeOrToken value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this in SyntaxNodeOrToken value) => value.GetLocation().GetEndingLine();

        internal static LinePosition GetStartPosition(this in SyntaxNodeOrToken value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this in SyntaxNodeOrToken value) => value.GetLocation().GetEndPosition();
    }
}