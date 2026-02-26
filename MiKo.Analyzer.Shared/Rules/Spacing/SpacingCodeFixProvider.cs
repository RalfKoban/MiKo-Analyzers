using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingCodeFixProvider : MiKoCodeFixProvider
    {
        protected static LinePosition GetProposedLinePosition(Diagnostic issue)
        {
            var properties = issue.Properties;

            if (properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.LineNumber, out var lineNumber)
             && properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.CharacterPosition, out var characterPosition))
            {
                return new LinePosition(int.Parse(lineNumber, NumberStyles.Integer), int.Parse(characterPosition, NumberStyles.Integer));
            }

            return LinePosition.Zero;
        }

        protected static int GetProposedSpaces(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.Spaces, out var s)
                                                                    ? int.Parse(s)
                                                                    : 0;

        protected static int GetProposedAdditionalSpaces(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.AdditionalSpaces, out var s)
                                                                              ? int.Parse(s)
                                                                              : 0;

#pragma warning disable CA1002
        protected static List<SyntaxNodeOrToken> SelfAndDescendantsOnSeparateLines(SyntaxNode syntax)
        {
            var lines = new HashSet<int>();
            var descendants = syntax.DescendantNodesAndTokensAndSelf().Where(_ => lines.Add(_.GetStartingLine())).ToList();

            return descendants;
        }
#pragma warning restore CA1002

        protected virtual TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode syntax, in int leadingSpaces) where TSyntaxNode : SyntaxNode => syntax.WithLeadingSpaces(leadingSpaces);

        protected AnonymousObjectCreationExpressionSyntax GetUpdatedSyntax(AnonymousObjectCreationExpressionSyntax syntax, in int spaces)
        {
            var openBraceToken = syntax.OpenBraceToken;
            var closeBraceToken = syntax.CloseBraceToken;

            var onSameLine = openBraceToken.IsOnSameLineAs(closeBraceToken);
            var openSpaces = onSameLine ? 0 : spaces;
            var closeSpaces = onSameLine ? 0 : spaces;

            return syntax.WithOpenBraceToken(openBraceToken.WithLeadingSpaces(openSpaces))
                         .WithInitializers(GetUpdatedSyntax(syntax.Initializers, openBraceToken, spaces + Constants.Indentation))
                         .WithCloseBraceToken(closeBraceToken.WithLeadingSpaces(closeSpaces));
        }

        protected InitializerExpressionSyntax GetUpdatedSyntax(InitializerExpressionSyntax syntax, in int spaces)
        {
            var openBraceToken = syntax.OpenBraceToken;
            var closeBraceToken = syntax.CloseBraceToken;

            var updatedOpenBraceToken = openBraceToken.WithLeadingSpaces(spaces);

            if (syntax.Parent is BaseObjectCreationExpressionSyntax parent)
            {
                if (openBraceToken.IsOnSameLineAs(parent.NewKeyword))
                {
                    if (openBraceToken.IsOnSameLineAs(closeBraceToken))
                    {
                        var openBraceSpaces = parent is ObjectCreationExpressionSyntax creation && openBraceToken.IsOnSameLineAs(creation.Type) ? 0 : 1;

                        updatedOpenBraceToken = updatedOpenBraceToken.WithLeadingSpaces(openBraceSpaces);
                    }
                    else
                    {
                        updatedOpenBraceToken = updatedOpenBraceToken.WithLeadingEmptyLine();
                    }
                }
            }

            int closeBraceTokenSpaces, indentation;

            if (syntax.IsKind(SyntaxKind.ComplexElementInitializerExpression))
            {
                indentation = Constants.IndentationForComplexElementInitializerExpression;
                closeBraceTokenSpaces = syntax.Expressions.LastOrDefault()?.IsOnSameLineAs(closeBraceToken) is true ? 0 : spaces;
            }
            else
            {
                indentation = Constants.Indentation;
                closeBraceTokenSpaces = openBraceToken.IsOnSameLineAs(closeBraceToken) ? 0 : spaces;
            }

            return syntax.WithOpenBraceToken(updatedOpenBraceToken)
                         .WithExpressions(GetUpdatedSyntax(syntax.Expressions, openBraceToken, spaces + indentation))
                         .WithCloseBraceToken(closeBraceToken.WithLeadingSpaces(closeBraceTokenSpaces));
        }

        protected ObjectCreationExpressionSyntax GetUpdatedSyntax(ObjectCreationExpressionSyntax syntax, in int spaces)
        {
            var updatedSyntax = syntax;

            if (syntax.Initializer is InitializerExpressionSyntax initializer)
            {
                var argumentList = syntax.ArgumentList;

                if (argumentList is null)
                {
                    updatedSyntax = syntax.WithInitializer(GetUpdatedSyntax(initializer, spaces));
                }
                else
                {
                    var openParenToken = argumentList.OpenParenToken;
                    var closeParenToken = argumentList.CloseParenToken;
                    var closeSpaces = openParenToken.IsOnSameLineAs(closeParenToken) ? 0 : spaces;

                    var updatedCloseParenToken = closeParenToken.WithLeadingSpaces(closeSpaces)
                                                                .WithoutTrailingTrivia();

                    if (initializer.OpenBraceToken.IsOnSameLineAs(closeParenToken) is false)
                    {
                        updatedCloseParenToken = updatedCloseParenToken.WithTrailingNewLine();
                    }

                    updatedSyntax = syntax.WithArgumentList(argumentList.WithOpenParenToken(openParenToken.WithoutTrivia()).WithCloseParenToken(updatedCloseParenToken))
                                          .WithInitializer(GetUpdatedSyntax(initializer, spaces + Constants.Indentation));
                }
            }

            return updatedSyntax.WithLeadingSpaces(spaces);
        }

        protected ImplicitObjectCreationExpressionSyntax GetUpdatedSyntax(ImplicitObjectCreationExpressionSyntax syntax, in int spaces)
        {
            var updatedSyntax = syntax;

            if (syntax.Initializer is InitializerExpressionSyntax initializer)
            {
                var argumentList = syntax.ArgumentList;

                var openParenToken = argumentList.OpenParenToken;
                var closeParenToken = argumentList.CloseParenToken;
                var closeSpaces = openParenToken.IsOnSameLineAs(closeParenToken) ? 0 : spaces;

                var updatedCloseParenToken = closeParenToken.WithLeadingSpaces(closeSpaces)
                                                            .WithoutTrailingTrivia();

                if (initializer.OpenBraceToken.IsOnSameLineAs(closeParenToken) is false)
                {
                    updatedCloseParenToken = updatedCloseParenToken.WithTrailingNewLine();
                }

                updatedSyntax = syntax.WithArgumentList(argumentList.WithOpenParenToken(openParenToken.WithoutTrivia()).WithCloseParenToken(updatedCloseParenToken))
                                      .WithInitializer(GetUpdatedSyntax(initializer, spaces + Constants.Indentation));
            }

            return updatedSyntax.WithLeadingSpaces(spaces);
        }

