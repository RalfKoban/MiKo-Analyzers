using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4102_TestTeardownMethodsOrderedSecondAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4102";

        public MiKo_4102_TestTeardownMethodsOrderedSecondAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            var method = GetTeardownMethod(symbol);

            return method != null
                   ? AnalyzeTestType(symbol, method)
                   : Enumerable.Empty<Diagnostic>();
        }

        private static IMethodSymbol GetTeardownMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                                         .OfType<IMethodSymbol>()
                                                                                         .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                         .FirstOrDefault(_ => _.IsTestTeardownMethod());

        private IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol, IMethodSymbol teardownMethod)
        {
            var path = teardownMethod.Locations.First(_ => _.IsInSource).GetLineSpan().Path;

            var methods = GetMethodsOrderedByLocation(symbol, path).ToList();
            var index = methods.Any(_ => _.IsTestSetupMethod()) ? 1 : 0;

            return teardownMethod.Equals(methods[index])
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(teardownMethod) };
        }
    }
}