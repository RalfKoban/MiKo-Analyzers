using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6031";

        public MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ConditionalExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ConditionalExpressionSyntax expression)
            {
                ReportDiagnostics(context, AnalyzeNode(expression));
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(ConditionalExpressionSyntax expression)
        {
            var operatorToken = expression.QuestionToken;

            var conditionPosition = expression.Condition.GetStartPosition();
            var operatorPosition = operatorToken.GetStartPosition();

            if (conditionPosition.Line != operatorPosition.Line)
            {
                var colonToken = expression.ColonToken;
                var colonPosition = colonToken.GetStartPosition();

                if (conditionPosition.Character != operatorPosition.Character)
                {
                    yield return Issue(operatorToken, CreateProposalForSpaces(conditionPosition.Character));
                }

                if (conditionPosition.Character != colonPosition.Character)
                {
                    yield return Issue(colonToken, CreateProposalForSpaces(conditionPosition.Character));
                }
            }
        }
    }
}