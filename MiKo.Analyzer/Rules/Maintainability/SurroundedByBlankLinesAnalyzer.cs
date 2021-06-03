using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class SurroundedByBlankLinesAnalyzer : MaintainabilityAnalyzer
    {
        internal const string NoLineBefore = "before";
        internal const string NoLineAfter = "after";

        protected SurroundedByBlankLinesAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected static bool HasNoBlankLinesBefore(FileLinePositionSpan callLineSpan, StatementSyntax other)
        {
            var otherLineSpan = other.GetLocation().GetLineSpan();

            var differenceBefore = callLineSpan.StartLinePosition.Line - otherLineSpan.EndLinePosition.Line;

            return differenceBefore == 1;
        }

        protected static bool HasNoBlankLinesAfter(FileLinePositionSpan callLineSpan, StatementSyntax other)
        {
            var otherLineSpan = other.GetLocation().GetLineSpan();

            var differenceAfter = otherLineSpan.StartLinePosition.Line - callLineSpan.EndLinePosition.Line;

            return differenceAfter == 1;
        }

        protected Diagnostic Issue(SyntaxNode call, bool noBlankLinesBefore, bool noBlankLinesAfter)
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

            return Issue(call, dictionary);
        }
    }
}