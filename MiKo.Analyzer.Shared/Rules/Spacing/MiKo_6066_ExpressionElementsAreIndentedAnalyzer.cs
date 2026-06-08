#if VS2022 || VS2026

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6066_ExpressionElementsAreIndentedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6066";

        public MiKo_6066_ExpressionElementsAreIndentedAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ExpressionElement);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeNode(context.Node as ExpressionElementSyntax);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeNode(ExpressionElementSyntax node)
        {
            if (node.Parent is CollectionExpressionSyntax collection)
            {
                var startPosition = node.GetStartPosition();
                var openBracketPosition = collection.OpenBracketToken.GetStartPosition();

                if (startPosition.Line != openBracketPosition.Line && startPosition.Character <= openBracketPosition.Character)
                {
                    return Issue(node, CreateProposalForSpaces(openBracketPosition.Character + Constants.Indentation));
                }
            }

            return null;
        }
    }
}

#endif