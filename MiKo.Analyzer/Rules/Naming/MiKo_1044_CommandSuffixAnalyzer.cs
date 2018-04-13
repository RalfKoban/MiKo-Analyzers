using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1044_CommandSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1044";

        private const string Suffix = "Command";

        public MiKo_1044_CommandSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsCommand() && !symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                               ? new[] { ReportIssue(symbol, Suffix) }
                                                                                               : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Ordinary && symbol.ReturnType.IsCommand() && !symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                              ? new[] { ReportIssue(symbol, Suffix) }
                                                                                              : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol) => symbol.Type.IsCommand() && !symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                              ? new[] { ReportIssue(symbol, Suffix) }
                                                                                              : Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => symbol.Type.IsCommand() && !symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                            ? new[] { ReportIssue(symbol, Suffix) }
                                                                                            : Enumerable.Empty<Diagnostic>();
    }
}