using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1113";

        public MiKo_1113_TestMethodsShouldNotBeNamedGivenWhenThenAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 20) // consider only long names
            {
                var hasIssue = methodName.StartsWith("Given", StringComparison.Ordinal) && methodName.Contains("When", StringComparison.OrdinalIgnoreCase) && methodName.Contains("Then", StringComparison.OrdinalIgnoreCase);
                if (hasIssue)
                {
                    yield return Issue(symbol);
                }
            }
        }
    }
}
