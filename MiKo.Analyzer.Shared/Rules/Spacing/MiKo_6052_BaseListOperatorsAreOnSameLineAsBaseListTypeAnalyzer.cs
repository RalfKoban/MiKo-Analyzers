using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6052_BaseListOperatorsAreOnSameLineAsBaseListTypeAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6052";

        public MiKo_6052_BaseListOperatorsAreOnSameLineAsBaseListTypeAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.BaseList);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = (BaseListSyntax)context.Node;
            var firstBaseType = node.Types[0];

            if (firstBaseType.IsMissing)
            {
                // only report if we have some types
                return;
            }

            var colonToken = node.ColonToken;
            var startLine = colonToken.GetStartingLine();
            var baseTypeLine = firstBaseType.GetStartingLine();

            if (startLine != baseTypeLine)
            {
                ReportDiagnostics(context, Issue(colonToken));
            }
        }
    }
}