using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6031_CodeFixProvider)), Shared]
    public sealed class MiKo_6031_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6031";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ConditionalExpressionSyntax expression)
            {
                var spaces = GetProposedSpaces(issue);

                SyntaxToken questionToken = expression.QuestionToken.WithLeadingSpaces(spaces);

                if (expression.WhenTrue is ObjectCreationExpressionSyntax o && o.Initializer is InitializerExpressionSyntax initializer)
                {
                    var colonToken = expression.ColonToken;
                    var closeBraceToken = initializer.CloseBraceToken;

                    if (colonToken.IsOnSameLineAs(closeBraceToken))
                    {
                        return expression.WithQuestionToken(questionToken)
                                         .WithColonToken(colonToken.WithLeadingEndOfLine().WithAdditionalLeadingSpacesAtEnd(spaces))
                                         .WithWhenTrue(o.WithInitializer(initializer.WithCloseBraceToken(closeBraceToken.WithoutTrailingTrivia()))); // remove spaces after initializer
                    }
                }

                return expression.WithQuestionToken(questionToken)
                                 .WithColonToken(expression.ColonToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}