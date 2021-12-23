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

        public const string ExpectedName = "CleanupTest";

        public MiKo_1104_TestTeardownMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestTearDownMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => symbol.Name == ExpectedName
                                                                                                                     ? Enumerable.Empty<Diagnostic>()
                                                                                                                     : new[] { Issue(symbol, ExpectedName) };
    }
}