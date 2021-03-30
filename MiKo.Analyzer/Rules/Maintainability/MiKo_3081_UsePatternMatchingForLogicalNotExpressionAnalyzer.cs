using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer : UsePatternMatchingForExpressionAnalyzer
    {
        public const string Id = "MiKo_3081";

        public MiKo_3081_UsePatternMatchingForLogicalNotExpressionAnalyzer() : base(Id, SyntaxKind.LogicalNotExpression)
        {
        }

        protected override void AnalyzeExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (PrefixUnaryExpressionSyntax)context.Node;

            if (node.IsExpression(context.SemanticModel))
            {
                // ignore expression trees
                return;
            }

            var symbol = context.SemanticModel.GetSymbolInfo(node.Operand).Symbol;
            if (symbol is IFieldSymbol f && f.IsConst)
            {
                // ignore constants
                return;
            }

            ReportIssue(context, node.OperatorToken);
        }
    }
}