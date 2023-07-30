using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6041_AssignmentsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6041";

        public MiKo_6041_AssignmentsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.EqualsValueClause);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (EqualsValueClauseSyntax)context.Node;

            var startPosition = node.EqualsToken.GetStartPosition();
            var expressionStartPosition = node.Value.GetStartPosition();

            if (startPosition.Line != expressionStartPosition.Line)
            {
                ReportDiagnostics(context, Issue(node.EqualsToken));
            }
        }
    }
}