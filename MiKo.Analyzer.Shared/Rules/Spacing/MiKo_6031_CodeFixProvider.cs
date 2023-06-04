using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6031_CodeFixProvider)), Shared]
    public sealed class MiKo_6031_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer.Id;

        protected override string Title => Resources.MiKo_6031_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ConditionalExpressionSyntax expression)
            {
                var position = MiKo_6031_TernaryOperatorsAreOnSamePositionLikeConditionAnalyzer.GetStartPosition(expression);

                var spaces = position.Character;

                return expression.WithQuestionToken(expression.QuestionToken.WithLeadingSpaces(spaces))
                                 .WithColonToken(expression.ColonToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}