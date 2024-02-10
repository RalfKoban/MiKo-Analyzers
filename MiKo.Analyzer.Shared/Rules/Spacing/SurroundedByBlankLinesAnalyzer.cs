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

        protected static Location GetLocationOfNodeOrLeadingComment(SyntaxNode node)
        {
            var list = node.GetLeadingTrivia();

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var listCount = list.Count;

            for (var index = 0; index < listCount; index++)
            {
                var trivia = list[index];

                if (trivia.IsComment())
                {
                    return trivia.GetLocation();
                }
            }

            return node.GetLocation();
        }

        protected static Location GetLocationOfNodeOrTrailingComment(SyntaxNode node)
        {
            var list = node.GetTrailingTrivia();

            for (var index = list.Count - 1; index > -1; index--)
            {
                var trivia = list[index];

                if (trivia.IsComment())
                {
                    return trivia.GetLocation();
                }
            }

            return node.GetLocation();
        }

        protected static bool HasNoBlankLinesBefore(FileLinePositionSpan callLineSpan, SyntaxNode other)
        {
            var endingLine = GetLocationOfNodeOrTrailingComment(other).GetEndingLine();

            var differenceBefore = callLineSpan.StartLinePosition.Line - endingLine;

            return differenceBefore == 1;
        }

        protected static bool HasNoBlankLinesBefore(SyntaxNode node, SyntaxNode other)
        {
            return HasNoBlankLinesBefore(GetLocationOfNodeOrLeadingComment(node).GetLineSpan(), other);
        }

        protected static bool HasNoBlankLinesAfter(FileLinePositionSpan callLineSpan, SyntaxNode other)
        {
            var startingLine = GetLocationOfNodeOrLeadingComment(other).GetStartingLine();

            var differenceAfter = startingLine - callLineSpan.EndLinePosition.Line;

            return differenceAfter == 1;
        }

        protected static bool HasNoBlankLinesAfter(SyntaxNode node, SyntaxNode other)
        {
            return HasNoBlankLinesAfter(GetLocationOfNodeOrTrailingComment(node).GetLineSpan(), other);
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