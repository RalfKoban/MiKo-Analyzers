using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingForEqualsExpressionAnalyzer : MaintainabilityAnalyzer
    {
        protected UsePatternMatchingForEqualsExpressionAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalNotExpression, SyntaxKind.EqualsExpression);

        protected abstract bool IsResponsibleNode(SyntaxKind kind);

        private bool IsResponsibleNode(CSharpSyntaxNode syntax) => syntax != null && IsResponsibleNode(syntax.Kind());

        private void AnalyzeLogicalNotExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (BinaryExpressionSyntax)context.Node;

            if (IsResponsibleNode(node.Right) || IsResponsibleNode(node.Left))
            {
                var location = node.OperatorToken.GetLocation();
                var issue = Issue(string.Empty, location);
                context.ReportDiagnostic(issue);
            }
        }
    }
}