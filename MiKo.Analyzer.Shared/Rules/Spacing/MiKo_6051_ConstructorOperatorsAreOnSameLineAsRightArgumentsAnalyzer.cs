using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6051";

        public MiKo_6051_ConstructorOperatorsAreOnSameLineAsRightArgumentsAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.BaseConstructorInitializer, SyntaxKind.ThisConstructorInitializer);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (ConstructorInitializerSyntax)context.Node;
            var keyword = node.ThisOrBaseKeyword;

            if (keyword.IsMissing)
            {
                // incomplete code
                return;
            }

            var colonToken = node.ColonToken;
            var startLine = colonToken.GetStartingLine();
            var rightPosition = keyword.GetStartPosition();

            if (startLine != rightPosition.Line)
            {
                ReportDiagnostics(context, Issue(colonToken));
            }
        }
    }
}