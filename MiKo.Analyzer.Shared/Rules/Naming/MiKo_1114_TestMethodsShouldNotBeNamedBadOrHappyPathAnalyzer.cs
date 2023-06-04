using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1114_TestMethodsShouldNotBeNamedBadOrHappyPathAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1114";

        private static readonly string[] WrongTerms =
                                                      {
                                                          "bad_case",
                                                          "bad_path",
                                                          "BadCase",
                                                          "BadPath",
                                                          "good_case",
                                                          "good_path",
                                                          "GoodCase",
                                                          "GoodPath",
                                                          "happy_case",
                                                          "happy_path",
                                                          "HappyCase",
                                                          "HappyPath",
                                                      };

        public MiKo_1114_TestMethodsShouldNotBeNamedBadOrHappyPathAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol)
                                                                      && symbol.IsTestMethod()
                                                                      && symbol.Name.Length >= 7; // consider only long names

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Name.ContainsAny(WrongTerms))
            {
                yield return Issue(symbol);
            }
        }
    }
}