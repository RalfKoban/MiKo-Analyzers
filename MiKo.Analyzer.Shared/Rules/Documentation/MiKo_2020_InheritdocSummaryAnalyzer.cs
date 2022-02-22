using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2020_InheritdocSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2020";

        private static readonly string[] SeeStartingPhrase = { "<see cref=", "<seealso cref=", "see <see cref=", "see <seealso cref=", "seealso <see cref=", "seealso <seealso cref=" };
        private static readonly string[] SeeEndingPhrase = { "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>." };

        public MiKo_2020_InheritdocSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml) => IsSeeCrefLink(summaryXml.ToString()) ? Issue(symbol) : null;

/*
 * TODO RKN:
        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxNode node) => node.IsSeeCref() ? Issue(symbol) : null;

        protected override Diagnostic SummaryIssue(ISymbol symbol, SyntaxToken textToken)
        {
            var summary = textToken.ValueText;
            var firstWord = summary.FirstWord();
            switch (firstWord)
            {
                case "See":
                case "Seealso":
                case "see":
                case "seealso":
                    return Issue(symbol);

                default:
                    return null;
            }
        }
*/

        private static bool IsSeeCrefLink(string summary) => summary.StartsWithAny(SeeStartingPhrase) && summary.EndsWithAny(SeeEndingPhrase);
    }
}