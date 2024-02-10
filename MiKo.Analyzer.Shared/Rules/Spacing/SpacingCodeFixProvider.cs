using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingCodeFixProvider : MiKoCodeFixProvider
    {
        protected static List<SyntaxNodeOrToken> SelfAndDescendantsOnSeparateLines(SyntaxNode node)
        {
            var lines = new HashSet<int>();
            var descendants = node.DescendantNodesAndTokensAndSelf().Where(_ => lines.Add(_.GetStartingLine())).ToList();

            return descendants;
        }

        protected static StatementSyntax GetUpdatedStatement(StatementSyntax statement, int spaces)
        {
            var syntax = statement.WithLeadingSpaces(spaces);

            var additionalSpaces = syntax.GetPositionWithinStartLine() - statement.GetPositionWithinStartLine();

            // collect all descendant nodes that are the first ones starting on a new line, then adjust leading space for each of those
            var startingNodes = GetNodesAndTokensStartingOnSeparateLines(syntax).ToList();

            if (startingNodes.Count == 0)
            {
                return syntax;
            }

            return syntax.WithAdditionalLeadingSpacesOnDescendants(startingNodes, additionalSpaces);
        }

        protected static BlockSyntax GetUpdatedBlock(BlockSyntax block, int spaces)
        {
            if (block is null)
            {
                return null;
            }

            var indentation = spaces + Constants.Indentation;

            return block.WithOpenBraceToken(block.OpenBraceToken.WithLeadingSpaces(spaces))
                        .WithStatements(block.Statements.Select(_ => GetUpdatedStatement(_, indentation)).ToSyntaxList())
                        .WithCloseBraceToken(block.CloseBraceToken.WithLeadingSpaces(spaces));
        }

        private static IEnumerable<SyntaxNodeOrToken> GetNodesAndTokensStartingOnSeparateLines(SyntaxNode startingNode)
        {
            var currentLine = startingNode.GetStartingLine();

            foreach (var nodeOrToken in startingNode.DescendantNodesAndTokens(_ => true, true))
            {
                var startingLine = nodeOrToken.GetStartingLine();

                if (startingLine != currentLine)
                {
                    currentLine = startingLine;

                    if (nodeOrToken.IsToken)
                    {
                        var token = nodeOrToken.AsToken();

                        switch (token.Kind())
                        {
                            case SyntaxKind.PlusToken when IsStringCreation(token.Parent):
                            {
                                // ignore string constructions via add
                                continue;
                            }

                            case SyntaxKind.CloseParenToken when token.Parent is ArgumentListSyntax l && IsStringCreation(l.Arguments.Last().Expression):
                            {
                                // ignore string constructions via add
                                continue;
                            }
                        }
                    }

                    yield return nodeOrToken;
                }
            }
        }

        private static bool IsStringCreation(SyntaxNode node)
        {
            if (node is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.AddExpression))
            {
                if (b.Left.IsStringLiteral() || b.Right.IsStringLiteral())
                {
                    return true;
                }

                if (IsStringCreation(b.Left) || IsStringCreation(b.Right))
                {
                    return true;
                }
            }

            return false;
        }
    }
}