﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3211_SimpleLambdaExpressionIsUsedInsteadOfParenthesizedLambdaExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3211";

        public MiKo_3211_SimpleLambdaExpressionIsUsedInsteadOfParenthesizedLambdaExpressionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (node.ExpressionBody != null)
            {
                // simplification works only if it is a single parameter
                if (node.ParameterList.Parameters.Count == 1)
                {
                    ReportDiagnostics(context, Issue(node));
                }
            }
        }
    }
}