using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class UsePatternMatchingForBinaryExpressionAnalyzer : UsePatternMatchingForExpressionAnalyzer
    {
        protected UsePatternMatchingForBinaryExpressionAnalyzer(string diagnosticId, in SyntaxKind syntaxKind, LanguageVersion languageVersion = LanguageVersion.CSharp7) : base(diagnosticId, syntaxKind, languageVersion)
        {
        }

        protected sealed override void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
                var semanticModel = context.SemanticModel;

                if (node.IsExpressionTree(semanticModel))
                {
                    // ignore expression trees
                    return;
                }

                if (IsResponsibleNode(node.Right, semanticModel) || IsResponsibleNode(node.Left, semanticModel))
                {
                    ReportIssue(context, node.OperatorToken);
                }
            }
        }

        protected abstract bool IsResponsibleNode(in SyntaxKind kind);

        protected virtual bool IsResponsibleNode(ExpressionSyntax node, SemanticModel semanticModel) => node != null && IsResponsibleNode(node.Kind());
    }
}