using System.Collections.Generic;

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

        public MiKo_3106_TestAssertsDoNotUseOperatorsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            if (node.Expression is IdentifierNameSyntax invokedClass && AssertionTypes.Contains(invokedClass.Identifier.ValueText))
            {
                if (node.Parent is InvocationExpressionSyntax methodCall && methodCall.ArgumentList.Arguments.Count > 0)
                {
                    var methodName = context.GetEnclosingMethod()?.Name;

                    foreach (var descendant in methodCall.ArgumentList.DescendantNodes())
                    {
                        switch (descendant.Kind())
                        {
                            case SyntaxKind.EqualsExpression:
                            case SyntaxKind.NotEqualsExpression:
                            case SyntaxKind.LessThanExpression:
                            case SyntaxKind.LessThanOrEqualExpression:
                            case SyntaxKind.GreaterThanExpression:
                            case SyntaxKind.GreaterThanOrEqualExpression:
                            {
                                var expression = (BinaryExpressionSyntax)descendant;
                                var issue = Issue(methodName, expression.GetLocation(), expression.OperatorToken.ValueText);
                                context.ReportDiagnostic(issue);
                                break;
                            }

                            default:
                            {
                                // TODO: check for .Equals or LINQ methods
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}