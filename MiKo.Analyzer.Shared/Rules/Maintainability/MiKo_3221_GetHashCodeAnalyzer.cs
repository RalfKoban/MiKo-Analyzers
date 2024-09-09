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

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName("System." + HashCode) != null;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride && symbol.Name == nameof(GetHashCode);

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.GetSyntax<MethodDeclarationSyntax>() is MethodDeclarationSyntax method)
            {
                var expressionsCount = 0;
                var expressions = method.DescendantNodes<MemberAccessExpressionSyntax>();

                foreach (var expression in expressions)
                {
                    switch (expression.GetName())
                    {
                        case ToHashCode:
                        case Combine when expression.Expression.GetName() == HashCode:
                            yield break;
                    }

                    expressionsCount++;
                }

                if (expressionsCount == 0)
                {
                    // we do not have any members to combine
                    yield break;
                }

                yield return Issue(method);
            }
        }
    }
}