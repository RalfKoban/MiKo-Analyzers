using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1062_IsMethodNameCamelCaseAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1062";

        private static readonly string[] Prefixes = { "Is", "Has", "Contains" };

        public MiKo_1062_IsMethodNameCamelCaseAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => AnalyzeCamelCase(symbol, 4);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol) => AnalyzeCamelCase(symbol, 3);

        private IEnumerable<Diagnostic> AnalyzeCamelCase(ISymbol symbol, int limit) => ViolatesLimit(symbol.Name, limit)
                                                                                           ? new[] { ReportIssue(symbol, limit) }
                                                                                           : Enumerable.Empty<Diagnostic>();

        private static bool ViolatesLimit(string name, int limit)
        {
            return name.StartsWithAny(Prefixes, StringComparison.Ordinal) && name.Count(_ => _.IsUpperCase()) > limit;
        }
    }
}