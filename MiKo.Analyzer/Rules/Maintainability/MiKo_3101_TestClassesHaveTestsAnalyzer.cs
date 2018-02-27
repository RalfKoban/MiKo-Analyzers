using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3101_TestClassesHaveTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3101";

        public MiKo_3101_TestClassesHaveTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (!symbol.IsTestClass()) return Enumerable.Empty<Diagnostic>();
            if (symbol.GetMembers().OfType<IMethodSymbol>().Any(_ => _.IsTestMethod())) return Enumerable.Empty<Diagnostic>();

            return new[] { ReportIssue(symbol) };
        }
    }
}