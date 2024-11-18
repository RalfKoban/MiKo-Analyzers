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

        protected static SeparatedSyntaxList<TSyntaxNode> GetUpdatedSyntax<TSyntaxNode>(SeparatedSyntaxList<TSyntaxNode> expressions, int leadingSpaces) where TSyntaxNode : SyntaxNode
        {
            if (expressions.Count == 0)
            {
                return SyntaxFactory.SeparatedList<TSyntaxNode>();
            }

            int? currentLine = null;

            var updatedExpressions = new List<TSyntaxNode>(expressions.Count);

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

        protected static T PlacedOnSameLine<T>(T syntax) where T : SyntaxNode
        {
            switch (syntax)
            {
                case null:
                    return null;

                case CaseSwitchLabelSyntax label:
                    return label.WithoutTrivia()
                                .WithKeyword(label.Keyword.WithoutTrailingTrivia())
                                .WithValue(PlacedOnSameLine(label.Value).WithLeadingSpace().WithoutTrailingTrivia())
                                .WithColonToken(label.ColonToken.WithoutLeadingTrivia()) as T;

                case CasePatternSwitchLabelSyntax patternLabel:
                    return patternLabel.WithoutTrivia()
                                       .WithKeyword(patternLabel.Keyword.WithoutTrailingTrivia())
                                       .WithPattern(PlacedOnSameLine(patternLabel.Pattern).WithLeadingSpace().WithoutTrailingTrivia())
                                       .WithWhenClause(PlacedOnSameLine(patternLabel.WhenClause))
                                       .WithColonToken(patternLabel.ColonToken.WithoutLeadingTrivia()) as T;

                case SwitchExpressionArmSyntax arm:
                    return arm.WithoutTrailingTrivia()
                              .WithEqualsGreaterThanToken(arm.EqualsGreaterThanToken.WithLeadingSpace().WithoutTrailingTrivia())
                              .WithExpression(PlacedOnSameLine(arm.Expression))
                              .WithWhenClause(PlacedOnSameLine(arm.WhenClause))
                              .WithPattern(PlacedOnSameLine(arm.Pattern)) as T;

                case MemberAccessExpressionSyntax maes:
                    return maes.WithoutTrivia()
                               .WithName(PlacedOnSameLine(maes.Name))
                               .WithOperatorToken(maes.OperatorToken.WithoutTrivia())
                               .WithExpression(PlacedOnSameLine(maes.Expression)) as T;

                case BinaryExpressionSyntax binary:
                    return binary.WithoutTrivia()
                                 .WithLeft(PlacedOnSameLine(binary.Left))
                                 .WithOperatorToken(binary.OperatorToken.WithLeadingSpace().WithoutTrailingTrivia())
                                 .WithRight(PlacedOnSameLine(binary.Right)) as T;

                case IsPatternExpressionSyntax pattern:
                    return pattern.WithoutTrivia()
                                  .WithPattern(PlacedOnSameLine(pattern.Pattern))
                                  .WithIsKeyword(pattern.IsKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                  .WithExpression(PlacedOnSameLine(pattern.Expression)) as T;

                case InvocationExpressionSyntax invocation:
                    return invocation.WithoutTrivia()
                                     .WithExpression(PlacedOnSameLine(invocation.Expression))
                                     .WithArgumentList(PlacedOnSameLine(invocation.ArgumentList)) as T;

                case WhenClauseSyntax clause:
                    return clause?.WithoutTrivia()
                                  .WithWhenKeyword(clause.WhenKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                  .WithCondition(PlacedOnSameLine(clause.Condition)) as T;

                case ConstantPatternSyntax constantPattern:
                    return constantPattern.WithoutTrivia()
                                          .WithExpression(PlacedOnSameLine(constantPattern.Expression)) as T;

                case DeclarationPatternSyntax declaration:
                    return declaration.WithoutTrivia()
                                      .WithType(declaration.Type.WithoutTrailingTrivia())
                                      .WithDesignation(PlacedOnSameLine(declaration.Designation)) as T;

                case SingleVariableDesignationSyntax singleVariable:
                    return singleVariable.WithoutTrivia()
                                         .WithIdentifier(singleVariable.Identifier.WithoutTrivia()) as T;

                case ArgumentListSyntax argumentList:
                    return argumentList.WithoutTrivia()
                                       .WithOpenParenToken(argumentList.OpenParenToken.WithoutTrivia())
                                       .WithArguments(PlacedOnSameLine(argumentList.Arguments))
                                       .WithCloseParenToken(argumentList.CloseParenToken.WithoutTrivia()) as T;

                case ArgumentSyntax argument:
                    return argument.WithoutTrivia()
                                   .WithRefKindKeyword(argument.RefKindKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                   .WithRefOrOutKeyword(argument.RefOrOutKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                   .WithNameColon(PlacedOnSameLine(argument.NameColon))
                                   .WithExpression(PlacedOnSameLine(argument.Expression)) as T;

                case GenericNameSyntax genericName:
                    return genericName.WithoutTrivia()
                                      .WithIdentifier(genericName.Identifier.WithoutTrailingTrivia())
                                      .WithTypeArgumentList(PlacedOnSameLine(genericName.TypeArgumentList)) as T;

                case SimpleNameSyntax simpleName:
                    return simpleName.WithoutTrivia() as T;

                case TypeArgumentListSyntax typeArgumentList:
                    return typeArgumentList.WithoutTrivia()
                                           .WithArguments(PlacedOnSameLine(typeArgumentList.Arguments))
                                           .WithGreaterThanToken(typeArgumentList.GreaterThanToken.WithoutTrivia())
                                           .WithLessThanToken(typeArgumentList.LessThanToken.WithoutTrivia()) as T;

                case ThrowExpressionSyntax throwExpression:
                    return throwExpression.WithoutTrivia()
                                          .WithThrowKeyword(throwExpression.ThrowKeyword.WithoutTrivia())
                                          .WithExpression(PlacedOnSameLine(throwExpression.Expression)) as T;

                case ObjectCreationExpressionSyntax creation:
                    return creation.WithoutTrivia()
                                   .WithNewKeyword(creation.NewKeyword.WithoutTrivia())
                                   .WithType(PlacedOnSameLine(creation.Type).WithLeadingSpace())
                                   .WithArgumentList(PlacedOnSameLine(creation.ArgumentList))
                                   .WithInitializer(PlacedOnSameLine(creation.Initializer)) as T;

                default:
                    return syntax.WithoutTrivia();
            }
        }

        protected static SeparatedSyntaxList<T> PlacedOnSameLine<T>(SeparatedSyntaxList<T> syntax) where T : SyntaxNode
        {
            var updatedItems = syntax.GetWithSeparators()
                                     .Select(token =>
                                                     {
                                                         if (token.IsNode)
                                                         {
                                                             return PlacedOnSameLine(token.AsNode());
                                                         }

                                                         if (token.IsToken)
                                                         {
                                                             return token.AsToken().WithoutLeadingTrivia().WithTrailingSpace();
                                                         }

                                                         return token;
                                                     });

            return SyntaxFactory.SeparatedList<T>(updatedItems);
        }
    }
}