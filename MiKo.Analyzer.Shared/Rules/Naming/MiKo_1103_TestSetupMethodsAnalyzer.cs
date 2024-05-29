using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1103_TestSetupMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1103";

        private const string ExpectedName = "PrepareTest";

        public MiKo_1103_TestSetupMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestSetUpMethod();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false; // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Name != ExpectedName)
            {
                yield return Issue(symbol, ExpectedName, CreateBetterNameProposal(ExpectedName));
            }
        }
    }
}