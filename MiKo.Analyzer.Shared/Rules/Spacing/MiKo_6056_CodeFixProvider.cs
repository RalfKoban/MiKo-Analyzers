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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<CollectionExpressionSyntax>().FirstOrDefault();

        protected override TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode node, in int leadingSpaces)
        {
            switch (node)
            {
                case InitializerExpressionSyntax initializer when initializer.OpenBraceToken.IsOnSameLineAs(initializer.CloseBraceToken) is false:
                {
                    return GetUpdatedSyntax(initializer, leadingSpaces) as TSyntaxNode;
                }

                case CollectionExpressionSyntax expression when expression.OpenBracketToken.IsOnSameLineAs(expression.CloseBracketToken) is false:
                {
                    return GetUpdatedSyntax(expression, leadingSpaces) as TSyntaxNode;
                }

                case ExpressionElementSyntax element:
                {
                    return GetUpdatedSyntax(element, leadingSpaces) as TSyntaxNode;
                }

                case AnonymousObjectCreationExpressionSyntax anonymous when anonymous.OpenBraceToken.IsOnSameLineAs(anonymous.CloseBraceToken) is false:
                {
                    return GetUpdatedSyntax(anonymous, leadingSpaces) as TSyntaxNode;
                }

                case ObjectCreationExpressionSyntax creation:
                {
                    return GetUpdatedSyntax(creation, leadingSpaces) as TSyntaxNode;
                }

                case ImplicitObjectCreationExpressionSyntax creation:
                {
                    return GetUpdatedSyntax(creation, leadingSpaces - Constants.Indentation) as TSyntaxNode;
                }

                default:
                {
                    return base.GetUpdatedSyntax(node, leadingSpaces);
                }
            }
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax, issue);

            return updatedSyntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is CollectionExpressionSyntax expression && expression.Parent is ArgumentSyntax argument && argument.Parent is ArgumentListSyntax argumentList && argumentList.Arguments.IndexOf(argument) is 0)
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

        private SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is CollectionExpressionSyntax expression)
            {
                return GetUpdatedSyntax(expression, issue);
            }

            return syntax;
        }

        private CollectionExpressionSyntax GetUpdatedSyntax(CollectionExpressionSyntax expression, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            var issueLocation = issue.Location;

            if (expression.OpenBracketToken.IsLocatedAt(issueLocation))
            {
                return GetUpdatedSyntax(expression, spaces);
            }

            if (expression.CloseBracketToken.IsLocatedAt(issueLocation))
            {
                CollectionExpressionSyntax updatedExpression;

                var elements = expression.Elements;

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

                return updatedExpression.WithCloseBracketToken(expression.CloseBracketToken.WithLeadingSpaces(spaces));
            }

            return expression;
        }
    }
}

#endif
