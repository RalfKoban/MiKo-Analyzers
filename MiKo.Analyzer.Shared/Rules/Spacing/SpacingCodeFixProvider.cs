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

        protected static SeparatedSyntaxList<T> PlacedOnSameLine<T>(SeparatedSyntaxList<T> syntax) where T : SyntaxNode
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

        protected static ArgumentSyntax PlacedOnSameLine(ArgumentSyntax argument) => argument.WithoutTrivia()
                                                                                             .WithRefKindKeyword(argument.RefKindKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                             .WithRefOrOutKeyword(argument.RefOrOutKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                             .WithNameColon(PlacedOnSameLine(argument.NameColon))
                                                                                             .WithExpression(PlacedOnSameLine(argument.Expression));

        protected static ArgumentListSyntax PlacedOnSameLine(ArgumentListSyntax argumentList) => argumentList.WithoutTrivia()
                                                                                                             .WithOpenParenToken(argumentList.OpenParenToken.WithoutTrivia())
                                                                                                             .WithArguments(PlacedOnSameLine(argumentList.Arguments))
                                                                                                             .WithCloseParenToken(argumentList.CloseParenToken.WithoutTrivia());

        protected static BinaryExpressionSyntax PlacedOnSameLine(BinaryExpressionSyntax binary) => binary.WithoutTrivia()
                                                                                                         .WithLeft(PlacedOnSameLine(binary.Left))
                                                                                                         .WithOperatorToken(binary.OperatorToken.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                         .WithRight(PlacedOnSameLine(binary.Right));

        protected static CasePatternSwitchLabelSyntax PlacedOnSameLine(CasePatternSwitchLabelSyntax patternLabel) => patternLabel.WithoutTrivia()
                                                                                                                                 .WithKeyword(patternLabel.Keyword.WithoutTrailingTrivia())
                                                                                                                                 .WithPattern(PlacedOnSameLine(patternLabel.Pattern).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                                                 .WithWhenClause(PlacedOnSameLine(patternLabel.WhenClause))
                                                                                                                                 .WithColonToken(patternLabel.ColonToken.WithoutLeadingTrivia());

        protected static CaseSwitchLabelSyntax PlacedOnSameLine(CaseSwitchLabelSyntax label) => label.WithoutTrivia()
                                                                                                     .WithKeyword(label.Keyword.WithoutTrailingTrivia())
                                                                                                     .WithValue(PlacedOnSameLine(label.Value).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                     .WithColonToken(label.ColonToken.WithoutLeadingTrivia());

        protected static ConstantPatternSyntax PlacedOnSameLine(ConstantPatternSyntax constantPattern) => constantPattern.WithoutTrivia()
                                                                                                                         .WithExpression(PlacedOnSameLine(constantPattern.Expression));

        protected static DeclarationPatternSyntax PlacedOnSameLine(DeclarationPatternSyntax declaration) => declaration.WithoutTrivia()
                                                                                                                       .WithType(declaration.Type.WithoutTrailingTrivia())
                                                                                                                       .WithDesignation(PlacedOnSameLine(declaration.Designation));

        protected static InvocationExpressionSyntax PlacedOnSameLine(InvocationExpressionSyntax invocation) => invocation.WithoutTrivia()
                                                                                                                         .WithExpression(PlacedOnSameLine(invocation.Expression))
                                                                                                                         .WithArgumentList(PlacedOnSameLine(invocation.ArgumentList));

        protected static IsPatternExpressionSyntax PlacedOnSameLine(IsPatternExpressionSyntax pattern) => pattern.WithoutTrivia()
                                                                                                                 .WithPattern(PlacedOnSameLine(pattern.Pattern))
                                                                                                                 .WithIsKeyword(pattern.IsKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                                 .WithExpression(PlacedOnSameLine(pattern.Expression));

        protected static MemberAccessExpressionSyntax PlacedOnSameLine(MemberAccessExpressionSyntax maes) => maes.WithoutTrivia()
                                                                                                                 .WithName(PlacedOnSameLine(maes.Name))
                                                                                                                 .WithOperatorToken(maes.OperatorToken.WithoutTrivia())
                                                                                                                 .WithExpression(PlacedOnSameLine(maes.Expression));

        protected static NameSyntax PlacedOnSameLine(NameSyntax name)
        {
            switch (name)
            {
                // note that 'GenericNameSyntax' inherits from 'SimpleNameSyntax', so we have to check that first
                case GenericNameSyntax genericName:
                    return genericName.WithoutTrivia()
                                      .WithIdentifier(genericName.Identifier.WithoutTrivia())
                                      .WithTypeArgumentList(PlacedOnSameLine(genericName.TypeArgumentList));

                case SimpleNameSyntax simpleName:
                    return simpleName.WithoutTrivia();

                default:
                    return name.WithoutTrivia();
            }
        }

        protected static ObjectCreationExpressionSyntax PlacedOnSameLine(ObjectCreationExpressionSyntax creation) => creation.WithoutTrivia()
                                                                                                                             .WithNewKeyword(creation.NewKeyword.WithoutTrivia())
                                                                                                                             .WithType(PlacedOnSameLine(creation.Type).WithLeadingSpace())
                                                                                                                             .WithArgumentList(PlacedOnSameLine(creation.ArgumentList))
                                                                                                                             .WithInitializer(PlacedOnSameLine(creation.Initializer));

        protected static SingleVariableDesignationSyntax PlacedOnSameLine(SingleVariableDesignationSyntax singleVariable) => singleVariable.WithoutTrivia()
                                                                                                                                           .WithIdentifier(singleVariable.Identifier.WithoutTrivia());

        protected static SwitchExpressionArmSyntax PlacedOnSameLine(SwitchExpressionArmSyntax arm) => arm.WithoutTrailingTrivia()
                                                                                                         .WithEqualsGreaterThanToken(arm.EqualsGreaterThanToken.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                         .WithExpression(PlacedOnSameLine(arm.Expression))
                                                                                                         .WithWhenClause(PlacedOnSameLine(arm.WhenClause))
                                                                                                         .WithPattern(PlacedOnSameLine(arm.Pattern));

        protected static ThrowExpressionSyntax PlacedOnSameLine(ThrowExpressionSyntax throwExpression) => throwExpression.WithoutTrivia()
                                                                                                                         .WithThrowKeyword(throwExpression.ThrowKeyword.WithoutTrivia())
                                                                                                                         .WithExpression(PlacedOnSameLine(throwExpression.Expression));

        protected static TypeArgumentListSyntax PlacedOnSameLine(TypeArgumentListSyntax typeArgumentList) => typeArgumentList.WithoutTrivia()
                                                                                                                             .WithArguments(PlacedOnSameLine(typeArgumentList.Arguments))
                                                                                                                             .WithGreaterThanToken(typeArgumentList.GreaterThanToken.WithoutTrivia())
                                                                                                                             .WithLessThanToken(typeArgumentList.LessThanToken.WithoutTrivia());

        protected static WhenClauseSyntax PlacedOnSameLine(WhenClauseSyntax clause) => clause?.WithoutTrivia()
                                                                                              .WithWhenKeyword(clause.WhenKeyword.WithLeadingSpace().WithoutTrailingTrivia())
                                                                                              .WithCondition(PlacedOnSameLine(clause.Condition));

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
    }
}