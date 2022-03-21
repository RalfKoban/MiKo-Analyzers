using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3111_TestAssertsUseZeroInsteadOfEqualToAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3111";

        public MiKo_3111_TestAssertsUseZeroInsteadOfEqualToAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpressionSyntax, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocationExpressionSyntax(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            if (HasIssue(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }

        private static bool HasIssue(InvocationExpressionSyntax node)
        {
            if (node.Expression.GetName() == "EqualTo")
            {
                var arguments = node.ArgumentList.Arguments;

                if (arguments.Count == 1)
                {
                    if (arguments[0].Expression is LiteralExpressionSyntax literal && literal.Token.ValueText == "0")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}