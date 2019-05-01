using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3002_MEFDependencyAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3002";

        private const int MaxDependenciesCount = 5;

        public MiKo_3002_MEFDependencyAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => !symbol.IsNamespace;

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol)
        {
            var dependencies = 0;

            foreach (var member in symbol.GetMembers())
            {
                switch (member)
                {
                    case IPropertySymbol property when property.IsImport():
                        dependencies++;
                        break;
                    case IMethodSymbol method when method.IsConstructor() && method.IsImportingConstructor():
                        dependencies += method.Parameters.Length;
                        break;
                }
            }

            return dependencies > MaxDependenciesCount ?
                       new[] { Issue(symbol, dependencies, MaxDependenciesCount) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}