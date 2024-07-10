using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3089_DoNotUsePropertyPatternForConditionsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3089";

        public MiKo_3089_DoNotUsePropertyPatternForConditionsAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IfStatementSyntax s && s.Condition is IsPatternExpressionSyntax p && p.Pattern is RecursivePatternSyntax r && r.PropertyPatternClause is PropertyPatternClauseSyntax pattern)
            {
                var subPatterns = pattern.Subpatterns;

                if (subPatterns.Count == 1)
                {
                    var subPattern = subPatterns[0];

                    if (subPattern.Pattern is ConstantPatternSyntax constantPattern && constantPattern.Expression is LiteralExpressionSyntax)
                    {
                        ReportDiagnostics(context, Issue(subPattern));
                    }
                }
            }
        }
    }
}