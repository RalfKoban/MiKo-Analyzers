using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingForBinaryExpressionAnalyzer : UsePatternMatchingForExpressionAnalyzer
    {
        protected UsePatternMatchingForBinaryExpressionAnalyzer(string diagnosticId, SyntaxKind syntaxKind, LanguageVersion languageVersion = LanguageVersion.CSharp7) : base(diagnosticId, syntaxKind, languageVersion)
        {
        }

        protected sealed override void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
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
        }

        protected abstract bool IsResponsibleNode(SyntaxKind kind);

        private bool IsResponsibleNode(CSharpSyntaxNode syntax) => syntax != null && IsResponsibleNode(syntax.Kind());
    }
}