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

        private static bool HasIssue(ExpressionSyntax expression, out SyntaxToken token)
        {
            var syntax = expression is InvocationExpressionSyntax i ? i.Expression : expression;

            if (syntax is MemberAccessExpressionSyntax m)
            {
                return HasIssue(m, out token);
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
                foreach (var argument in methodCall.ArgumentList.Arguments)
                {
                    if (HasIssue(argument.Expression, out var token))
                    {
                        var methodName = context.GetEnclosingMethod()?.Name;

                        yield return Issue(methodName, token, token.ValueText);
                    }
                }
            }
        }
    }
}