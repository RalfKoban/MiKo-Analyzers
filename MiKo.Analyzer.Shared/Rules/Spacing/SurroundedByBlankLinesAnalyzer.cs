using System;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SurroundedByBlankLinesAnalyzer : SpacingAnalyzer
    {
        private static readonly Pair[] NoLineBoth = { new Pair(Constants.AnalyzerCodeFixSharedData.NoLineBefore), new Pair(Constants.AnalyzerCodeFixSharedData.NoLineAfter) };
        private static readonly Pair[] NoLineBefore = { new Pair(Constants.AnalyzerCodeFixSharedData.NoLineBefore) };
        private static readonly Pair[] NoLineAfter = { new Pair(Constants.AnalyzerCodeFixSharedData.NoLineAfter) };

        protected SurroundedByBlankLinesAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected static FileLinePositionSpan GetLineSpanOfNodeOrLeadingComment(SyntaxNode node)
        {
            var list = node.GetLeadingTrivia();
            var listCount = list.Count;

            if (listCount > 0)
            {
                for (var index = 0; index < listCount; index++)
                {
                    var trivia = list[index];

                    if (trivia.IsComment())
                    {
                        return trivia.GetLineSpan();
                    }
                }
            }

            return node.GetLineSpan();
        }

        protected static FileLinePositionSpan GetLineSpanOfNodeOrTrailingComment(SyntaxNode node)
        {
            var list = node.GetTrailingTrivia();
            var listCount = list.Count;

            if (listCount > 0)
            {
                for (var index = listCount - 1; index > -1; index--)
                {
                    var trivia = list[index];

                    if (trivia.IsComment())
                    {
                        return trivia.GetLineSpan();
                    }
                }
            }

            return node.GetLineSpan();
        }

        protected static bool HasNoBlankLinesBefore(in FileLinePositionSpan callLineSpan, SyntaxNode other)
        {
            var endingLine = GetLineSpanOfNodeOrTrailingComment(other).EndLinePosition.Line;

            var differenceBefore = callLineSpan.StartLinePosition.Line - endingLine;

            return differenceBefore is 1;
        }

        protected static bool HasNoBlankLinesBefore(SyntaxNode node, SyntaxNode other)
        {
            return HasNoBlankLinesBefore(GetLineSpanOfNodeOrLeadingComment(node), other);
        }

        protected static bool HasNoBlankLinesAfter(in FileLinePositionSpan callLineSpan, SyntaxNode other)
        {
            var startingLine = GetLineSpanOfNodeOrLeadingComment(other).StartLinePosition.Line;

            var differenceAfter = startingLine - callLineSpan.EndLinePosition.Line;

            return differenceAfter is 1;
        }

        protected static bool HasNoBlankLinesAfter(SyntaxNode node, SyntaxNode other)
        {
            return HasNoBlankLinesAfter(GetLineSpanOfNodeOrTrailingComment(node), other);
        }

        protected Diagnostic Issue(SyntaxNode call, in bool noBlankLinesBefore, in bool noBlankLinesAfter)
        {
            return Issue(call.GetLocation(), noBlankLinesBefore, noBlankLinesAfter);
        }

        protected Diagnostic Issue(in SyntaxToken token, in bool noBlankLinesBefore, in bool noBlankLinesAfter)
        {
            return Issue(token.GetLocation(), noBlankLinesBefore, noBlankLinesAfter);
        }

        protected Diagnostic Issue(Location location, in bool noBlankLinesBefore, in bool noBlankLinesAfter)
        {
            var pairs = CreateProperties(noBlankLinesBefore, noBlankLinesAfter);

            return Issue(location, properties: pairs);
        }

        private static Pair[] CreateProperties(in bool noBlankLinesBefore, in bool noBlankLinesAfter)
        {
            if (noBlankLinesBefore)
            {
                return noBlankLinesAfter ? NoLineBoth : NoLineBefore;
            }

            return noBlankLinesAfter ? NoLineAfter : Array.Empty<Pair>();
        }
    }
}