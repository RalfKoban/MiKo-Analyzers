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
            var properties = diagnostic.Properties;

            if (properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.LineNumber, out var lineNumber)
             && properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.CharacterPosition, out var characterPosition))
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

#pragma warning disable CA1002
        protected static List<SyntaxNodeOrToken> SelfAndDescendantsOnSeparateLines(SyntaxNode node)
        {
            var lines = new HashSet<int>();
            var descendants = node.DescendantNodesAndTokensAndSelf().Where(_ => lines.Add(_.GetStartingLine())).ToList();

            return descendants;
        }
#pragma warning restore CA1002

        protected static SeparatedSyntaxList<T> PlacedOnSameLine<T>(in SeparatedSyntaxList<T> syntax) where T : SyntaxNode
        {
            var updatedItems = syntax.GetWithSeparators()
                                     .Select(_ =>
                                                 {
                                                     if (_.IsNode)
                                                     {
                                                         return PlacedOnSameLine(_.AsNode());
                                                     }

                                                     if (_.IsToken)
                                                     {
                                                         return _.AsToken().WithoutLeadingTrivia().WithTrailingSpace();
                                                     }

                                                     return _;
                                                 });

            return SyntaxFactory.SeparatedList<T>(updatedItems);
        }

        protected static T PlacedOnSameLine<T>(T syntax) where T : SyntaxNode
        {
            switch (syntax)
            {
                case null: return null;
                case ArgumentListSyntax argumentList: return PlacedOnSameLine(argumentList) as T;
                case ArgumentSyntax argument: return PlacedOnSameLine(argument) as T;
                case BinaryExpressionSyntax binary: return PlacedOnSameLine(binary) as T;
                case CasePatternSwitchLabelSyntax patternLabel: return PlacedOnSameLine(patternLabel) as T;
                case CaseSwitchLabelSyntax label: return PlacedOnSameLine(label) as T;
                case ConstantPatternSyntax constantPattern: return PlacedOnSameLine(constantPattern) as T;
                case DeclarationPatternSyntax declaration: return PlacedOnSameLine(declaration) as T;
                case InvocationExpressionSyntax invocation: return PlacedOnSameLine(invocation) as T;
                case IsPatternExpressionSyntax pattern: return PlacedOnSameLine(pattern) as T;
                case MemberAccessExpressionSyntax maes: return PlacedOnSameLine(maes) as T;
                case NameSyntax name: return PlacedOnSameLine(name) as T;
                case ObjectCreationExpressionSyntax creation: return PlacedOnSameLine(creation) as T;
                case SingleVariableDesignationSyntax singleVariable: return PlacedOnSameLine(singleVariable) as T;
                case SwitchExpressionArmSyntax arm: return PlacedOnSameLine(arm) as T;
                case ThrowExpressionSyntax throwExpression: return PlacedOnSameLine(throwExpression) as T;
                case TypeArgumentListSyntax typeArgumentList: return PlacedOnSameLine(typeArgumentList) as T;
                case WhenClauseSyntax clause: return PlacedOnSameLine(clause) as T;
                default:
                    return syntax.WithoutTrivia();
            }
        }

        protected static ArgumentSyntax PlacedOnSameLine(ArgumentSyntax node) => node.WithoutTrivia()
                                                                                     .WithRefKindKeyword(node.RefKindKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                     .WithRefOrOutKeyword(node.RefOrOutKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                     .WithNameColon(PlacedOnSameLine(node.NameColon))
                                                                                     .WithExpression(PlacedOnSameLine(node.Expression));

        protected static ArgumentListSyntax PlacedOnSameLine(ArgumentListSyntax node) => node.WithoutTrivia()
                                                                                             .WithOpenParenToken(node.OpenParenToken.WithoutTrivia())
                                                                                             .WithArguments(PlacedOnSameLine(node.Arguments))
                                                                                             .WithCloseParenToken(node.CloseParenToken.WithoutTrivia());

        protected static BinaryExpressionSyntax PlacedOnSameLine(BinaryExpressionSyntax node) => node.WithoutTrivia()
                                                                                                     .WithLeft(PlacedOnSameLine(node.Left))
                                                                                                     .WithOperatorToken(node.OperatorToken.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                     .WithRight(PlacedOnSameLine(node.Right));

        protected static CasePatternSwitchLabelSyntax PlacedOnSameLine(CasePatternSwitchLabelSyntax node) => node.WithoutTrivia()
                                                                                                                 .WithKeyword(node.Keyword.WithoutTrailingTrivia())
                                                                                                                 .WithPattern(PlacedOnSameLine(node.Pattern).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                                 .WithWhenClause(PlacedOnSameLine(node.WhenClause))
                                                                                                                 .WithColonToken(node.ColonToken.WithoutLeadingTrivia());

        protected static CaseSwitchLabelSyntax PlacedOnSameLine(CaseSwitchLabelSyntax node) => node.WithoutTrivia()
                                                                                                   .WithKeyword(node.Keyword.WithoutTrailingTrivia())
                                                                                                   .WithValue(PlacedOnSameLine(node.Value).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                   .WithColonToken(node.ColonToken.WithoutLeadingTrivia());

        protected static ConstantPatternSyntax PlacedOnSameLine(ConstantPatternSyntax node) => node.WithoutTrivia()
                                                                                                   .WithExpression(PlacedOnSameLine(node.Expression));

        protected static DeclarationPatternSyntax PlacedOnSameLine(DeclarationPatternSyntax node) => node.WithoutTrivia()
                                                                                                         .WithType(node.Type.WithoutTrailingTrivia())
                                                                                                         .WithDesignation(PlacedOnSameLine(node.Designation));

        protected static InvocationExpressionSyntax PlacedOnSameLine(InvocationExpressionSyntax node) => node.WithoutTrivia()
                                                                                                             .WithExpression(PlacedOnSameLine(node.Expression))
                                                                                                             .WithArgumentList(PlacedOnSameLine(node.ArgumentList));

        protected static IsPatternExpressionSyntax PlacedOnSameLine(IsPatternExpressionSyntax node) => node.WithoutTrivia()
                                                                                                           .WithPattern(PlacedOnSameLine(node.Pattern))
                                                                                                           .WithIsKeyword(node.IsKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                           .WithExpression(PlacedOnSameLine(node.Expression));

        protected static MemberAccessExpressionSyntax PlacedOnSameLine(MemberAccessExpressionSyntax node) => node.WithoutTrivia()
                                                                                                                 .WithName(PlacedOnSameLine(node.Name))
                                                                                                                 .WithOperatorToken(node.OperatorToken.WithoutTrivia())
                                                                                                                 .WithExpression(PlacedOnSameLine(node.Expression));

        protected static NameSyntax PlacedOnSameLine(NameSyntax node)
        {
            switch (node)
            {
                // note that 'GenericNameSyntax' inherits from 'SimpleNameSyntax', so we have to check that first
                case GenericNameSyntax genericName:
                {
                    return genericName.WithoutTrivia()
                                      .WithIdentifier(genericName.Identifier.WithoutTrivia())
                                      .WithTypeArgumentList(PlacedOnSameLine(genericName.TypeArgumentList));
                }

                case SimpleNameSyntax simpleName:
                {
                    return simpleName.WithoutTrivia();
                }

                default:
                {
                    return node.WithoutTrivia();
                }
            }
        }

        protected static ObjectCreationExpressionSyntax PlacedOnSameLine(ObjectCreationExpressionSyntax node) => node.WithoutTrivia()
                                                                                                                     .WithNewKeyword(node.NewKeyword.WithoutTrivia())
                                                                                                                     .WithType(PlacedOnSameLine(node.Type).WithLeadingSpace())
                                                                                                                     .WithArgumentList(PlacedOnSameLine(node.ArgumentList))
                                                                                                                     .WithInitializer(PlacedOnSameLine(node.Initializer));

        protected static SingleVariableDesignationSyntax PlacedOnSameLine(SingleVariableDesignationSyntax node) => node.WithoutTrivia()
                                                                                                                       .WithIdentifier(node.Identifier.WithoutTrivia());

        protected static SwitchExpressionArmSyntax PlacedOnSameLine(SwitchExpressionArmSyntax node) => node.WithoutTrailingTrivia()
                                                                                                           .WithEqualsGreaterThanToken(node.EqualsGreaterThanToken.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                           .WithExpression(PlacedOnSameLine(node.Expression))
                                                                                                           .WithWhenClause(PlacedOnSameLine(node.WhenClause))
                                                                                                           .WithPattern(PlacedOnSameLine(node.Pattern));

        protected static ThrowExpressionSyntax PlacedOnSameLine(ThrowExpressionSyntax node) => node.WithoutTrivia()
                                                                                                   .WithThrowKeyword(node.ThrowKeyword.WithoutTrivia())
                                                                                                   .WithExpression(PlacedOnSameLine(node.Expression));

        protected static TypeArgumentListSyntax PlacedOnSameLine(TypeArgumentListSyntax node) => node.WithoutTrivia()
                                                                                                     .WithArguments(PlacedOnSameLine(node.Arguments))
                                                                                                     .WithGreaterThanToken(node.GreaterThanToken.WithoutTrivia())
                                                                                                     .WithLessThanToken(node.LessThanToken.WithoutTrivia());

        protected static WhenClauseSyntax PlacedOnSameLine(WhenClauseSyntax node) => node?.WithoutTrivia()
                                                                                          .WithWhenKeyword(node.WhenKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                          .WithCondition(PlacedOnSameLine(node.Condition));

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

        protected virtual TSyntaxNode GetUpdatedSyntax<TSyntaxNode>(TSyntaxNode node, in int leadingSpaces) where TSyntaxNode : SyntaxNode => node.WithLeadingSpaces(leadingSpaces);

        protected AnonymousObjectCreationExpressionSyntax GetUpdatedSyntax(AnonymousObjectCreationExpressionSyntax node, in int spaces)
        {
            var openBraceToken = node.OpenBraceToken;
            var closeBraceToken = node.CloseBraceToken;

            var closeBraceTokenSpaces = openBraceToken.IsOnSameLineAs(closeBraceToken) ? 0 : spaces;

            return node.WithOpenBraceToken(openBraceToken.WithLeadingSpaces(spaces))
                       .WithInitializers(GetUpdatedSyntax(node.Initializers, openBraceToken, spaces + Constants.Indentation))
                       .WithCloseBraceToken(closeBraceToken.WithLeadingSpaces(closeBraceTokenSpaces));
        }

        protected InitializerExpressionSyntax GetUpdatedSyntax(InitializerExpressionSyntax node, in int spaces)
        {
            var openBraceToken = node.OpenBraceToken;
            var closeBraceToken = node.CloseBraceToken;

            var closeBraceTokenSpaces = openBraceToken.IsOnSameLineAs(closeBraceToken) ? 0 : spaces;

            return node.WithOpenBraceToken(openBraceToken.WithLeadingSpaces(spaces))
                       .WithExpressions(GetUpdatedSyntax(node.Expressions, openBraceToken, spaces + Constants.Indentation))
                       .WithCloseBraceToken(closeBraceToken.WithLeadingSpaces(closeBraceTokenSpaces));
        }
    }
}