using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_6067_CodeFixProvider"/>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6031_CodeFixProvider)), Shared]
    public sealed class MiKo_6031_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6031";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ConditionalExpressionSyntax expression)
            {
                return GetUpdatedSyntax(expression, issue);
            }

            return syntax;
        }

        private static ConditionalExpressionSyntax GetUpdatedSyntax(ConditionalExpressionSyntax expression, Diagnostic issue)
        {
            var questionToken = expression.QuestionToken;
            var colonToken = expression.ColonToken;

            var spaces = GetProposedSpaces(issue);

            // when adjusting, also take a look at MiKo_6067 code fix
            if (expression.WhenTrue is ObjectCreationExpressionSyntax o && o.Initializer is InitializerExpressionSyntax initializer)
            {
                var closeBraceToken = initializer.CloseBraceToken;

                if (colonToken.IsOnSameLineAs(closeBraceToken))
                {
                    return expression.WithQuestionToken(questionToken.WithLeadingSpaces(spaces))
                                     .WithColonToken(colonToken.WithLeadingEndOfLine().WithAdditionalLeadingSpacesAtEnd(spaces))
                                     .WithWhenTrue(o.WithInitializer(initializer.WithCloseBraceToken(closeBraceToken.WithoutTrailingTrivia()))); // remove spaces after initializer
                }
            }

            return expression.WithCondition(expression.Condition.WithoutTrailingTrivia().WithTrailingNewLine())
                             .WithQuestionToken(questionToken.WithLeadingSpaces(spaces).WithTrailingSpace())
                             .WithWhenTrue(expression.WhenTrue.WithoutLeadingTrivia().WithoutTrailingSpaces())
                             .WithColonToken(colonToken.WithLeadingEndOfLine().WithAdditionalLeadingSpacesAtEnd(spaces).WithTrailingSpace())
                             .WithWhenFalse(expression.WhenFalse.WithoutLeadingTrivia().WithoutTrailingSpaces());
        }
    }
}