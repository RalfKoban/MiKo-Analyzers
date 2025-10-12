using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_6068_PropertyPatternInConditionIsOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6068";

        public MiKo_6068_PropertyPatternInConditionIsOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzePropertyPatternClause, SyntaxKind.PropertyPatternClause);

        private static bool ShallAnalyze(SyntaxNode clause)
        {
            if (clause.FirstAncestor<IfStatementSyntax>() is IfStatementSyntax statement)
            {
                return statement.Condition.DescendantNodes().Contains(clause);
            }

            return false;
        }

        private static bool HasIssue(SyntaxNode clause) => clause.IsSpanningMultipleLines();

        private void AnalyzePropertyPatternClause(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is PropertyPatternClauseSyntax clause && ShallAnalyze(clause))
            {
                if (HasIssue(clause))
                {
                    ReportDiagnostics(context, Issue(clause));
                }
            }
        }
    }
}