using System.Collections.Generic;
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

        private static readonly HashSet<string> AssertionTypes = new HashSet<string>
                                                                     {
                                                                         "Assert",
                                                                         "StringAssert",
                                                                         "CollectionAssert",
                                                                         "FileAssert",
                                                                         "DirectoryAssert",
                                                                     };

        private static readonly HashSet<string> AssertionMethods = new HashSet<string>
                                                                       {
                                                                           "That",
                                                                           "IsTrue",
                                                                           "IsFalse",
                                                                       };

        public MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => AssertionMethods.Contains(node.Name.Identifier.ValueText)
                                                                                 && node.Expression is IdentifierNameSyntax invokedType
                                                                                 && AssertionTypes.Contains(invokedType.Identifier.ValueText);

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

        private static bool HasIssue(ExpressionSyntax expression, out string invokedName)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                {
                    var be = (BinaryExpressionSyntax)expression;
                    invokedName = be.OperatorToken.ValueText;
                    return true;
                }

                case SyntaxKind.InvocationExpression:
                {
                    if (((InvocationExpressionSyntax)expression).Expression is MemberAccessExpressionSyntax mae)
                    {
                        var invokedMethodName = mae.Name.Identifier.ValueText;

                        if (IsBinaryMethod(invokedMethodName))
                        {
                            invokedName = invokedMethodName;
                            return true;
                        }
                    }

                    break;
                }
            }

            invokedName = null;
            return false;
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (IsAssertionMethod(node) && node.Parent is InvocationExpressionSyntax methodCall)
            {
                var arguments = methodCall.ArgumentList.Arguments;
                if (arguments.Count > 0)
                {
                    var methodName = context.GetEnclosingMethod()?.Name;

                    foreach (var argument in arguments)
                    {
                        var expression = argument.Expression;

                        if (HasIssue(expression, out var invokedName))
                        {
                            ReportIssue(context, methodName, expression, invokedName);
                        }
                    }
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, string methodName, ExpressionSyntax expression, string invokedMethodName)
        {
            var issue = Issue(methodName, expression.GetLocation(), invokedMethodName);
            context.ReportDiagnostic(issue);
        }
    }
}