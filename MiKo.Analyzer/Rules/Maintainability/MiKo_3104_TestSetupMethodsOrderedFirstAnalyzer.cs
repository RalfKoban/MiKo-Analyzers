using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3104_TestSetupMethodsOrderedFirstAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3104";

        public MiKo_3104_TestSetupMethodsOrderedFirstAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => GetSetupMethod(symbol) != null
                                                                                               ? AnalyzeTestType(symbol)
                                                                                               : Enumerable.Empty<Diagnostic>();

        private static IMethodSymbol GetSetupMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                                      .OfType<IMethodSymbol>()
                                                                                      .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                      .FirstOrDefault(_ => _.IsTestSetupMethod());

        private static IEnumerable<IMethodSymbol> GetMethodsOrderedByLocation(INamedTypeSymbol symbol, string path) => symbol.GetMembers()
                                                                                                                             .OfType<IMethodSymbol>()
                                                                                                                             .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                                                                             .Where(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().Path == path)
                                                                                                                             .OrderBy(_ => _.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition);

        private IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol)
        {
            var setupMethod = GetSetupMethod(symbol);
            var path = setupMethod.Locations.First(_ => _.IsInSource).GetLineSpan().Path;

            var firstMethod = GetMethodsOrderedByLocation(symbol, path).First();

            return setupMethod.Equals(firstMethod)
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(setupMethod) };
        }
    }
}