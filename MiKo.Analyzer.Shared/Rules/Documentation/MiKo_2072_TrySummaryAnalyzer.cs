using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2072_TrySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2072";

        internal const string StartingPhrase = "Attempts to";

        internal static readonly string[] Words = { "Try", "Tries" };

        public MiKo_2072_TrySummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeTextStart(symbol, summaryXml);
            }
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.Ordinal;

            var firstWord = new StringBuilder(valueText).Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                                                        .ToString()
                                                        .FirstWord();

            if (firstWord.EqualsAny(Words))
            {
                problematicText = firstWord;

                return true;
            }

            problematicText = null;

            return false;
        }
    }
}