using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6039_ReturnStatementsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6039";

        public MiKo_6039_ReturnStatementsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ReturnStatement);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (ReturnStatementSyntax)context.Node;

            var returnValue = node.Expression;

            if (returnValue != null)
            {
                var startPosition = node.ReturnKeyword.GetStartPosition();
                var returnValueStartPosition = returnValue.GetStartPosition();

                if (startPosition.Line != returnValueStartPosition.Line)
                {
                    ReportDiagnostics(context, Issue(node.ReturnKeyword));
                }
            }
        }
    }
}