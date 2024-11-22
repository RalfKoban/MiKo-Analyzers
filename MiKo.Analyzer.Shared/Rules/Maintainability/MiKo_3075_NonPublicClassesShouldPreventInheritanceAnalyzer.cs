using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                return Enumerable.Empty<Diagnostic>();
            }

            var node = symbol.GetSyntaxNodeInSource();

            if (node is CompilationUnitSyntax)
            {
                // nothing to report as we cannot use it anyway (such as the 'Program' class in the new C# global statement style)
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbol.DeclaredAccessibility == Accessibility.Private)
            {
                // find other symbols that inherit from this one
                var privateTypes = symbol.ContainingType.GetMembers<INamedTypeSymbol>();

                if (privateTypes.Count > 0)
                {
                    var privateClasses = privateTypes.Where(_ => _.TypeKind == TypeKind.Class).ToList();
                    privateClasses.Remove(symbol);

                    foreach (var otherClass in privateClasses)
                    {
                        if (otherClass.InheritsFrom(symbol.FullyQualifiedName()))
                        {
                            // we found a private base class, so nothing to report
                            return Enumerable.Empty<Diagnostic>();
                        }
                    }
                }
            }

            return new[] { Issue(symbol) };
        }
    }
}