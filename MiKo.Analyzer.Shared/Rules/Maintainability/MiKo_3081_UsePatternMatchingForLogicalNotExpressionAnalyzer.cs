﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer : UsePatternMatchingForExpressionAnalyzer
    {
        public const string Id = "MiKo_3081";

        public MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer() : base(Id, SyntaxKind.LogicalNotExpression)
        {
        }

        protected override void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (PrefixUnaryExpressionSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (node.IsExpression(semanticModel))
            {
                // ignore expression trees
                return;
            }

            if (node.Operand.GetSymbol(semanticModel) is IFieldSymbol f && f.IsConst)
            {
                // ignore constants
                return;
            }

            ReportIssue(context, node.OperatorToken);
        }
    }
}