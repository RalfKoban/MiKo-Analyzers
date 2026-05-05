using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3505_DoNotReturnValueImmediatelyAfterTryCatchBlockAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3505";

        private static readonly SyntaxKind[] ProblematicCatchStatements = { SyntaxKind.ReturnStatement, SyntaxKind.ThrowStatement };

        public MiKo_3505_DoNotReturnValueImmediatelyAfterTryCatchBlockAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeTryStatement, SyntaxKind.TryStatement);

        private static bool HasIssue(CatchClauseSyntax catchClause)
        {
            var statements = catchClause.Block.Statements;

            return statements.Count > 0 && statements.Last().IsAnyKind(ProblematicCatchStatements);
        }

        private void AnalyzeTryStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is TryStatementSyntax node)
            {
                ReportDiagnostics(context, Analyze(node, context.SemanticModel));
            }
        }

        private Diagnostic Analyze(TryStatementSyntax tryCatchBlock, SemanticModel semanticModel)
        {
            if (tryCatchBlock.NextSibling() is ReturnStatementSyntax returnStatement && tryCatchBlock.Catches.All(HasIssue))
            {
                var expression = returnStatement.Expression;

                if (expression is LiteralExpressionSyntax || expression.IsEnum(semanticModel))
                {
                    return Issue(expression);
                }
            }

            return null;
        }
    }
}