#if VS2022 || VS2026

        protected CollectionExpressionSyntax GetUpdatedSyntax(CollectionExpressionSyntax syntax, in int spaces)
        {
            var openBracketToken = syntax.OpenBracketToken;
            var closeBracketToken = syntax.CloseBracketToken;

            var onSameLine = openBracketToken.IsOnSameLineAs(closeBracketToken);
            var openSpaces = onSameLine ? 0 : spaces;
            var closeSpaces = onSameLine ? 0 : spaces;

            return syntax.WithOpenBracketToken(openBracketToken.WithLeadingSpaces(openSpaces))
                         .WithElements(GetUpdatedSyntax(syntax.Elements, openBracketToken, spaces + Constants.Indentation))
                         .WithCloseBracketToken(closeBracketToken.WithLeadingSpaces(closeSpaces));
        }

        protected ExpressionElementSyntax GetUpdatedSyntax(ExpressionElementSyntax syntax, in int spaces) => syntax.WithExpression(GetUpdatedSyntax(syntax.Expression, spaces + Constants.Indentation))
                                                                                                                   .WithLeadingSpaces(spaces);

#endif

        protected SeparatedSyntaxList<TSyntaxNode> GetUpdatedSyntax<TSyntaxNode>(in SeparatedSyntaxList<TSyntaxNode> nodes, in SyntaxToken openBraceToken, in int leadingSpaces) where TSyntaxNode : SyntaxNode
        {
            if (nodes.Count is 0)
            {
                return SyntaxFactory.SeparatedList<TSyntaxNode>();
            }

            int? currentLine = openBraceToken.GetStartingLine();

            var updatedNodes = new List<TSyntaxNode>(nodes.Count);

            foreach (var node in nodes)
            {
                var startingLine = node.GetStartingLine();

                if (currentLine == startingLine)
                {
                    // it is on same line, so do not add any additional space
                    updatedNodes.Add(node);
                }
                else
                {
                    currentLine = startingLine;

                    // it seems to be on a different line, so add with spaces
                    var updatedExpression = GetUpdatedSyntax(node, leadingSpaces);

                    updatedNodes.Add(updatedExpression);
                }
            }

            return SyntaxFactory.SeparatedList(updatedNodes, nodes.GetSeparators());
        }
    }
}