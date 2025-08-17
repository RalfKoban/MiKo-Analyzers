using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    public abstract class ArrayCreationExpressionPerformanceAnalyzer : PerformanceAnalyzer
    {
        private static readonly SyntaxKind[] Expressions = { SyntaxKind.ArrayCreationExpression, SyntaxKind.ArrayInitializerExpression };

        protected ArrayCreationExpressionPerformanceAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeArrayCreation, Expressions);

        protected virtual bool ShallAnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel) => true;

        protected virtual bool ShallAnalyzeArrayInitializer(InitializerExpressionSyntax node, SemanticModel semanticModel) => true;

        protected abstract IEnumerable<Diagnostic> AnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel);

        protected abstract IEnumerable<Diagnostic> AnalyzeArrayInitializer(InitializerExpressionSyntax node, SemanticModel semanticModel);

        private void AnalyzeArrayCreation(SyntaxNodeAnalysisContext context)
        {
            var issues = AnalyzeArrayCreationNode(context.Node, context);

            if (issues.IsEmptyArray() is false)
            {
                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeArrayCreationNode(SyntaxNode node, in SyntaxNodeAnalysisContext context)
        {
            switch (node)
            {
                case ArrayCreationExpressionSyntax creation:
                {
                    var semanticModel = context.SemanticModel;

                    if (ShallAnalyzeArrayCreation(creation, semanticModel))
                    {
                        return AnalyzeArrayCreation(creation, semanticModel);
                    }

                    break;
                }

                case InitializerExpressionSyntax initializer when initializer.Parent is EqualsValueClauseSyntax:
                {
                    var semanticModel = context.SemanticModel;

                    if (ShallAnalyzeArrayInitializer(initializer, semanticModel))
                    {
                        return AnalyzeArrayInitializer(initializer, semanticModel);
                    }

                    break;
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}