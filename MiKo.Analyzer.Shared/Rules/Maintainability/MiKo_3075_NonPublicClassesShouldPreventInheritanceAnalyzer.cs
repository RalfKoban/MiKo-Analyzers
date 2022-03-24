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

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            if (symbol.IsStatic || symbol.IsSealed || symbol.IsAbstract)
            {
                // nothing to report
                yield break;
            }

            if (symbol.DeclaredAccessibility == Accessibility.Private)
            {
                // find other symbols that inherit from this one
                var privateClasses = symbol.ContainingType.GetMembers().OfType<INamedTypeSymbol>().Where(_ => _.TypeKind == TypeKind.Class).ToList();
                privateClasses.Remove(symbol);

                foreach (var otherClass in privateClasses)
                {
                    if (otherClass.InheritsFrom(symbol.FullyQualifiedName()))
                    {
                        // we found a private base class, so nothing to report
                        yield break;
                    }
                }
            }

            yield return Issue(symbol);
        }
    }
}