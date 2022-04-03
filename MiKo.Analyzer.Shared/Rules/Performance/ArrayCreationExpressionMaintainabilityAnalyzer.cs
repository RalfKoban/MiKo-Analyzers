using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    public abstract class ArrayCreationExpressionMaintainabilityAnalyzer : PerformanceAnalyzer
    {
        protected ArrayCreationExpressionMaintainabilityAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeArrayCreation, SyntaxKind.ArrayCreationExpression);

        protected virtual bool ShallAnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel) => true;

        protected abstract IEnumerable<Diagnostic> AnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel);

        private void AnalyzeArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var node = (ArrayCreationExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (ShallAnalyzeArrayCreation(node, semanticModel))
            {
                var issues = AnalyzeArrayCreation(node, semanticModel);

                ReportDiagnostics(context, issues);
            }
        }
    }
}