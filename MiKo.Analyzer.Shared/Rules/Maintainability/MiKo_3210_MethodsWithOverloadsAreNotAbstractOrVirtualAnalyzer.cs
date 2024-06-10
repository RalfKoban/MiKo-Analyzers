using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3210_MethodsWithOverloadsAreNotAbstractOrVirtualAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3210";

        public MiKo_3210_MethodsWithOverloadsAreNotAbstractOrVirtualAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol)
        {
            switch (symbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Struct:
                    return true;

                default:
                    return false;
            }
        }

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var group in symbol.GetNamedMethods().GroupBy(_ => _.Name))
            {
                var methods = group.ToList();

                if (methods.Count > 1)
                {
                    foreach (var diagnostic in AnalyzeMethodOverloads(methods))
                    {
                        yield return diagnostic;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeMethodOverloads(IReadOnlyCollection<IMethodSymbol> methods)
        {
            var methodWithoutParameters = methods.FirstOrDefault(_ => _.Parameters.Length == 0);

            if (methodWithoutParameters != null)
            {
                // always report parameter-less methods
                yield return AnalyzeModifier(methodWithoutParameters);
            }

            // group methods based on first parameter
            foreach (var group in methods.Where(_ => _.Parameters.Length > 0).GroupBy(_ => _.Parameters[0].Type.Name))
            {
                var groupedMethods = group.ToList();
                var maxParameters = groupedMethods.Max(_ => _.Parameters.Length);

                foreach (var otherMethod in groupedMethods.Where(_ => _.Parameters.Length < maxParameters))
                {
                    yield return AnalyzeModifier(otherMethod);
                }
            }
        }

        private Diagnostic AnalyzeModifier(IMethodSymbol method)
        {
            if (method.IsVirtual)
            {
                return Issue(method.GetModifier(SyntaxKind.VirtualKeyword));
            }

            if (method.IsAbstract)
            {
                return Issue(method.GetModifier(SyntaxKind.AbstractKeyword));
            }

            return null;
        }
    }
}