using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3110_TestAssertsDoNotUseCountAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3110";

        private static readonly HashSet<string> AssertionMethods = new HashSet<string>
                                                                       {
                                                                           "That",
                                                                           "AreEqual",
                                                                           "AreNotEqual",
                                                                           "AreSame",
                                                                           "AreNotSame",
                                                                           "Less",
                                                                           "LessOrEqual",
                                                                           "Greater",
                                                                           "GreaterOrEqual",
                                                                       };

        public MiKo_3110_TestAssertsDoNotUseCountAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => AssertionMethods.Contains(node.GetName())
                                                                                    && node.Expression is IdentifierNameSyntax invokedType
                                                                                    && Constants.Names.AssertionTypes.Contains(invokedType.GetName());

        private static bool IsFixableAssertionForLinqCall(InvocationExpressionSyntax invocation)
        {
            if (invocation.GetName() == "Is")
            {
                switch (invocation.Expression.GetName())
                {
                    case "EqualTo":
                    case "Zero":
                        return true;
                }
            }

            return false;
        }

        private static bool HasIssue(MemberAccessExpressionSyntax expression, out SyntaxToken token)
        {
            switch (expression.GetName())
            {
                case "Count":
                case "Length":
                {
                    token = expression.Name.Identifier;

                    return true;
                }
            }

            token = default;

            return false;
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            var issues = AnalyzeSimpleMemberAccessExpression(node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (IsAssertionMethod(node) && node.Parent is InvocationExpressionSyntax methodCall)
            {
                var arguments = methodCall.ArgumentList.Arguments.ToList();

                foreach (var argument in arguments)
                {
                    var issue = AnalyzeArgument(node, argument, arguments);

                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private Diagnostic AnalyzeArgument(MemberAccessExpressionSyntax node, ArgumentSyntax argument, IReadOnlyList<ArgumentSyntax> arguments)
        {
            switch (argument.Expression)
            {
                case MemberAccessExpressionSyntax m when HasIssue(m, out var token):
                {
                    // 'values.Count' or 'values.Length' call
                    return Issue(token, token.ValueText);
                }

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax m && HasIssue(m, out var token):
                {
                    // linq call
                    switch (node.GetName())
                    {
                        case "AreEqual":
                        {
                            return Issue(token, token.ValueText);
                        }

                        case "That" when arguments.Count >= 2:
                        {
                            var expression = arguments[1].Expression;

                            if (expression is InvocationExpressionSyntax ai && IsFixableAssertionForLinqCall(ai))
                            {
                                // we can only fix "Assert.That(xyz.Count(), Is.EqualTo(42)"
                                return Issue(token, token.ValueText);
                            }

                            if (expression is MemberAccessExpressionSyntax am && am.GetName() == "Zero")
                            {
                                // we can only fix "Assert.That(xyz.Count(), Is.Zero"
                                return Issue(token, token.ValueText);
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            return null;
        }
    }
}