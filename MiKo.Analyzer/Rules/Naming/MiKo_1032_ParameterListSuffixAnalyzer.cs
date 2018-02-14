using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1032_ParameterListSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1032";

        public MiKo_1032_ParameterListSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol) => symbol.Parameters.SelectMany(Analyze).ToList();

        private IEnumerable<Diagnostic> Analyze(IParameterSymbol symbol)
        {
            var diagnostic = Analyze(symbol, "List") ?? Analyze(symbol, "Dictionary");
            return diagnostic != null ? new [] { diagnostic } : Enumerable.Empty<Diagnostic>();
        }

        private Diagnostic Analyze(IParameterSymbol symbol, string suffix, StringComparison comparison = StringComparison.Ordinal)
        {
            var symbolName = symbol.Name;
            if (!symbolName.EndsWith(suffix, comparison)) return null;

            var name = symbolName.Substring(0, symbolName.Length - suffix.Length);
            var expectedName = name.EndsWith("s", comparison) ? name : name + "s";
            return ReportIssue(symbol, expectedName);
        }
    }
}