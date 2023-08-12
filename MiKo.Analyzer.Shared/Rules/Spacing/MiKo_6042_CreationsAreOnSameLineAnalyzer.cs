using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6042_CreationsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6042";

        public MiKo_6042_CreationsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ObjectCreationExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var creation = (ObjectCreationExpressionSyntax)context.Node;
            var newKeyword = creation.NewKeyword;

            var startLine = newKeyword.GetStartingLine();
            var expressionLine = creation.Type.GetStartingLine();

            if (startLine != expressionLine)
            {
                ReportDiagnostics(context, Issue(newKeyword));
            }
        }
    }
}