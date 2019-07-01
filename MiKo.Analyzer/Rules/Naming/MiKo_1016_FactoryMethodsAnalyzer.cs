using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1016_FactoryMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1016";

        public MiKo_1016_FactoryMethodsAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsFactory();

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsConstructor())
            {
                return false;
            }

            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return false; // ignore private methods or those that don't have any accessibility (means they are also private)

                default:
                    return base.ShallAnalyze(symbol);
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => symbol.GetMembers().OfType<IMethodSymbol>().SelectMany(AnalyzeMethod).ToList();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method) => method.Name.StartsWith("Create", StringComparison.Ordinal)
                                                                                                      ? Enumerable.Empty<Diagnostic>()
                                                                                                      : new[] { Issue(method) };
    }
}