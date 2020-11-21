using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1107_TestMethodsPascalCasingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1107";

        private const string PascalCasingRegex = "[a-z]+[A-Z]+";

        public MiKo_1107_TestMethodsPascalCasingAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (Regex.IsMatch(symbolName, PascalCasingRegex) is false)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbolName.Contains("_"))
            {
                var underlinesNr = symbolName.Count(_ => _ is '_');
                var upperCasesNr = symbolName.Count(_ => _.IsUpperCase());
                var diff = underlinesNr - upperCasesNr;
                if (diff >= 0)
                {
                    return Enumerable.Empty<Diagnostic>();
                }
            }

            return new[] { Issue(symbol) };
        }
    }
}