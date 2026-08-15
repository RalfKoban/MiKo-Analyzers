using System.Collections.Generic;

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

        public MiKo_3110_TestAssertsDoNotUseCountAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);

        private static bool IsAssertionMethod(MemberAccessExpressionSyntax node) => AssertionMethods.Contains(node.GetName())
                                                                                 && node.Expression is IdentifierNameSyntax invokedType
                                                                                 && Constants.Names.AssertionTypes.Contains(invokedType.GetName());

        private static bool IsFixableAssertionForLinqCall(InvocationExpressionSyntax invocation) => invocation.Is("Is", "EqualTo") || invocation.Is("Is", "Zero");

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

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation)
            {
                var issues = AnalyzeInvocationExpression(invocation);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax maes && IsAssertionMethod(maes))
            {
                var arguments = node.ArgumentList.Arguments;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                for (int index = 0, argumentsCount = arguments.Count; index < argumentsCount; index++)
                {
                    var issue = AnalyzeArgument(maes, arguments[index], arguments);

                    if (issue != null)
                    {
                        yield return issue;
                    }
                }
            }
        }

        private Diagnostic AnalyzeArgument(MemberAccessExpressionSyntax node, ArgumentSyntax argument, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            switch (argument.Expression)
            {
                case MemberAccessExpressionSyntax m when HasIssue(m, out var token):
                {
                    // 'values.Count' or 'values.Length' call
                    return Issue(token);
                }

                case InvocationExpressionSyntax i when i.Expression is MemberAccessExpressionSyntax m && HasIssue(m, out var token):
                {
                    // linq call
                    switch (node.GetName())
                    {
                        case "AreEqual":
                        {
                            return Issue(token);
                        }

                        case "That" when arguments.Count >= 2:
                        {
                            switch (arguments[1].Expression)
                            {
                                case InvocationExpressionSyntax ai when IsFixableAssertionForLinqCall(ai): // we can only fix "Assert.That(xyz.Count(), Is.EqualTo(42)"
                                case MemberAccessExpressionSyntax am when am.Is("Zero"): // we can only fix "Assert.That(xyz.Count(), Is.Zero"
                                    return Issue(token);
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            return null;
        }

        private new Diagnostic Issue(in SyntaxToken token) => Issue(token, new Pair(Constants.AnalyzerCodeFixSharedData.Marker, token.ValueText));
    }
}