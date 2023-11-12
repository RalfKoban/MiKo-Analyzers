using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6038_CastsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6038";

        public MiKo_6038_CastsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CastExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var cast = (CastExpressionSyntax)context.Node;

            var startLine = cast.OpenParenToken.GetStartingLine();
            var expressionLine = cast.Expression.GetStartingLine();

            if (startLine != expressionLine)
            {
                ReportDiagnostics(context, Issue(cast));
            }
        }
    }
}