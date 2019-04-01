using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3022_TaskReturnsIEnumerableAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3022";

        public MiKo_3022_TaskReturnsIEnumerableAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.ReturnType.IsTask();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            var typeArguments = ((INamedTypeSymbol)method.ReturnType).TypeArguments;

            if (typeArguments.Length == 1 && typeArguments[0] is INamedTypeSymbol typeArgument)
            {
                var specialType = typeArgument.SpecialType;
                switch (specialType)
                {
                    case SpecialType.System_Collections_IEnumerable:
                    case SpecialType.System_Collections_Generic_IEnumerable_T:
                    {
                        return ReportIssue(method);
                    }
                }

                if (typeArgument.TypeKind == TypeKind.Interface && typeArgument.ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                {
                    return ReportIssue(method);
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> ReportIssue(IMethodSymbol method) => new[] { ReportIssue(method, method.ReturnType.MinimalTypeName()) };
    }
}