using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1103_TestSetupMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1103";

        public MiKo_1103_TestSetupMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol symbol)
        {
            const string Marker = "PrepareTest";

            if (symbol.IsTestSetupMethod() && symbol.Name != Marker)
                return new[] { ReportIssue(symbol, Marker) };

            return Enumerable.Empty<Diagnostic>();
        }
    }
}