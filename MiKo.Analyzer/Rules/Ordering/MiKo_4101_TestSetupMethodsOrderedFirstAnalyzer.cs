using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4101_TestSetupMethodsOrderedFirstAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4101";

        public MiKo_4101_TestSetupMethodsOrderedFirstAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol)
        {
            var method = GetSetupMethod(symbol);

            return method != null
                   ? AnalyzeTestType(symbol, method)
                   : Enumerable.Empty<Diagnostic>();
        }

        private static IMethodSymbol GetSetupMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                                      .OfType<IMethodSymbol>()
                                                                                      .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                      .FirstOrDefault(_ => _.IsTestSetupMethod());

        private static IEnumerable<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol symbol, string path) => symbol.GetMembers()
                                                                                                                             .OfType<IMethodSymbol>()
                                                                                                                             .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                                                             .Where(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().Path == path)
                                                                                                                             .OrderBy(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition);

        private IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol, IMethodSymbol setupMethod)
        {
            var path = setupMethod.Locations.First(_ => _.IsInSource).GetLineSpan().Path;

            var firstMethod = GetMethodsOrderedByLocation(symbol, path).First();

            return setupMethod.Equals(firstMethod)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(setupMethod) };
        }
    }
}