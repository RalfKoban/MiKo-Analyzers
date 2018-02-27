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

        public MiKo_1102_TestMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol)
        {
            const string TestSuffix = "Test";

            if (symbol.IsTestMethod() && symbol.Name.Contains(TestSuffix))
                return new[] { ReportIssue(symbol, symbol.Name.RemoveAll(TestSuffix)) };

            return Enumerable.Empty<Diagnostic>();
        }
    }
}