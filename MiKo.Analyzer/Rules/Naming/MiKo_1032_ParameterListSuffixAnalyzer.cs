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

            var betterName = GetBetterName(symbolName, suffix, comparison);
            return ReportIssue(symbol, betterName);
        }

        private static string GetBetterName(string symbolName, string suffix, StringComparison comparison)
        {
            var name = symbolName.Substring(0, symbolName.Length - suffix.Length);
            if (name.EndsWith("y", comparison)) return name.Substring(0, name.Length - 1) + "ies";
            if (name.EndsWith("ss", comparison)) return name + "es";
            if (name.EndsWith("complete", comparison)) return "all";
            if (name.EndsWith("Data", comparison)) return name;
            if (name.EndsWith("Datas", comparison)) return name.Substring(0, name.Length - 1);
            if (name.EndsWith("nformation", comparison)) return name;
            if (name.EndsWith("nformations", comparison)) return name.Substring(0, name.Length - 1);

            var betterName = name;
            if (symbolName.IsEntityMarker())
                betterName = name.Substring(0, name.Length - 5);
            else if (name.EndsWith("ToConvert", comparison))
                betterName = name.Substring(0, name.Length - "ToConvert".Length);

            return betterName.EndsWith("s", comparison) ? betterName : betterName + "s";
        }
    }
}