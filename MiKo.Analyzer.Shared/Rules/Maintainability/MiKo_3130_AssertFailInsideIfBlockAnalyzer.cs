using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3130_AssertFailInsideIfBlockAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3130";

        public MiKo_3130_AssertFailInsideIfBlockAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            if (ReferencesNUnit(context.Compilation))
            {
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            }
        }

        private static bool HasIssue(InvocationExpressionSyntax assertFail)
        {
            if (assertFail.Parent is ExpressionStatementSyntax statement)
            {
                switch (statement.Parent)
                {
                    case BlockSyntax block when block.Statements.Count is 1 && HasIssue(block.Statements[0], assertFail):
                    case IfStatementSyntax ifStatement when HasIssue(ifStatement.Statement, assertFail):
                    case ElseClauseSyntax elseClause when HasIssue(elseClause.Statement, assertFail):
                        return true;
                }
            }

            return false;
        }

        private static bool HasIssue(StatementSyntax statement, InvocationExpressionSyntax assertFail) => statement is ExpressionStatementSyntax s && s.Expression == assertFail;

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IfStatementSyntax ifStatement)
            {
                var assertFails = ifStatement.DescendantNodes<InvocationExpressionSyntax>(_ => _.Is("Assert", "Fail"));

                foreach (var assertFail in assertFails.Where(HasIssue))
                {
                    ReportDiagnostics(context, Issue(assertFail));
                }
            }
        }
    }
}