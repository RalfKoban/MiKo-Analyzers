using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1102_TestMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1102";

        private const string TestMarker = "Test";
        private const string TestCaseMarker = "TestCase";

        public MiKo_1102_TestMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        internal static string FindBetterName(ISymbol symbol)
        {
            var symbolName = symbol.Name;
            var marker = GetTestMarker(symbolName);
            var phrases = new[]
                              {
                                  marker.SurroundedWith("_"),
                                  "_" + marker,
                                  marker + "_",
                                  marker,
                              };

            return symbolName.Without(phrases);
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.ContainingSymbol is IMethodSymbol method && ShallAnalyze(method);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.Contains(TestMarker))
            {
                yield return Issue(symbol, GetTestMarker(symbolName));
            }
        }

        private static string GetTestMarker(string symbolName)
        {
            var testCase = symbolName.Contains(TestCaseMarker, StringComparison.OrdinalIgnoreCase);

            return testCase ? TestCaseMarker : TestMarker;
        }
    }
}