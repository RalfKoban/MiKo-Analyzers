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

            var leftLine = node.Left.GetStartingLine();
            var startLine = node.OperatorToken.GetStartingLine();
            var rightLine = node.Right.GetStartingLine();

            if (leftLine != startLine || startLine != rightLine)
            {
                ReportDiagnostics(context, Issue(node.OperatorToken));
            }
        }
    }
}