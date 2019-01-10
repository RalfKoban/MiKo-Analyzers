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

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol) => symbol.IsTestClass() || HasSetupMethod(symbol)
                                                                                               ? AnalyzeTestType(symbol)
                                                                                               : Enumerable.Empty<Diagnostic>();

        private static bool HasSetupMethod(INamedTypeSymbol symbol) => symbol.GetMembers()
                                                                             .OfType<IMethodSymbol>()
                                                                             .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                                                             .Any(_ => _.IsTestSetupMethod());

        private IEnumerable<Diagnostic> AnalyzeTestType(INamedTypeSymbol symbol)
        {
            var methods = symbol.GetMembers()
                                .OfType<IMethodSymbol>()
                                .Where(_ => _.MethodKind == MethodKind.Ordinary)
                                .OrderBy(_ => _.Locations.First().GetLineSpan().StartLinePosition)
                                .ToList();

            foreach (var method in methods)
            {
                if (method.IsTestSetupMethod() && !method.Equals(methods[0]))
                {
                    return new[] { ReportIssue(method) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}