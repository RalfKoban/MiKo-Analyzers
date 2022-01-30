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

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsReferenceType && symbol.DeclaredAccessibility == Accessibility.Public && symbol.IsTestClass() is false && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            return symbol.IsSealed is false && summaries.Any(_ => _.Contains(Constants.Comments.SealedClassPhrase))
                       ? new[] { Issue(symbol, Constants.Comments.SealedClassPhrase) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}