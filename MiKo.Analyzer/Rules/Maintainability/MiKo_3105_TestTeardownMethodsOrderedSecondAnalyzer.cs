using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3105_TestTeardownMethodsOrderedSecondAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3105";

        public MiKo_3105_TestTeardownMethodsOrderedSecondAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsTestClass() || HasTeardownMethod(symbol)
                                                                                               ? AnalyzeTestType(symbol)
                                                                                               : Enumerable.Empty<Diagnostic>();

        private static bool HasTeardownMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                                .OfType<IMethodSymbol>()
                                                                                .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                .Any(_ => _.IsTestTeardownMethod());

        private IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol)
        {
            var methods = symbol.GetMembers()
                                .OfType<IMethodSymbol>()
                                .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                .OrderBy(_ => _.Locations.First().GetLineSpan().StartLinePosition)
                                .ToList();

            var index = methods.Any(_ => _.IsTestSetupMethod()) ? 1 : 0;

            foreach (var method in methods)
            {
                if (method.IsTestTeardownMethod() && !method.Equals(methods[index]))
                {
                    return new[] { ReportIssue(method) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}