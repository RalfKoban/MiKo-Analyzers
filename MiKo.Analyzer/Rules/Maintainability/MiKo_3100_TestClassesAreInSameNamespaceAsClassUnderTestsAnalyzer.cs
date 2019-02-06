using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3100";

        public MiKo_3100_TestClassesAreInSameNamespaceAsClassUnderTestsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            if (symbol.IsTestClass())
            {
                var typeUnderTest = symbol.GetMembers("ObjectUnderTest").OfType<IPropertySymbol>().FirstOrDefault()?.GetReturnType();
                if (typeUnderTest != null)
                {
                    var testNamespace = symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var typeUnderTestNamespace = typeUnderTest.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    if (testNamespace != typeUnderTestNamespace)
                        return new[] { ReportIssue(symbol) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}