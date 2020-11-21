using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.Contains(TestMarker))
            {
                var testCase = symbolName.Contains(TestCaseMarker, StringComparison.OrdinalIgnoreCase);

                return new[] { Issue(symbol, testCase ? TestCaseMarker : TestMarker) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}