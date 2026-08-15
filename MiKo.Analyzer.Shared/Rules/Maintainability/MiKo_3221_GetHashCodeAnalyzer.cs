using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3221_GetHashCodeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3221";

        private const string HashCode = "HashCode";
        private const string Combine = "Combine";
        private const string ToHashCode = "ToHashCode";

        public MiKo_3221_GetHashCodeAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(Compilation compilation) => compilation.GetTypeByMetadataName("System." + HashCode) != null;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride && symbol.Name is nameof(GetHashCode);

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.GetSyntax<MethodDeclarationSyntax>() is MethodDeclarationSyntax method)
            {
                var expressionsCount = 0;

                foreach (var expression in method.DescendantNodes<MemberAccessExpressionSyntax>())
                {
                    if (expression.Is(ToHashCode) || expression.Is(HashCode, Combine))
                    {
                        yield break;
                    }

                    expressionsCount++;
                }

                if (expressionsCount is 0)
                {
                    // we do not have any members to combine
                    yield break;
                }

                yield return Issue(method);
            }
        }
    }
}