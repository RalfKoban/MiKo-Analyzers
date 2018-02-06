using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2002_UnsealedClassAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2002";

        public MiKo_2002_UnsealedClassAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyzeType(INamedTypeSymbol symbol) => symbol.IsReferenceType;

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, string commentXml) => AnalyzeSummary(symbol, commentXml);

        protected override  IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var containsComment = summaries.Any(_ => _.Contains(MiKo_2001_SealedClassAnalyzer.ExpectedComment));

            return !symbol.IsSealed && containsComment
                       ? new[] { ReportIssue(symbol, MiKo_2001_SealedClassAnalyzer.ExpectedComment) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}