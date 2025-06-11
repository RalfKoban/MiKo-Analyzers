using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6045_ComparisonsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6045";

        private static readonly SyntaxKind[] BinaryExpressions =
                                                                 {
                                                                     SyntaxKind.EqualsExpression,
                                                                     SyntaxKind.NotEqualsExpression,
                                                                     SyntaxKind.LessThanExpression,
                                                                     SyntaxKind.LessThanOrEqualExpression,
                                                                     SyntaxKind.GreaterThanExpression,
                                                                     SyntaxKind.GreaterThanOrEqualExpression,
                                                                     SyntaxKind.IsExpression,
                                                                 };

        public MiKo_6045_ComparisonsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, BinaryExpressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;
            var operatorToken = node.OperatorToken;

            var allOnSameLine = operatorToken.IsOnSameLineAs(node.Left) && operatorToken.IsOnSameLineAs(node.Right);

            if (allOnSameLine is false)
            {
                ReportDiagnostics(context, Issue(operatorToken));
            }
        }
    }
}