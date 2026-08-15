using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3127";

        public MiKo_3127_AssertThatDoesNotUseLiteralForActualAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        // !!! Attention !!!:
        // Visual Studio will not allow the code fix to show up in case it is not for a location within the analyzed syntax node.
        // So, we have to register for the invocation here (instead of the simple member access) as we are interested in reporting the element of the contained argument (actually it's expression).
        // Otherwise, when we would register for the SimpleMemberAccessExpression, the argument would not belong to that access (it belongs to the invocation), and therefore it will be ignored by Visual Studio.
        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation && invocation.Is("Assert", "That"))
            {
                var issue = AnalyzeInvocationExpression(invocation, context.SemanticModel);

                if (issue != null)
                {
                    ReportDiagnostics(context, issue);
                }
            }
        }

        private Diagnostic AnalyzeInvocationExpression(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count > 0)
            {
                var expression = arguments[0].Expression;

                switch (expression)
                {
                    case PrefixUnaryExpressionSyntax unary when unary.Operand is LiteralExpressionSyntax:
                    case LiteralExpressionSyntax _:
                    case MemberAccessExpressionSyntax maes when maes.IsEnum(semanticModel):
                        return Issue(expression);
                }
            }

            return null;
        }
    }
}