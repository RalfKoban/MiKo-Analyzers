using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3075";

        public MiKo_3075_NonPublicClassesShouldPreventInheritanceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind == TypeKind.Class)
            {
                switch (symbol.DeclaredAccessibility)
                {
                    case Accessibility.Private:
                    case Accessibility.Internal:
                        return true;
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol) => symbol.IsStatic || symbol.IsSealed
                                                                                           ? Enumerable.Empty<Diagnostic>()
                                                                                           : new[] { Issue(symbol) };
    }
}