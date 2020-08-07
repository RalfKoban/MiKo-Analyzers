using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1106_OneTimeTestTeardownMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1106";

        public const string ExpectedName = "CleanupTestEnvironment";

        public MiKo_1106_OneTimeTestTeardownMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsTestOneTimeTearDownMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name == ExpectedName
                                                                                            ? Enumerable.Empty<Diagnostic>()
                                                                                            : new[] { Issue(symbol, ExpectedName) };
    }
}