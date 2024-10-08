using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3225_LogicalConditionsAreTheSameAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3225";

        private static readonly SyntaxKind[] LogicalConditions = { SyntaxKind.LogicalOrExpression, SyntaxKind.LogicalAndExpression };

        public MiKo_3225_LogicalConditionsAreTheSameAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLogicalExpressions, LogicalConditions);

        private static bool HasIssue(BinaryExpressionSyntax expression) => expression.Left.WithoutParenthesis().ToString() == expression.Right.WithoutParenthesis().ToString();

        private void AnalyzeLogicalExpressions(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax node)
            {
                if (HasIssue(node))
                {
                    ReportDiagnostics(context, Issue(node));
                }
            }
        }
    }
}