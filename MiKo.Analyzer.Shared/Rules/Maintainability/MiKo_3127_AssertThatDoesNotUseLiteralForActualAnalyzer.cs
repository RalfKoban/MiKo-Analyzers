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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        private static bool IsAssertThat(MemberAccessExpressionSyntax node) => node.Expression is IdentifierNameSyntax invokedType
                                                                            && invokedType.GetName() is "Assert"
                                                                            && node.GetName() is "That";

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberAccessExpressionSyntax maes)
            {
                var issue = AnalyzeSimpleMemberAccessExpression(maes, context.SemanticModel);

                if (issue != null)
                {
                    ReportDiagnostics(context, issue);
                }
            }
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.Parent is InvocationExpressionSyntax methodCall && IsAssertThat(node))
            {
                var arguments = methodCall.ArgumentList.Arguments;

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
            }

            return null;
        }
    }
}