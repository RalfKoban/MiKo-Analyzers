using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SurroundedByBlankLinesAnalyzer : SpacingAnalyzer
    {
        internal const string NoLineBefore = "before";
        internal const string NoLineAfter = "after";

        protected SurroundedByBlankLinesAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected static bool HasNoBlankLinesBefore(FileLinePositionSpan callLineSpan, SyntaxNode other)
        {
            var otherLineSpan = other.GetLocation().GetLineSpan();

            var differenceBefore = callLineSpan.StartLinePosition.Line - otherLineSpan.EndLinePosition.Line;

            return differenceBefore == 1;
        }

        protected static bool HasNoBlankLinesBefore(SyntaxNode node, SyntaxNode other)
        {
            return HasNoBlankLinesBefore(node.GetLocation().GetLineSpan(), other);
        }

        protected static bool HasNoBlankLinesAfter(FileLinePositionSpan callLineSpan, SyntaxNode other)
        {
            var otherLineSpan = other.GetLocation().GetLineSpan();

            var differenceAfter = otherLineSpan.StartLinePosition.Line - callLineSpan.EndLinePosition.Line;

            return differenceAfter == 1;
        }

        protected static bool HasNoBlankLinesAfter(SyntaxNode node, SyntaxNode other)
        {
            return HasNoBlankLinesAfter(node.GetLocation().GetLineSpan(), other);
        }

        protected Diagnostic Issue(SyntaxNode call, bool noBlankLinesBefore, bool noBlankLinesAfter)
        {
            return Issue(call.GetLocation(), noBlankLinesBefore, noBlankLinesAfter);
        }

        protected Diagnostic Issue(SyntaxToken token, bool noBlankLinesBefore, bool noBlankLinesAfter)
        {
            return Issue(token.GetLocation(), noBlankLinesBefore, noBlankLinesAfter);
        }

        protected Diagnostic Issue(Location location, bool noBlankLinesBefore, bool noBlankLinesAfter)
        {
            // prepare additional data so that code fix can benefit from information
            var dictionary = new Dictionary<string, string>();

            if (noBlankLinesBefore)
            {
                dictionary.Add(NoLineBefore, string.Empty);
            }

            if (noBlankLinesAfter)
            {
                dictionary.Add(NoLineAfter, string.Empty);
            }

            return Issue(location, dictionary);
        }
    }
}