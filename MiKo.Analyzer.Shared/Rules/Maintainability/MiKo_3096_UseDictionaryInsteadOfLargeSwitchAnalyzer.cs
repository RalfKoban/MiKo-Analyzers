using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3096";

        private const int MinimumCases = 7;

        private static readonly SyntaxKind[] AllowedSwitchExpressionArmKinds = { SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ThrowExpression };

        public MiKo_3096_UseDictionaryInsteadOfLargeSwitchAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement, SyntaxKind.SwitchExpression);

        private static bool IsAcceptable(StatementSyntax syntax)
        {
            switch (syntax)
            {
                case ReturnStatementSyntax statement when statement.Expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) is true:
                    return true;

                case ThrowStatementSyntax _:
                    return true;

                default:
                    return false;
            }
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case SwitchStatementSyntax statement:
                    AnalyzeSwitchStatement(context, statement);

                    break;

                case SwitchExpressionSyntax expression:
                    AnalyzeSwitchExpression(context, expression);

                    break;
            }
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context, SwitchStatementSyntax syntax)
        {
            var sections = syntax.Sections;

            if (sections.Count > MinimumCases && sections.All(_ => _.Statements.Count == 1 && IsAcceptable(_.Statements[0])))
            {
                ReportDiagnostics(context, Issue(syntax));
            }
        }

        private void AnalyzeSwitchExpression(SyntaxNodeAnalysisContext context, SwitchExpressionSyntax syntax)
        {
            var arms = syntax.Arms;

            if (arms.Count > MinimumCases && arms.All(_ => _.Expression.IsAnyKind(AllowedSwitchExpressionArmKinds)))
            {
                ReportDiagnostics(context, Issue(syntax));
            }
        }
    }
}