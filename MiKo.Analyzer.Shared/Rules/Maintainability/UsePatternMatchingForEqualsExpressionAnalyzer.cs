using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingForEqualsExpressionAnalyzer : UsePatternMatchingForExpressionAnalyzer
    {
        protected UsePatternMatchingForEqualsExpressionAnalyzer(string diagnosticId) : base(diagnosticId, SyntaxKind.EqualsExpression)
        {
        }

        protected sealed override void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            if (node.IsExpression(context.SemanticModel))
            {
                // ignore expression trees
                return;
            }

            if (IsResponsibleNode(node.Right) || IsResponsibleNode(node.Left))
            {
                ReportIssue(context, node.OperatorToken);
            }
        }

        protected abstract bool IsResponsibleNode(SyntaxKind kind);

        private bool IsResponsibleNode(CSharpSyntaxNode syntax) => syntax != null && IsResponsibleNode(syntax.Kind());
    }
}