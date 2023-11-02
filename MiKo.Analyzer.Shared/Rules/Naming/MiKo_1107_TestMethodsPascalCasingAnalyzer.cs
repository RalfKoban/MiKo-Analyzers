using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1107_TestMethodsPascalCasingAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1107";

        public MiKo_1107_TestMethodsPascalCasingAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol) => NamesFinder.FindBetterTestName(symbol.Name);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => HasIssue(symbol.Name)
                                                                                                                 ? new[] { Issue(symbol) }
                                                                                                                 : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(string symbolName)
        {
            if (symbolName.IsPascalCasing() is false)
            {
                return false;
            }

            if (symbolName.Contains(Constants.Underscore))
            {
                var underlinesNr = symbolName.Count(_ => _ is Constants.Underscore);
                var upperCasesNr = symbolName.Count(_ => _.IsUpperCase());

                var diff = underlinesNr - upperCasesNr;

                if (diff >= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}