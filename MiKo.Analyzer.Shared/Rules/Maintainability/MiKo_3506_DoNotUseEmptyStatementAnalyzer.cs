using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3506_DoNotUseEmptyStatementAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3506";

        public MiKo_3506_DoNotUseEmptyStatementAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeEmptyStatement, SyntaxKind.EmptyStatement);

        private static bool HasIssue(EmptyStatementSyntax statement)
        {
            switch (statement.Parent)
            {
                case IfStatementSyntax i when i.Statement == statement:
                case DoStatementSyntax d when d.Statement == statement:
                case WhileStatementSyntax w when w.Statement == statement:
                case CommonForEachStatementSyntax c when c.Statement == statement:
                case ForStatementSyntax f when f.Statement == statement:
                case UsingStatementSyntax u when u.Statement == statement:
                case LockStatementSyntax l when l.Statement == statement:
                case FixedStatementSyntax x when x.Statement == statement:
                    return true;

                default:
                    return false;
            }
        }

        private void AnalyzeEmptyStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is EmptyStatementSyntax node && HasIssue(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}