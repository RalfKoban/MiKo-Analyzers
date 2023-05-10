using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2070";

        internal static readonly string[] Phrases = { "Return", "Returns" };

        public MiKo_2070_ReturnsSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            switch (symbol.Name)
            {
                case nameof(ToString):
                case nameof(IEnumerable.GetEnumerator):
                    return false;

                default:
                    return base.ShallAnalyze(symbol);
            }
        }

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, GetProposal(symbol));

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
            comparison = StringComparison.OrdinalIgnoreCase;

            var firstWord = valueText.Without(Constants.Comments.AsynchrounouslyStartingPhrase) // skip over async starting phrase
                                     .FirstWord();

            problematicText = valueText.FirstWord();

            return firstWord.EqualsAny(Phrases);
        }

        private static string GetProposal(ISymbol symbol)
        {
            if (symbol is IMethodSymbol m)
            {
                var startText = m.ReturnType.IsBoolean()
                                ? Constants.Comments.DeterminesWhetherPhrase
                                : "Gets";

                if (m.IsAsync)
                {
                    return Constants.Comments.AsynchrounouslyStartingPhrase + startText.ToLowerCaseAt(0);
                }

                return startText;
            }

            return "Gets";
        }
    }
}