using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1116";

        private static readonly string[] NonPresentPhrases = { "Got", "Had", "Returned", "Threw", "Was", "Will", "Wont" };

        public MiKo_1116_TestMethodsShouldBeInPresentTenseAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var methodName = symbol.Name;

            if (methodName.Length > 10)
            {
                if (HasIssue(methodName))
                {
                    var betterName = NamesFinder.FindBetterTestNameWithReorder(symbol.Name, symbol);

                    yield return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
                }
            }
        }

        private static bool HasIssue(string methodName) => methodName.IndexOfAny(NonPresentPhrases, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}