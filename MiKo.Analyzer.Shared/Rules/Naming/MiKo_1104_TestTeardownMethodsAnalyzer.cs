using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1104_TestTeardownMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1104";

        private const string ExpectedName = "CleanupTest";

        public MiKo_1104_TestTeardownMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestTearDownMethod();

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // do not consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.Name != ExpectedName)
            {
                yield return Issue(symbol, ExpectedName, CreateBetterNameProposal(ExpectedName));
            }
        }
    }
}