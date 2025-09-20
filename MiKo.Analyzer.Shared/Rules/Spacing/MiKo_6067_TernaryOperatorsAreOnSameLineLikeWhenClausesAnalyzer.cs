using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6067_TernaryOperatorsAreOnSameLineLikeWhenClausesAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6067";

        public MiKo_6067_TernaryOperatorsAreOnSameLineLikeWhenClausesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ConditionalExpression);

        private static bool HasIssue(in SyntaxToken token, ExpressionSyntax previousExpression, ExpressionSyntax ownWhenCase)
        {
            if (token.IsOnSameLineAsEndOf(previousExpression))
            {
                return token.IsOnSameLineAs(ownWhenCase) is false;
            }

            // when adjusting, also take a look at MiKo_6031
            if (previousExpression is ObjectCreationExpressionSyntax o && o.Initializer is InitializerExpressionSyntax initializer)
            {
                return token.IsOnSameLineAs(initializer.CloseBraceToken);
            }

            return false;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ConditionalExpressionSyntax expression)
            {
                ReportDiagnostics(context, AnalyzeNode(expression));
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(ConditionalExpressionSyntax expression)
        {
            var questionToken = expression.QuestionToken;
            var colonToken = expression.ColonToken;

            var condition = expression.Condition;
            var whenTrue = expression.WhenTrue;
            var whenFalse = expression.WhenFalse;

            if (HasIssue(questionToken, condition, whenTrue))
            {
                yield return Issue(questionToken);
            }

            if (HasIssue(colonToken, whenTrue, whenFalse))
            {
                yield return Issue(colonToken);
            }
        }
    }
}