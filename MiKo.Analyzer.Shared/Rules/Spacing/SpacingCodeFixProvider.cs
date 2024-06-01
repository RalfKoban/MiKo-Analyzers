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
        protected static LinePosition GetProposedLinePosition(Diagnostic diagnostic)
        {
            if (diagnostic.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.LineNumber, out var lineNumber)
             && diagnostic.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.CharacterPosition, out var characterPosition))
            {
                return new LinePosition(int.Parse(lineNumber, NumberStyles.Integer), int.Parse(characterPosition, NumberStyles.Integer));
            }

            return LinePosition.Zero;
        }

        protected static int GetProposedSpaces(Diagnostic diagnostic) => diagnostic.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.Spaces, out var s)
                                                                         ? int.Parse(s)
                                                                         : 0;

        protected static int GetProposedAdditionalSpaces(Diagnostic diagnostic) => diagnostic.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.AdditionalSpaces, out var s)
                                                                                   ? int.Parse(s)
                                                                                   : 0;

        protected static List<SyntaxNodeOrToken> SelfAndDescendantsOnSeparateLines(SyntaxNode node)
        {
            var lines = new HashSet<int>();
            var descendants = node.DescendantNodesAndTokensAndSelf().Where(_ => lines.Add(_.GetStartingLine())).ToList();

            return descendants;
        }

        protected static TSyntaxNode GetSyntaxWithLeadingSpaces<TSyntaxNode>(TSyntaxNode syntaxNode, int spaces) where TSyntaxNode : SyntaxNode
        {
            var syntax = syntaxNode.WithLeadingSpaces(spaces);

            var additionalSpaces = syntax.GetPositionWithinStartLine() - syntaxNode.GetPositionWithinStartLine();

            // collect all descendant nodes that are the first ones starting on a new line, then adjust leading space for each of those
            var startingNodes = GetNodesAndTokensStartingOnSeparateLines(syntax).ToList();

            if (startingNodes.Count == 0)
            {
                return syntax;
            }

            return syntax.WithAdditionalLeadingSpacesOnDescendants(startingNodes, additionalSpaces);
        }

        protected static StatementSyntax GetUpdatedStatement(StatementSyntax statement, int spaces) => GetSyntaxWithLeadingSpaces(statement, spaces);

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

        protected static ExpressionSyntax GetUpdatedExpressionPlacedOnSameLine(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax maes:
                {
                    return maes.WithName((SimpleNameSyntax)GetUpdatedExpressionPlacedOnSameLine(maes.Name));
                }

                case GenericNameSyntax genericName:
                {
                    var types = genericName.TypeArgumentList;
                    var arguments = types.Arguments;

                    var separators = Enumerable.Repeat(arguments.GetSeparator(0).WithoutTrivia().WithTrailingSpace(), arguments.Count - 1);

                    var updatedTypes = types.WithoutTrivia()
                                            .WithArguments(SyntaxFactory.SeparatedList(arguments.Select(_ => _.WithoutTrivia()), separators))
                                            .WithGreaterThanToken(types.GreaterThanToken.WithoutTrivia())
                                            .WithLessThanToken(types.LessThanToken.WithoutTrivia());

                    return genericName.WithIdentifier(genericName.Identifier.WithoutTrailingTrivia())
                                      .WithTypeArgumentList(updatedTypes);
                }

                default:
                {
                    return expression;
                }
            }
        }

        protected static SeparatedSyntaxList<TSyntaxNode> GetUpdatedSyntax<TSyntaxNode>(SeparatedSyntaxList<TSyntaxNode> expressions, int leadingSpaces) where TSyntaxNode : SyntaxNode
        {
            if (expressions.Count == 0)
            {
                return SyntaxFactory.SeparatedList<TSyntaxNode>();
            }

            int? currentLine = null;

            var updatedExpressions = new List<TSyntaxNode>();

            foreach (var expression in expressions)
            {
                var startingLine = expression.GetStartingLine();

                if (currentLine == startingLine)
                {
                    // it is on same line, so do not add any additional space
                    updatedExpressions.Add(expression);
                }
                else
                {
                    currentLine = startingLine;

                    // it seems to be on a different line, so add with spaces
                    updatedExpressions.Add(expression.WithLeadingSpaces(leadingSpaces));
                }
            }

            return SyntaxFactory.SeparatedList(updatedExpressions, expressions.GetSeparators());
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