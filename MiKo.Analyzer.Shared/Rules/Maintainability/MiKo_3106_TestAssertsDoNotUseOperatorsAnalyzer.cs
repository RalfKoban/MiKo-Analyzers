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

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => AssertionMethods.Contains(node.GetName())
                                                                                    && node.Expression is IdentifierNameSyntax invokedType
                                                                                    && Constants.Names.AssertionTypes.Contains(invokedType.GetName());

        private static bool IsXUnitAssertTrueMethod(MemberAccessExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.GetName() is "True" && node.Expression is IdentifierNameSyntax invokedType && invokedType.GetName() is "Assert")
            {
                var symbol = invokedType.GetTypeSymbol(semanticModel);

                if (symbol?.FullyQualifiedName() is "Xunit.Assert")
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsFrameworkAssertion(in SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
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

            var issues = AnalyzeSimpleMemberAccessExpression(context, node);
            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context, MemberAccessExpressionSyntax node)
        {
            if (IsAssertionMethod(node) && node.Parent is InvocationExpressionSyntax methodCall)
            {
                var arguments = methodCall.ArgumentList.Arguments;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var argumentsCount = arguments.Count;

                if (argumentsCount is 2 && IsXUnitAssertTrueMethod(node, context.SemanticModel))
                {
                    // XUnit does not provide user assertion messages on other methods, so we have to ignore the operators here as the user like wants to use the assertion message
                    yield break;
                }

                for (var index = 0; index < argumentsCount; index++)
                {
                    var expression = arguments[index].Expression;

                    if (HasIssue(expression, out var token) && IsFrameworkAssertion(context, expression) is false)
                    {
                        var methodName = context.GetEnclosingMethod()?.Name;

                        yield return Issue(methodName, token, token.ValueText);
                    }
                }
            }
        }
    }
}