﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3106";

        private static readonly HashSet<string> AssertionMethods = new HashSet<string>
                                                                       {
                                                                           "That",
                                                                           "IsTrue",
                                                                           "IsFalse",
                                                                           "True",
                                                                           "False",
                                                                       };

        public MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => AssertionMethods.Contains(node.GetName())
                                                                                    && node.Expression is IdentifierNameSyntax invokedType
                                                                                    && Constants.Names.AssertionTypes.Contains(invokedType.GetName());

        private static bool IsFrameworkAssertion(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            var type = expression.GetTypeSymbol(context.SemanticModel);
            var namespaceName = type?.ContainingNamespace.FullyQualifiedName();

            return Constants.Names.AssertionNamespaces.Contains(namespaceName);
        }

        private static bool IsBinaryMethod(string methodName)
        {
            switch (methodName)
            {
                case nameof(Equals):
                case nameof(Enumerable.Contains):
                case nameof(Enumerable.Any):
                case nameof(Enumerable.All):
                {
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }

        private static bool HasIssue(ExpressionSyntax expression, out SyntaxToken token)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                {
                    var be = (BinaryExpressionSyntax)expression;
                    token = be.OperatorToken;

                    return true;
                }

                case SyntaxKind.LogicalNotExpression:
                {
                    var pue = (PrefixUnaryExpressionSyntax)expression;
                    token = pue.OperatorToken;

                    return true;
                }

                case SyntaxKind.InvocationExpression when ((InvocationExpressionSyntax)expression).Expression is MemberAccessExpressionSyntax mae:
                {
                    token = mae.Name.Identifier;

                    return IsBinaryMethod(mae.GetName());
                }

                case SyntaxKind.IsPatternExpression:
                {
                    var ipe = (IsPatternExpressionSyntax)expression;
                    token = ipe.IsKeyword;

                    return true;
                }
            }

            token = default;

            return false;
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (IsAssertionMethod(node) && node.Parent is InvocationExpressionSyntax methodCall)
            {
                foreach (var argument in methodCall.ArgumentList.Arguments)
                {
                    var expression = argument.Expression;

                    if (HasIssue(expression, out var token))
                    {
                        if (IsFrameworkAssertion(context, expression) is false)
                        {
                            ReportIssue(context, token);
                        }
                    }
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxToken token)
        {
            var methodName = context.GetEnclosingMethod()?.Name;
            var issue = Issue(methodName, token, token.ValueText);
            context.ReportDiagnostic(issue);
        }
    }
}