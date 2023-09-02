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

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol) => NamesFinder.FindBetterTestName(symbol.Name);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (Regex.IsMatch(symbolName, PascalCasingRegex))
            {
                if (symbolName.Contains(Constants.Underscore))
                {
                    var underlinesNr = symbolName.Count(_ => _ is Constants.Underscore);
                    var upperCasesNr = symbolName.Count(_ => _.IsUpperCase());

                    var diff = underlinesNr - upperCasesNr;

                    if (diff >= 0)
                    {
                        yield break;
                    }
                }

                yield return Issue(symbol);
            }
        }
    }
}