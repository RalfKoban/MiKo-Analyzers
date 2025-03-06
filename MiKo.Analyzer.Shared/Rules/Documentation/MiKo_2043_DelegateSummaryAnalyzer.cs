using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2043_DelegateSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2043";

        private const string StartingPhrase = Constants.Comments.DelegateSummaryStartingPhrase;

        public MiKo_2043_DelegateSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind == TypeKind.Delegate && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Constants.XmlTag.Summary, StartingPhrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeSummariesStart(symbol, compilation, commentXml, comment);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = null;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWith(StartingPhrase, comparison))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}