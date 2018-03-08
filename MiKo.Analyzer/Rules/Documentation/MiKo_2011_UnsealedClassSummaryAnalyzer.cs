using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2011_UnsealedClassSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2011";

        public MiKo_2011_UnsealedClassSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsReferenceType;

        protected override  IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var containsComment = summaries.Any(_ => _.Contains(Constants.Comments.SealedClassPhrase));

            return !symbol.IsSealed && containsComment
                       ? new[] { ReportIssue(symbol, Constants.Comments.SealedClassPhrase) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}