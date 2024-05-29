using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1111";

        public MiKo_1111_TestMethodsWithoutParametersNotSuffixedWithUnderscoreAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.Parameters.Length == 0 && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false; // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.Underscore))
            {
                yield return Issue(symbol, CreateBetterNameProposal(symbolName.TrimEnd(Constants.Underscore)));
            }
        }
    }
}