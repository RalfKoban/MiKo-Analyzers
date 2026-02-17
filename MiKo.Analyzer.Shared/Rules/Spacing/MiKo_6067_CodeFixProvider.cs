using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_6031_CodeFixProvider"/>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6067_CodeFixProvider)), Shared]
    public sealed class MiKo_6067_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6067";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is ConditionalExpressionSyntax expression)
            {
                return GetUpdatedSyntax(expression, expression.Condition, expression.QuestionToken, expression.WhenTrue, expression.ColonToken, expression.WhenFalse);
            }

            return syntax;
        }

        private static ConditionalExpressionSyntax GetUpdatedSyntax(
                                                                ConditionalExpressionSyntax expression,
                                                                ExpressionSyntax originalCondition,
                                                                in SyntaxToken originalQuestionToken,
                                                                ExpressionSyntax originalWhenTrue,
                                                                in SyntaxToken originalColonToken,
                                                                ExpressionSyntax originalWhenFalse)
        {
            var updatedCondition = originalCondition;
            var updatedQuestionToken = originalQuestionToken;
            var updatedColonToken = originalColonToken;
            var updatedWhenTrue = originalWhenTrue;

            if (originalQuestionToken.IsOnSameLineAsEndOf(originalCondition))
            {
                updatedCondition = updatedCondition.WithoutTrailingTrivia();

                updatedQuestionToken = updatedQuestionToken.WithLeadingEndOfLine()
                                                           .WithAdditionalLeadingSpacesAtEnd(originalWhenTrue.GetPositionWithinStartLine())
                                                           .WithTrailingSpace();
            }

            if (originalColonToken.IsOnSameLineAsEndOf(originalWhenTrue))
            {
                updatedWhenTrue = updatedWhenTrue.WithoutTrailingTrivia();

                updatedColonToken = updatedColonToken.WithLeadingEndOfLine()
                                                     .WithAdditionalLeadingSpacesAtEnd(originalWhenFalse.GetPositionWithinStartLine())
                                                     .WithTrailingSpace();
            }
            else
            {
                // when adjusting, also take a look at MiKo_6031 code fix
                if (updatedWhenTrue is ObjectCreationExpressionSyntax o && o.Initializer is InitializerExpressionSyntax initializer)
                {
                    var closeBraceToken = initializer.CloseBraceToken;

                    if (originalColonToken.IsOnSameLineAsEndOf(closeBraceToken))
                    {
                        updatedWhenTrue = updatedWhenTrue.ReplaceToken(closeBraceToken, closeBraceToken.WithoutTrailingTrivia());

                        updatedColonToken = updatedColonToken.WithLeadingEndOfLine()
                                                             .WithAdditionalLeadingSpacesAtEnd(originalWhenFalse.GetPositionWithinStartLine())
                                                             .WithTrailingSpace();
                    }
                }
            }

            var updatedExpression = expression.WithCondition(updatedCondition)
                                              .WithQuestionToken(updatedQuestionToken)
                                              .WithWhenTrue(updatedWhenTrue.WithoutLeadingTrivia())
                                              .WithColonToken(updatedColonToken)
                                              .WithWhenFalse(originalWhenFalse.WithoutLeadingTrivia());

            return updatedExpression;
        }
    }
}