using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_4008_GetHashCodeMethodsOrderedAfterEqualsMethodAnalyzer : OrderingAnalyzer
    {
        public const string Id = "MiKo_4008";

        public MiKo_4008_GetHashCodeMethodsOrderedAfterEqualsMethodAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation)
        {
            var methods = GetMethodsOrderedByLocation(symbol).Where(_ => _.DeclaredAccessibility == Accessibility.Public).ToList();
            var methodNames = methods.ToHashSet(_ => _.Name);

            return methodNames.Contains(nameof(GetHashCode)) && methodNames.Contains(nameof(Equals))
                   ? AnalyzeMethods(methods)
                   : Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeMethods(List<IMethodSymbol> methods)
        {
            var potentialCorrectPositions = methods.Where(_ => _.Name == nameof(Equals)).ToHashSet(_ => methods.IndexOf(_) + 1);

            var getHashCodeMethod = methods.FindLast(_ => _.Name == nameof(GetHashCode));
            var currentPosition = methods.IndexOf(getHashCodeMethod);

            if (potentialCorrectPositions.Contains(currentPosition) is false)
            {
                // not directly behind any Equals method
                yield return Issue(getHashCodeMethod);
            }
        }
    }
}