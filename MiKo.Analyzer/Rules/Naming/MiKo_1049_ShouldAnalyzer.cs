using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1049_ShouldAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1049";

        private const string Marker = "Should";

        public MiKo_1049_ShouldAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol) => symbol.Name.Contains(Marker, StringComparison.OrdinalIgnoreCase)
                                                                       ? new[] { ReportIssue(symbol, Marker) }
                                                                       : Enumerable.Empty<Diagnostic>();
    }
}