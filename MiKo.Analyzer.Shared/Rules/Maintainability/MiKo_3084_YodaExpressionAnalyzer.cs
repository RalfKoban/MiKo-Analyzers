﻿using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3084_YodaExpressionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3084";

        private static readonly SyntaxKind[] ExpressionValues =
                                                                {
                                                                    SyntaxKind.TrueLiteralExpression,
                                                                    SyntaxKind.FalseLiteralExpression,
                                                                    SyntaxKind.NullLiteralExpression,
                                                                    SyntaxKind.NumericLiteralExpression,
                                                                    SyntaxKind.StringLiteralExpression,
                                                                };

        private static readonly SyntaxKind[] Expressions =
                                                           {
                                                               SyntaxKind.EqualsExpression,
                                                               SyntaxKind.NotEqualsExpression,
                                                               SyntaxKind.LessThanExpression,
                                                               SyntaxKind.LessThanOrEqualExpression,
                                                               SyntaxKind.GreaterThanExpression,
                                                               SyntaxKind.GreaterThanOrEqualExpression,
                                                           };

        public MiKo_3084_YodaExpressionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeExpression, Expressions);

        private static bool IsResponsibleNode(CSharpSyntaxNode syntax) => syntax != null && IsResponsibleNode(syntax.Kind());

        private static bool IsResponsibleNode(in SyntaxKind kind) => ExpressionValues.Contains(kind);

        private void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            var left = node.Left;

            if (IsResponsibleNode(left) || left.IsConst(context))
            {
                ReportDiagnostics(context, Issue(left, node.OperatorToken.ValueText));
            }
        }
    }
}