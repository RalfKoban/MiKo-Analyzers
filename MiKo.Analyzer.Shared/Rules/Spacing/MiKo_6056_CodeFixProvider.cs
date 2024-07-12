#if VS2022

using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6056_CodeFixProvider)), Shared]
    public sealed class MiKo_6056_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6056";

        protected override string Title => Resources.MiKo_6056_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<CollectionExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is CollectionExpressionSyntax expression)
            {
                var spaces = GetProposedSpaces(issue);

                var issueLocation = issue.Location;

                var openBracketToken = expression.OpenBracketToken;
                var closeBracketToken = expression.CloseBracketToken;
                var elements = expression.Elements;

                if (openBracketToken.IsLocatedAt(issueLocation))
                {
                    return expression.WithOpenBracketToken(openBracketToken.WithLeadingSpaces(spaces))
                                     .WithElements(GetUpdatedSyntax(elements, spaces + Constants.Indentation))
                                     .WithCloseBracketToken(closeBracketToken.WithLeadingSpaces(spaces));
                }

                if (closeBracketToken.IsLocatedAt(issueLocation))
                {
                    CollectionExpressionSyntax updatedExpression;

                    if (elements.SeparatorCount == elements.Count)
                    {
                        // we have a separator at the last element
                        var last = elements.GetSeparators().Last();

                        updatedExpression = expression.WithElements(elements.ReplaceSeparator(last, last.WithTrailingNewLine()));
                    }
                    else
                    {
                        var last = elements.Last();

                        updatedExpression = expression.WithElements(elements.Replace(last, last.WithTrailingNewLine()));
                    }

                    return updatedExpression.WithCloseBracketToken(closeBracketToken.WithLeadingSpaces(spaces));
                }
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is CollectionExpressionSyntax expression && expression.Parent is ArgumentSyntax argument && argument.Parent is ArgumentListSyntax argumentList && argumentList.Arguments.IndexOf(argument) == 0)
            {
                var updatedArgumentList = argumentList.ReplaceNode(argument, argument.WithoutLeadingTrivia());

                var openParenToken = updatedArgumentList.OpenParenToken;

                if (openParenToken.HasTrailingTrivia)
                {
                    updatedArgumentList = updatedArgumentList.WithOpenParenToken(openParenToken.WithoutTrailingTrivia());
                }

                return root.ReplaceNode(argumentList, updatedArgumentList);
            }

            return base.GetUpdatedSyntaxRoot(document, root, syntax, annotationOfSyntax, issue);
        }
    }
}

#endif
