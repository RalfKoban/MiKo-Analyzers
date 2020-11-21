using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1105_OneTimeTestSetupMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1105";

        public const string ExpectedName = "PrepareTestEnvironment";

        public MiKo_1105_OneTimeTestSetupMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestOneTimeSetUpMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Name == ExpectedName
                                                                                            ? Enumerable.Empty<Diagnostic>()
                                                                                            : new[] { Issue(symbol, ExpectedName) };
    }
}