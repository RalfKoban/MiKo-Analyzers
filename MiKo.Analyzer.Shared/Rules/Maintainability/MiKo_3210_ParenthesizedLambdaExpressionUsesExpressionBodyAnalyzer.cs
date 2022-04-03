using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3210_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3210";

        public MiKo_3210_ParenthesizedLambdaExpressionUsesExpressionBodyAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        private void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (ParenthesizedLambdaExpressionSyntax)context.Node;
            if (node.ExpressionBody != null)
            {
                // it's already an expression body, so no need to report
                return;
            }

            const int Threshold = 2; // just attempt to take at least 2 statements which would indicate that we have more than 1 statement
            var statements = node.Block.DescendantNodes<StatementSyntax>().Take(Threshold).ToList();

            // simplification works only if it is a single statement
            if (statements.Count == 1)
            {
                switch (statements[0])
                {
                    case EmptyStatementSyntax _:
                    case LocalDeclarationStatementSyntax _:
                    {
                        // we cannot simplify those lambdas
                        return;
                    }
                }

                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}