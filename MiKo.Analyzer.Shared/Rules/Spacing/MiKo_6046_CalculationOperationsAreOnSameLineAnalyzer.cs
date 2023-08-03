using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6046_CalculationOperationsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6046";

        private static readonly SyntaxKind[] BinaryExpressions =
                                                                 {
                                                                     SyntaxKind.AddExpression,
                                                                     SyntaxKind.SubtractExpression,
                                                                     SyntaxKind.MultiplyExpression,
                                                                     SyntaxKind.DivideExpression,
                                                                     SyntaxKind.ModuloExpression,
                                                                     SyntaxKind.LeftShiftExpression,
                                                                     SyntaxKind.RightShiftExpression,
                                                                 };

        public MiKo_6046_CalculationOperationsAreOnSameLineAnalyzer() : base(Id)
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
                if (node.IsStringConcatenation())
                {
                    // ignore string concatenations
                }
                else
                {
                    ReportDiagnostics(context, Issue(node.OperatorToken));
                }
            }
        }
    }
}