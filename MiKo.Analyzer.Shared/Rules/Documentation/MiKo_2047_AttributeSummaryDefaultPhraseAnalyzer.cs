using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2047";

        private static readonly string StartingPhrases = Constants.Comments.AttributeSummaryStartingPhrase.OrderBy(_ => _).HumanizedConcatenated();

        public MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.InheritsFrom<Attribute>() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Constants.XmlTag.Summary, StartingPhrases);

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

            var text = valueText.AsSpan().TrimStart();

            var startsWith = text.StartsWithAny(Constants.Comments.AttributeSummaryStartingPhrase, comparison);

            problematicText = valueText.FirstWord();

            return startsWith is false;
        }
    }
}