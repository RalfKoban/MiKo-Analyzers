using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    public abstract class ArrayCreationExpressionMaintainabilityAnalyzer : PerformanceAnalyzer
    {
        private static readonly SyntaxKind[] Expressions = { SyntaxKind.ArrayCreationExpression, SyntaxKind.ArrayInitializerExpression };

        protected ArrayCreationExpressionMaintainabilityAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeArrayCreation, Expressions);

        protected virtual bool ShallAnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel) => true;

        protected virtual bool ShallAnalyzeArrayInitializer(InitializerExpressionSyntax node, SemanticModel semanticModel) => true;

        protected abstract IEnumerable<Diagnostic> AnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel);

        protected abstract IEnumerable<Diagnostic> AnalyzeArrayInitializer(InitializerExpressionSyntax node, SemanticModel semanticModel);

        private void AnalyzeArrayCreation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArrayCreationExpressionSyntax creation)
            {
                var semanticModel = context.SemanticModel;

                if (ShallAnalyzeArrayCreation(creation, semanticModel))
                {
                    var issues = AnalyzeArrayCreation(creation, semanticModel);

                    ReportDiagnostics(context, issues);
                }
            }
            else if (context.Node is InitializerExpressionSyntax initializer && initializer.Parent is EqualsValueClauseSyntax)
            {
                var semanticModel = context.SemanticModel;

                if (ShallAnalyzeArrayInitializer(initializer, semanticModel))
                {
                    var issues = AnalyzeArrayInitializer(initializer, semanticModel);

                    ReportDiagnostics(context, issues);
                }
            }
        }
    }
}