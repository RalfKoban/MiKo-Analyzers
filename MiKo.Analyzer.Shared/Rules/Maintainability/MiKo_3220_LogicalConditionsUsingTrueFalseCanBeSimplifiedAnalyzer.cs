using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3220_LogicalConditionsUsingTrueFalseCanBeSimplifiedAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3220";

        private static readonly SyntaxKind[] LogicalExpressions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        private static readonly SyntaxKind[] TrueFalseExpressions = { SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression };

        public MiKo_3220_LogicalConditionsUsingTrueFalseCanBeSimplifiedAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalExpressions, LogicalExpressions);

        private void AnalyzeLogicalExpressions(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
                var issues = AnalyzeLogicalExpressions(node);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> AnalyzeLogicalExpressions(BinaryExpressionSyntax node)
        {
            if (node.Left is ExpressionSyntax left && left.IsAnyKind(TrueFalseExpressions))
            {
                yield return Issue(left);
            }

            if (node.Right is ExpressionSyntax right && right.IsAnyKind(TrueFalseExpressions))
            {
                yield return Issue(right);
            }
        }
    }
}