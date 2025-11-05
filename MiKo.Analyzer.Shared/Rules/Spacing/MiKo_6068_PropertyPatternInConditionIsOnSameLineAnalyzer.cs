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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzePropertyPatternClause, SyntaxKind.RecursivePattern);

        private static bool ShallAnalyze(RecursivePatternSyntax pattern)
        {
            if (pattern.PropertyPatternClause != null)
            {
                if (pattern.FirstAncestor<IfStatementSyntax>() is IfStatementSyntax statement)
                {
                    return statement.Condition.DescendantNodes().Contains(pattern);
                }
            }

            return false;
        }

        private void AnalyzePropertyPatternClause(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is RecursivePatternSyntax pattern && ShallAnalyze(pattern))
            {
                if (pattern.IsSpanningMultipleLines())
                {
                    ReportDiagnostics(context, Issue(pattern));
                }
            }
        }
    }
}