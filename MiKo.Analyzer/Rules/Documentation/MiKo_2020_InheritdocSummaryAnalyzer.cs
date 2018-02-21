using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]

    public sealed class MiKo_2020_InheritdocSummaryAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2020";

        public MiKo_2020_InheritdocSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            Initialize(context, SymbolKind.Event);
            Initialize(context, SymbolKind.Field);
            Initialize(context, SymbolKind.Method);
            Initialize(context, SymbolKind.NamedType);
            Initialize(context, SymbolKind.Property);
        }

        protected override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol) => AnalyzeSummary(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => AnalyzeSummary(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => AnalyzeSummary(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => AnalyzeSummary(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => AnalyzeSummary(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(summary => IsSeeCrefLink(summary.Trim()))
                                                                                                                        ? new[] { ReportIssue(symbol) }
                                                                                                                        : Enumerable.Empty<Diagnostic>();

        private static bool IsSeeCrefLink(string summary)
        {
            const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

            return summary.StartsWithAny(Comparison, "<see cref=", "<seealso cref=") && summary.EndsWithAny(Comparison, "/>", "/>.", "/see>", "/see>.", "/seealso>", "/seealso>.");
        }
    }
}