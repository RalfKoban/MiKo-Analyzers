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
        protected static List<SyntaxNodeOrToken> SelfAndDescendantsOnSeparateLines(SyntaxNode syntax)
        {
            var lines = new HashSet<int>();
            var descendants = syntax.DescendantNodesAndTokensAndSelf().Where(_ => lines.Add(_.GetStartingLine())).ToList();

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

        protected static ArgumentSyntax PlacedOnSameLine(ArgumentSyntax syntax) => syntax.WithoutTrivia()
                                                                                         .WithRefKindKeyword(syntax.RefKindKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                         .WithRefOrOutKeyword(syntax.RefOrOutKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                         .WithNameColon(PlacedOnSameLine(syntax.NameColon))
                                                                                         .WithExpression(PlacedOnSameLine(syntax.Expression));

        protected static ArgumentListSyntax PlacedOnSameLine(ArgumentListSyntax syntax) => syntax.WithoutTrivia()
                                                                                                 .WithOpenParenToken(syntax.OpenParenToken.WithoutTrivia())
                                                                                                 .WithArguments(PlacedOnSameLine(syntax.Arguments))
                                                                                                 .WithCloseParenToken(syntax.CloseParenToken.WithoutTrivia());

        protected static BinaryExpressionSyntax PlacedOnSameLine(BinaryExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                         .WithLeft(PlacedOnSameLine(syntax.Left))
                                                                                                         .WithOperatorToken(syntax.OperatorToken.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                         .WithRight(PlacedOnSameLine(syntax.Right));

        protected static CasePatternSwitchLabelSyntax PlacedOnSameLine(CasePatternSwitchLabelSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                     .WithKeyword(syntax.Keyword.WithoutTrailingTrivia())
                                                                                                                     .WithPattern(PlacedOnSameLine(syntax.Pattern).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                                     .WithWhenClause(PlacedOnSameLine(syntax.WhenClause))
                                                                                                                     .WithColonToken(syntax.ColonToken.WithoutLeadingTrivia());

        protected static CaseSwitchLabelSyntax PlacedOnSameLine(CaseSwitchLabelSyntax syntax) => syntax.WithoutTrivia()
                                                                                                       .WithKeyword(syntax.Keyword.WithoutTrailingTrivia())
                                                                                                       .WithValue(PlacedOnSameLine(syntax.Value).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                       .WithColonToken(syntax.ColonToken.WithoutLeadingTrivia());

        protected static ConstantPatternSyntax PlacedOnSameLine(ConstantPatternSyntax syntax) => syntax.WithoutTrivia()
                                                                                                       .WithExpression(PlacedOnSameLine(syntax.Expression));

        protected static DeclarationPatternSyntax PlacedOnSameLine(DeclarationPatternSyntax syntax) => syntax.WithoutTrivia()
                                                                                                             .WithType(syntax.Type.WithoutTrailingTrivia())
                                                                                                             .WithDesignation(PlacedOnSameLine(syntax.Designation));

        protected static InvocationExpressionSyntax PlacedOnSameLine(InvocationExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                 .WithExpression(PlacedOnSameLine(syntax.Expression))
                                                                                                                 .WithArgumentList(PlacedOnSameLine(syntax.ArgumentList));

        protected static IsPatternExpressionSyntax PlacedOnSameLine(IsPatternExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                               .WithPattern(PlacedOnSameLine(syntax.Pattern))
                                                                                                               .WithIsKeyword(syntax.IsKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                               .WithExpression(PlacedOnSameLine(syntax.Expression));

        protected static MemberAccessExpressionSyntax PlacedOnSameLine(MemberAccessExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                     .WithName(PlacedOnSameLine(syntax.Name))
                                                                                                                     .WithOperatorToken(syntax.OperatorToken.WithoutTrivia())
                                                                                                                     .WithExpression(PlacedOnSameLine(syntax.Expression));

        protected static NameSyntax PlacedOnSameLine(NameSyntax syntax)
        {
            switch (syntax)
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
                    return syntax.WithoutTrivia();
                }
            }
        }

        protected static ObjectCreationExpressionSyntax PlacedOnSameLine(ObjectCreationExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                         .WithNewKeyword(syntax.NewKeyword.WithoutTrivia())
                                                                                                                         .WithType(PlacedOnSameLine(syntax.Type).WithLeadingSpace())
                                                                                                                         .WithArgumentList(PlacedOnSameLine(syntax.ArgumentList))
                                                                                                                         .WithInitializer(PlacedOnSameLine(syntax.Initializer));

        protected static SingleVariableDesignationSyntax PlacedOnSameLine(SingleVariableDesignationSyntax syntax) => syntax.WithoutTrivia()
                                                                                                                           .WithIdentifier(syntax.Identifier.WithoutTrivia());

        protected static SwitchExpressionArmSyntax PlacedOnSameLine(SwitchExpressionArmSyntax syntax) => syntax.WithoutTrailingTrivia()
                                                                                                               .WithEqualsGreaterThanToken(syntax.EqualsGreaterThanToken.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                               .WithExpression(PlacedOnSameLine(syntax.Expression))
                                                                                                               .WithWhenClause(PlacedOnSameLine(syntax.WhenClause))
                                                                                                               .WithPattern(PlacedOnSameLine(syntax.Pattern));

        protected static ThrowExpressionSyntax PlacedOnSameLine(ThrowExpressionSyntax syntax) => syntax.WithoutTrivia()
                                                                                                       .WithThrowKeyword(syntax.ThrowKeyword.WithoutTrivia())
                                                                                                       .WithExpression(PlacedOnSameLine(syntax.Expression));

        protected static TypeArgumentListSyntax PlacedOnSameLine(TypeArgumentListSyntax syntax) => syntax.WithoutTrivia()
                                                                                                         .WithArguments(PlacedOnSameLine(syntax.Arguments))
                                                                                                         .WithGreaterThanToken(syntax.GreaterThanToken.WithoutTrivia())
                                                                                                         .WithLessThanToken(syntax.LessThanToken.WithoutTrivia());

        protected static WhenClauseSyntax PlacedOnSameLine(WhenClauseSyntax syntax) => syntax?.WithoutTrivia()
                                                                                              .WithWhenKeyword(syntax.WhenKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                              .WithCondition(PlacedOnSameLine(syntax.Condition));

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

            var closeBraceTokenSpaces = openBraceToken.IsOnSameLineAs(closeBraceToken) ? 0 : spaces;

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

            return syntax.WithOpenBraceToken(updatedOpenBraceToken)
                         .WithExpressions(GetUpdatedSyntax(syntax.Expressions, openBraceToken, spaces + Constants.Indentation))
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

#if VS2022

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