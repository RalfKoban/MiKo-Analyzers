using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6034_DotsAreOnSameLineAsInvokedMemberAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6034";

        public MiKo_6034_DotsAreOnSameLineAsInvokedMemberAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var maes = (MemberAccessExpressionSyntax)context.Node;

            var dotLine = maes.OperatorToken.GetStartingLine();
            var memberLine = maes.Name.GetStartingLine();

            if (dotLine != memberLine)
            {
                ReportDiagnostics(context, Issue(maes.OperatorToken));
            }
        }
    }
}