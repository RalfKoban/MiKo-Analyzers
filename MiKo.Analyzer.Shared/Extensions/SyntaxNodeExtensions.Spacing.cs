using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s that are related to spacing.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        /// <summary>
        /// Gets the ending line number of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the ending line for.
        /// </param>
        /// <returns>
        /// The ending line number of the syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetEndingLine(this SyntaxNode value) => value.GetLocation().GetEndingLine();

        /// <summary>
        /// Gets the end position of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the end position for.
        /// </param>
        /// <returns>
        /// The end position of the syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetEndPosition(this SyntaxNode value) => value.GetLocation().GetEndPosition();

        /// <summary>
        /// Gets the position within the ending line of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the position for.
        /// </param>
        /// <returns>
        /// The position within the ending line of the syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinEndLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinEndLine();

        /// <summary>
        /// Gets the position within the starting line of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the position for.
        /// </param>
        /// <returns>
        /// The position within the starting line of the syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetPositionWithinStartLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinStartLine();

        /// <summary>
        /// Gets the starting line number of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the starting line for.
        /// </param>
        /// <returns>
        /// The starting line number of the syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int GetStartingLine(this SyntaxNode value) => value.GetLocation().GetStartingLine();

        /// <summary>
        /// Gets the start position of the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the start position for.
        /// </param>
        /// <returns>
        /// The start position of the syntax node.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static LinePosition GetStartPosition(this SyntaxNode value) => value.GetLocation().GetStartPosition();

        /// <summary>
        /// Determines whether the specified syntax node has a leading empty line.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has a leading empty line; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasLeadingEmptyLine(this SyntaxNode value)
        {
            var trivia = value.GetLeadingTrivia();

            if (trivia.Count > 1)
            {
                if (trivia[0].IsEndOfLine())
                {
                    return true;
                }

                if (trivia[0].IsWhiteSpace() && trivia.Count > 1 && trivia[1].IsEndOfLine())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified syntax node has a trailing end-of-line.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has a trailing end-of-line; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool HasTrailingEndOfLine(this SyntaxNode value) => value != null && value.GetTrailingTrivia().HasEndOfLine();

        /// <summary>
        /// Determines whether the specified syntax node is on the same line as another syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="other">
        /// The other syntax node to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if both nodes are on the same line; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAs(this SyntaxNode value, SyntaxNode other) => value?.GetStartingLine() == other?.GetStartingLine();

        /// <summary>
        /// Determines whether the specified syntax node is on the same line as a syntax token.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="other">
        /// The syntax token to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the node and token are on the same line; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAs(this SyntaxNode value, in SyntaxToken other) => value?.GetStartingLine() == other.GetStartingLine();

        /// <summary>
        /// Determines whether the specified syntax node is on the same line as a syntax node or token.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="other">
        /// The syntax node or token to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if both are on the same line; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsOnSameLineAs(this SyntaxNode value, in SyntaxNodeOrToken other) => value?.GetStartingLine() == other.GetStartingLine();

        /// <summary>
        /// Creates a new separated syntax list with all its items and separators placed on the same line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax nodes in the list.
        /// </typeparam>
        /// <param name="values">
        /// The separated syntax list to modify.
        /// </param>
        /// <returns>
        /// A collection of syntax nodes that contains all items and separators placed on the same line.
        /// </returns>
        internal static SeparatedSyntaxList<T> PlacedOnSameLine<T>(this in SeparatedSyntaxList<T> values) where T : SyntaxNode
        {
            var updatedItems = values.GetWithSeparators()
                                     .Select(_ =>
                                                 {
                                                     if (_.IsNode)
                                                     {
                                                         return PlacedOnSameLine(_.AsNode());
                                                     }

                                                     if (_.IsToken)
                                                     {
                                                         return _.AsToken().WithoutTrivia().WithTrailingSpace();
                                                     }

                                                     return _;
                                                 });

            return SyntaxFactory.SeparatedList<T>(updatedItems);
        }

        /// <summary>
        /// Creates a new syntax node with all its components placed on the same line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to modify.
        /// </param>
        /// <returns>
        /// A new syntax node with all its components placed on the same line.
        /// </returns>
        internal static T PlacedOnSameLine<T>(this T value) where T : SyntaxNode
        {
            switch (value)
            {
                case null: return null;
                case ArgumentListSyntax argumentList: return PlacedOnSameLine(argumentList) as T;
                case ArgumentSyntax argument: return PlacedOnSameLine(argument) as T;
                case BinaryExpressionSyntax binary: return PlacedOnSameLine(binary) as T;
                case CasePatternSwitchLabelSyntax patternLabel: return PlacedOnSameLine(patternLabel) as T;
                case CaseSwitchLabelSyntax label: return PlacedOnSameLine(label) as T;
                case ConditionalExpressionSyntax conditional: return PlacedOnSameLine(conditional) as T;
                case IfStatementSyntax ifStatement: return PlacedOnSameLine(ifStatement) as T;
                case InvocationExpressionSyntax invocation: return PlacedOnSameLine(invocation) as T;
                case IsPatternExpressionSyntax pattern: return PlacedOnSameLine(pattern) as T;
                case MemberAccessExpressionSyntax maes: return PlacedOnSameLine(maes) as T;
                case NameSyntax name: return PlacedOnSameLine(name) as T;
                case ObjectCreationExpressionSyntax creation: return PlacedOnSameLine(creation) as T;
                case PatternSyntax pattern: return PlacedOnSameLine(pattern) as T;
                case PropertyPatternClauseSyntax clause: return PlacedOnSameLine(clause) as T;
                case SingleVariableDesignationSyntax singleVariable: return PlacedOnSameLine(singleVariable) as T;
                case SubpatternSyntax subpattern: return PlacedOnSameLine(subpattern) as T;
                case SwitchExpressionArmSyntax arm: return PlacedOnSameLine(arm) as T;
                case ThrowExpressionSyntax throwExpression: return PlacedOnSameLine(throwExpression) as T;
                case TypeArgumentListSyntax typeArgumentList: return PlacedOnSameLine(typeArgumentList) as T;
                case WhenClauseSyntax clause: return PlacedOnSameLine(clause) as T;
                default:
                    return value.WithoutTrivia();
            }
        }

        /// <summary>
        /// Creates a new argument with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The argument to modify.
        /// </param>
        /// <returns>
        /// A new argument with all its components placed on the same line.
        /// </returns>
        internal static ArgumentSyntax PlacedOnSameLine(this ArgumentSyntax value) => value.WithoutTrivia()
                                                                                           .WithRefKindKeyword(value.RefKindKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                           .WithRefOrOutKeyword(value.RefOrOutKeyword.WithoutLeadingTrivia().WithTrailingSpace())
                                                                                           .WithNameColon(PlacedOnSameLine(value.NameColon))
                                                                                           .WithExpression(PlacedOnSameLine(value.Expression));

        /// <summary>
        /// Creates a new argument list with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The argument list to modify.
        /// </param>
        /// <returns>
        /// A new argument list with all its components placed on the same line.
        /// </returns>
        internal static ArgumentListSyntax PlacedOnSameLine(this ArgumentListSyntax value) => value.WithoutTrivia()
                                                                                                   .WithOpenParenToken(value.OpenParenToken.WithoutTrivia())
                                                                                                   .WithArguments(PlacedOnSameLine(value.Arguments))
                                                                                                   .WithCloseParenToken(value.CloseParenToken.WithoutTrivia());

        /// <summary>
        /// Creates a new binary expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The binary expression to modify.
        /// </param>
        /// <returns>
        /// A new binary expression with all its components placed on the same line.
        /// </returns>
        internal static BinaryExpressionSyntax PlacedOnSameLine(this BinaryExpressionSyntax value) => value.WithoutTrivia()
                                                                                                           .WithLeft(PlacedOnSameLine(value.Left))
                                                                                                           .WithOperatorToken(value.OperatorToken.WithLeadingAndTrailingSpace())
                                                                                                           .WithRight(PlacedOnSameLine(value.Right));

        /// <summary>
        /// Creates a new case pattern switch label with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The case pattern switch label to modify.
        /// </param>
        /// <returns>
        /// A new case pattern switch label with all its components placed on the same line.
        /// </returns>
        internal static CasePatternSwitchLabelSyntax PlacedOnSameLine(this CasePatternSwitchLabelSyntax value) => value.WithoutTrivia()
                                                                                                                       .WithKeyword(value.Keyword.WithoutTrailingTrivia())
                                                                                                                       .WithPattern(PlacedOnSameLine(value.Pattern).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                                       .WithWhenClause(PlacedOnSameLine(value.WhenClause))
                                                                                                                       .WithColonToken(value.ColonToken.WithoutLeadingTrivia());

        /// <summary>
        /// Creates a new case switch label with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The case switch label to modify.
        /// </param>
        /// <returns>
        /// A new case switch label with all its components placed on the same line.
        /// </returns>
        internal static CaseSwitchLabelSyntax PlacedOnSameLine(this CaseSwitchLabelSyntax value) => value.WithoutTrivia()
                                                                                                         .WithKeyword(value.Keyword.WithoutTrailingTrivia())
                                                                                                         .WithValue(PlacedOnSameLine(value.Value).WithLeadingSpace().WithoutTrailingTrivia())
                                                                                                         .WithColonToken(value.ColonToken.WithoutLeadingTrivia());

        /// <summary>
        /// Creates a new conditional expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The conditional expression to modify.
        /// </param>
        /// <returns>
        /// A new conditional expression with all its components placed on the same line.
        /// </returns>
        internal static ConditionalExpressionSyntax PlacedOnSameLine(this ConditionalExpressionSyntax value) => value.WithoutTrivia()
                                                                                                                     .WithCondition(PlacedOnSameLine(value.Condition).WithoutTrivia())
                                                                                                                     .WithQuestionToken(value.QuestionToken.WithLeadingAndTrailingSpace())
                                                                                                                     .WithWhenTrue(PlacedOnSameLine(value.WhenTrue).WithoutTrivia())
                                                                                                                     .WithColonToken(value.ColonToken.WithLeadingAndTrailingSpace())
                                                                                                                     .WithWhenFalse(PlacedOnSameLine(value.WhenFalse).WithoutTrivia());

        /// <summary>
        /// Creates a new constant pattern with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The constant pattern to modify.
        /// </param>
        /// <returns>
        /// A new constant pattern with all its components placed on the same line.
        /// </returns>
        internal static ConstantPatternSyntax PlacedOnSameLine(this ConstantPatternSyntax value) => value.WithoutTrivia()
                                                                                                         .WithExpression(PlacedOnSameLine(value.Expression));

        /// <summary>
        /// Creates a new declaration pattern with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The declaration pattern to modify.
        /// </param>
        /// <returns>
        /// A new declaration pattern with all its components placed on the same line.
        /// </returns>
        internal static DeclarationPatternSyntax PlacedOnSameLine(this DeclarationPatternSyntax value) => value.WithoutTrivia()
                                                                                                               .WithType(value.Type.WithTrailingSpace())
                                                                                                               .WithDesignation(PlacedOnSameLine(value.Designation));

        /// <summary>
        /// Creates a new if statement with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The if statement to modify.
        /// </param>
        /// <returns>
        /// A new if statement with all its components placed on the same line.
        /// </returns>
        internal static IfStatementSyntax PlacedOnSameLine(this IfStatementSyntax value) => value.WithIfKeyword(value.IfKeyword.WithTrailingSpace())
                                                                                                 .WithOpenParenToken(value.OpenParenToken.WithoutTrailingTrivia())
                                                                                                 .WithCondition(value.Condition.PlacedOnSameLine())
                                                                                                 .WithCloseParenToken(value.CloseParenToken.WithoutLeadingTrivia());

        /// <summary>
        /// Creates a new invocation expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The invocation expression to modify.
        /// </param>
        /// <returns>
        /// A new invocation expression with all its components placed on the same line.
        /// </returns>
        internal static InvocationExpressionSyntax PlacedOnSameLine(this InvocationExpressionSyntax value) => value.WithoutTrivia()
                                                                                                                   .WithExpression(PlacedOnSameLine(value.Expression))
                                                                                                                   .WithArgumentList(PlacedOnSameLine(value.ArgumentList));

        /// <summary>
        /// Creates a new is pattern expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The is pattern expression to modify.
        /// </param>
        /// <returns>
        /// A new is pattern expression with all its components placed on the same line.
        /// </returns>
        internal static IsPatternExpressionSyntax PlacedOnSameLine(this IsPatternExpressionSyntax value) => value.WithoutTrivia()
                                                                                                                 .WithPattern(PlacedOnSameLine(value.Pattern))
                                                                                                                 .WithIsKeyword(value.IsKeyword.WithLeadingAndTrailingSpace())
                                                                                                                 .WithExpression(PlacedOnSameLine(value.Expression));

        /// <summary>
        /// Creates a new member access expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The member access expression to modify.
        /// </param>
        /// <returns>
        /// A new member access expression with all its components placed on the same line.
        /// </returns>
        internal static MemberAccessExpressionSyntax PlacedOnSameLine(this MemberAccessExpressionSyntax value) => value.WithoutTrivia()
                                                                                                                       .WithName(PlacedOnSameLine(value.Name))
                                                                                                                       .WithOperatorToken(value.OperatorToken.WithoutTrivia())
                                                                                                                       .WithExpression(PlacedOnSameLine(value.Expression));

        /// <summary>
        /// Creates a new name with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The name to modify.
        /// </param>
        /// <returns>
        /// A new name with all its components placed on the same line.
        /// </returns>
        internal static NameSyntax PlacedOnSameLine(this NameSyntax value)
        {
            switch (value)
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
                    return value.WithoutTrivia();
                }
            }
        }

        /// <summary>
        /// Creates a new object creation expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The object creation expression to modify.
        /// </param>
        /// <returns>
        /// A new object creation expression with all its components placed on the same line.
        /// </returns>
        internal static ObjectCreationExpressionSyntax PlacedOnSameLine(this ObjectCreationExpressionSyntax value) => value.WithoutTrivia()
                                                                                                                           .WithNewKeyword(value.NewKeyword.WithoutTrivia())
                                                                                                                           .WithType(PlacedOnSameLine(value.Type).WithLeadingSpace())
                                                                                                                           .WithArgumentList(PlacedOnSameLine(value.ArgumentList))
                                                                                                                           .WithInitializer(PlacedOnSameLine(value.Initializer));

        /// <summary>
        /// Creates a new pattern with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The pattern to modify.
        /// </param>
        /// <returns>
        /// A new pattern with all its components placed on the same line.
        /// </returns>
        internal static PatternSyntax PlacedOnSameLine(this PatternSyntax value)
        {
            switch (value)
            {
                case ConstantPatternSyntax constantPattern: return PlacedOnSameLine(constantPattern);
                case DeclarationPatternSyntax declaration: return PlacedOnSameLine(declaration);
                case UnaryPatternSyntax unaryPattern: return PlacedOnSameLine(unaryPattern);
                case RecursivePatternSyntax recursivePattern: return PlacedOnSameLine(recursivePattern);

                /*
                   -> BinaryPatternSyntax
                   -> DiscardPatternSyntax
                   -> ListPatternSyntax
                   -> ParenthesizedPatternSyntax
                   -> RecursivePatternSyntax
                   -> RelationalPatternSyntax
                   -> SlicePatternSyntax
                   -> TypePatternSyntax
                   -> VarPatternSyntax
                 */
                default:
                    return value.WithoutTrivia();
            }
        }

        /// <summary>
        /// Creates a new recursive pattern with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The recursive pattern to modify.
        /// </param>
        /// <returns>
        /// A new recursive pattern with all its components placed on the same line.
        /// </returns>
        internal static RecursivePatternSyntax PlacedOnSameLine(this RecursivePatternSyntax value)
        {
            var updatedType = value.Type?.PlacedOnSameLine();
            var updatedPropertyPatternClause = value.PropertyPatternClause?.PlacedOnSameLine();
            var updatedDesignation = value.Designation?.PlacedOnSameLine();

            if (updatedType != null)
            {
                updatedPropertyPatternClause = updatedPropertyPatternClause?.WithLeadingSpace();
            }

            if (updatedPropertyPatternClause != null)
            {
                updatedDesignation = updatedDesignation?.WithLeadingSpace();
            }

            var updatedSyntax = value.WithoutTrivia()
                                     .WithType(updatedType)
                                     .WithPropertyPatternClause(updatedPropertyPatternClause)
                                     .WithDesignation(updatedDesignation)
                                     .WithPositionalPatternClause(value.PositionalPatternClause?.PlacedOnSameLine());

            return updatedSyntax;
        }

        /// <summary>
        /// Creates a new property pattern clause with all its subpatterns placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The property pattern clause to modify.
        /// </param>
        /// <returns>
        /// A new property pattern clause with all its subpatterns placed on the same line.
        /// </returns>
        internal static PropertyPatternClauseSyntax PlacedOnSameLine(this PropertyPatternClauseSyntax value)
        {
            var updatedSyntax = value.WithoutTrivia()
                                     .WithOpenBraceToken(value.OpenBraceToken.WithoutTrivia().WithTrailingSpace())
                                     .WithSubpatterns(value.Subpatterns.PlacedOnSameLine());

            if (value.Subpatterns.Count is 0)
            {
                return updatedSyntax;
            }

            return updatedSyntax.WithCloseBraceToken(value.CloseBraceToken.WithoutTrivia().WithLeadingSpace());
        }

        /// <summary>
        /// Creates a new single variable designation with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The single variable designation to modify.
        /// </param>
        /// <returns>
        /// A new single variable designation with all its components placed on the same line.
        /// </returns>
        internal static SingleVariableDesignationSyntax PlacedOnSameLine(this SingleVariableDesignationSyntax value) => value.WithoutTrivia()
                                                                                                                             .WithIdentifier(value.Identifier.WithoutTrivia());

        /// <summary>
        /// Creates a new subpattern with all its patterns placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The subpattern to modify.
        /// </param>
        /// <returns>
        /// A new subpattern with all its patterns placed on the same line.
        /// </returns>
        internal static SubpatternSyntax PlacedOnSameLine(this SubpatternSyntax value) => value.WithoutTrivia()
                                                                                               .WithNameColon(value.NameColon?.WithoutTrivia().WithTrailingSpace())
#if VS2022 || VS2026
                                                                                               .WithExpressionColon(value.ExpressionColon?.WithoutTrivia().WithTrailingSpace())
#endif
                                                                                               .WithPattern(value.Pattern.PlacedOnSameLine());

        /// <summary>
        /// Creates a new switch expression arm with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The switch expression arm to modify.
        /// </param>
        /// <returns>
        /// A new switch expression arm with all its components placed on the same line.
        /// </returns>
        internal static SwitchExpressionArmSyntax PlacedOnSameLine(this SwitchExpressionArmSyntax value) => value.WithoutTrailingTrivia()
                                                                                                                 .WithEqualsGreaterThanToken(value.EqualsGreaterThanToken.WithLeadingAndTrailingSpace())
                                                                                                                 .WithExpression(PlacedOnSameLine(value.Expression))
                                                                                                                 .WithWhenClause(PlacedOnSameLine(value.WhenClause))
                                                                                                                 .WithPattern(PlacedOnSameLine(value.Pattern));

        /// <summary>
        /// Creates a new throw expression with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The throw expression to modify.
        /// </param>
        /// <returns>
        /// A new throw expression with all its components placed on the same line.
        /// </returns>
        internal static ThrowExpressionSyntax PlacedOnSameLine(this ThrowExpressionSyntax value) => value.WithoutTrivia()
                                                                                                         .WithThrowKeyword(value.ThrowKeyword.WithoutTrivia().WithTrailingSpace())
                                                                                                         .WithExpression(PlacedOnSameLine(value.Expression));

        /// <summary>
        /// Creates a new type argument list with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The type argument list to modify.
        /// </param>
        /// <returns>
        /// A new type argument list with all its components placed on the same line.
        /// </returns>
        internal static TypeArgumentListSyntax PlacedOnSameLine(this TypeArgumentListSyntax value) => value.WithoutTrivia()
                                                                                                           .WithArguments(PlacedOnSameLine(value.Arguments))
                                                                                                           .WithGreaterThanToken(value.GreaterThanToken.WithoutTrivia())
                                                                                                           .WithLessThanToken(value.LessThanToken.WithoutTrivia());

        /// <summary>
        /// Creates a new unary pattern with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The unary pattern to modify.
        /// </param>
        /// <returns>
        /// A new unary pattern with all its components placed on the same line.
        /// </returns>
        internal static UnaryPatternSyntax PlacedOnSameLine(this UnaryPatternSyntax value) => value.WithoutTrivia()
                                                                                                   .WithOperatorToken(value.OperatorToken.WithLeadingAndTrailingSpace())
                                                                                                   .WithPattern(PlacedOnSameLine(value.Pattern));

        /// <summary>
        /// Creates a new when clause with all its components placed on the same line.
        /// </summary>
        /// <param name="value">
        /// The when clause to modify.
        /// </param>
        /// <returns>
        /// A new when clause with all its components placed on the same line.
        /// </returns>
        internal static WhenClauseSyntax PlacedOnSameLine(this WhenClauseSyntax value) => value?.WithoutTrivia()
                                                                                                .WithWhenKeyword(value.WhenKeyword.WithLeadingAndTrailingSpace())
                                                                                                .WithCondition(PlacedOnSameLine(value.Condition));

        /// <summary>
        /// Creates a new node from this node with an additional leading empty line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add an empty line to.
        /// </param>
        /// <returns>
        /// A new syntax node with an additional leading empty line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithAdditionalLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithAdditionalLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        /// <summary>
        /// Creates a new node from this node with additional leading spaces.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add leading spaces to.
        /// </param>
        /// <param name="additionalSpaces">
        /// The number of additional spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax node with additional leading spaces.
        /// </returns>
        internal static T WithAdditionalLeadingSpaces<T>(this T value, in int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            var currentSpaces = value.GetPositionWithinStartLine();

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        /// <summary>
        /// Creates a new node from this node with additional leading spaces at the end of its leading trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add leading spaces to.
        /// </param>
        /// <param name="additionalSpaces">
        /// The number of additional spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax node with additional leading spaces at the end of its leading trivia.
        /// </returns>
        internal static T WithAdditionalLeadingSpacesAtEnd<T>(this T value, in int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            return value.WithAdditionalLeadingTrivia(WhiteSpaces(additionalSpaces));
        }

        /// <summary>
        /// Creates a new node from this node with additional leading spaces on its descendants.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node whose descendants to add leading spaces to.
        /// </param>
        /// <param name="descendants">
        /// The descendants to add leading spaces to.
        /// </param>
        /// <param name="additionalSpaces">
        /// The number of additional spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax node with additional leading spaces on its descendants.
        /// </returns>
        internal static T WithAdditionalLeadingSpacesOnDescendants<T>(this T value, IReadOnlyCollection<SyntaxNodeOrToken> descendants, int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            if (descendants.Count is 0)
            {
                return value;
            }

            return value.ReplaceSyntax(
                                   descendants.Where(_ => _.IsNode).Select(_ => _.AsNode()),
                                   (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                   descendants.Where(_ => _.IsToken).Select(_ => _.AsToken()),
                                   (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                   Array.Empty<SyntaxTrivia>(),
                                   (original, rewritten) => rewritten);
        }

        /// <summary>
        /// Creates a new node from this node with additional leading trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia list to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the additional leading trivia.
        /// </returns>
        internal static T WithAdditionalLeadingTrivia<T>(this T value, in SyntaxTriviaList trivia) where T : SyntaxNode => value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivia));

        /// <summary>
        /// Creates a new node from this node with an additional leading trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the additional leading trivia.
        /// </returns>
        internal static T WithAdditionalLeadingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode => value.WithLeadingTrivia(value.GetLeadingTrivia().Add(trivia));

        /// <summary>
        /// Creates a new node from this node with additional leading trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia array to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the additional leading trivia.
        /// </returns>
        internal static T WithAdditionalLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode => value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivia));

        /// <summary>
        /// Creates a new node from this node with additional leading trivia from another node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="node">
        /// The syntax node to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with additional leading trivia from the specified node.
        /// </returns>
        internal static T WithAdditionalLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new node from this node with additional leading trivia from a token.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="token">
        /// The syntax token to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with additional leading trivia from the specified token.
        /// </returns>
        internal static T WithAdditionalLeadingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new node from this node with an additional trailing empty line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a trailing empty line to.
        /// </param>
        /// <returns>
        /// A new syntax node with an additional trailing empty line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithAdditionalTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithAdditionalTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        /// <summary>
        /// Creates a new node from this node with additional trailing trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia list to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the additional trailing trivia.
        /// </returns>
        internal static T WithAdditionalTrailingTrivia<T>(this T value, in SyntaxTriviaList trivia) where T : SyntaxNode => value.WithTrailingTrivia(value.GetTrailingTrivia().AddRange(trivia));

        /// <summary>
        /// Creates a new node from this node with an additional trailing trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the additional trailing trivia.
        /// </returns>
        internal static T WithAdditionalTrailingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode => value.WithTrailingTrivia(value.GetTrailingTrivia().Add(trivia));

        /// <summary>
        /// Creates a new node from this node with additional trailing trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia array to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the additional trailing trivia.
        /// </returns>
        internal static T WithAdditionalTrailingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode => value.WithTrailingTrivia(value.GetTrailingTrivia().AddRange(trivia));

        /// <summary>
        /// Creates a new node from this node with a trailing end-of-line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add an end-of-line to.
        /// </param>
        /// <returns>
        /// A new syntax node with a trailing end-of-line.
        /// </returns>
        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        /// <summary>
        /// Creates a new node from this node with a trivia added at the beginning of its leading trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the trivia added at the beginning of its leading trivia.
        /// </returns>
        internal static T WithFirstLeadingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
        {
            // Attention: leading trivia contains XML comments, so we have to keep them!
            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count > 0)
            {
                // remove leading end-of-line as otherwise we would have multiple empty lines left over
                if (leadingTrivia[0].IsEndOfLine())
                {
                    leadingTrivia = leadingTrivia.RemoveAt(0);
                }

                return value.WithLeadingTrivia(leadingTrivia.Insert(0, trivia));
            }

            return value.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Creates a new node from this node with trivia added at the beginning of its leading trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="trivia">
        /// The trivia array to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the trivia added at the beginning of its leading trivia.
        /// </returns>
        internal static T WithFirstLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
        {
            // Attention: leading trivia contains XML comments, so we have to keep them!
            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count > 0)
            {
                // remove leading end-of-line as otherwise we would have multiple empty lines left over
                if (leadingTrivia[0].IsEndOfLine())
                {
                    leadingTrivia = leadingTrivia.RemoveAt(0);
                }

                return value.WithLeadingTrivia(leadingTrivia.InsertRange(0, trivia));
            }

            return value.WithLeadingTrivia(trivia);
        }

        /// <summary>
        /// Creates a new node from this node with indentation.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add indentation to.
        /// </param>
        /// <returns>
        /// A new syntax node with indentation.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithIndentation<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticMarker); // use elastic one to allow formatting to be done automatically

        /// <summary>
        /// Creates a new node from this node with a leading empty line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a leading empty line to.
        /// </param>
        /// <returns>
        /// A new syntax node with a leading empty line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again

        /// <summary>
        /// Creates a new node from this node with a leading end-of-line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a leading end-of-line to.
        /// </param>
        /// <returns>
        /// A new syntax node with a leading end-of-line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithLeadingEndOfLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        /// <summary>
        /// Creates a new node from this node with a leading space.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a leading space to.
        /// </param>
        /// <returns>
        /// A new syntax node with a leading space.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithLeadingSpace<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(SyntaxFactory.Space); // use non-elastic one to prevent formatting to be done automatically

        /// <summary>
        /// Creates a new node from this node with the specified number of leading spaces.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add leading spaces to.
        /// </param>
        /// <param name="count">
        /// The number of leading spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the specified number of leading spaces.
        /// </returns>
        internal static T WithLeadingSpaces<T>(this T value, in int count) where T : SyntaxNode
        {
            if (count <= 0)
            {
                return value;
            }

            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count is 0)
            {
                return value.WithLeadingTrivia(WhiteSpaces(count));
            }

            // re-construct leading comment with correct amount of spaces but keep comments
            // (so we have to find out each white-space trivia and have to replace it with the correct amount of spaces)
            var finalTrivia = leadingTrivia.ToArray();

            var resetFinalTrivia = false;

            for (int index = 0, length = finalTrivia.Length; index < length; index++)
            {
                var trivia = finalTrivia[index];

                if (trivia.IsWhiteSpace())
                {
                    finalTrivia[index] = WhiteSpaces(count);
                }

                if (trivia.IsComment())
                {
                    resetFinalTrivia = true;

                    // we do not need to adjust further as we found a comment and have to fix them based on their specific lines
                    break;
                }
            }

            if (resetFinalTrivia)
            {
                finalTrivia = CalculateWhitespaceTriviaWithComment(count, leadingTrivia.ToArray());
            }

            return value.WithLeadingTrivia(finalTrivia);
        }

        /// <summary>
        /// Creates a new node from this node with leading trivia from another node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="node">
        /// The syntax node to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with leading trivia from the specified node.
        /// </returns>
        internal static T WithLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new node from this node with leading trivia from a token.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="token">
        /// The syntax token to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with leading trivia from the specified token.
        /// </returns>
        internal static T WithLeadingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new node from this node without a leading end-of-line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to remove the leading end-of-line from.
        /// </param>
        /// <returns>
        /// A new syntax node without a leading end-of-line.
        /// </returns>
        internal static T WithoutLeadingEndOfLine<T>(this T value) where T : SyntaxNode
        {
            var trivia = value.GetLeadingTrivia();

            if (trivia.Count > 0 && trivia[0].IsEndOfLine())
            {
                return value.WithLeadingTrivia(trivia.RemoveAt(0));
            }

            return value;
        }

        /// <summary>
        /// Creates a new node from this node without trailing spaces.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to remove trailing spaces from.
        /// </param>
        /// <returns>
        /// A new syntax node without trailing spaces.
        /// </returns>
        internal static T WithoutTrailingSpaces<T>(this T value) where T : SyntaxNode
        {
            var trivia = value.GetTrailingTrivia();
            var triviaCount = trivia.Count;

            if (triviaCount <= 0)
            {
                return value;
            }

            var i = triviaCount - 1;

            for (; i > -1; i--)
            {
                if (trivia[i].IsKind(SyntaxKind.WhitespaceTrivia) is false)
                {
                    break;
                }
            }

            return value.WithTrailingTrivia(i > 0 ? trivia.Take(i) : SyntaxTriviaList.Empty);
        }

        /// <summary>
        /// Creates a copy of the specified XML element with tags on separate lines.
        /// </summary>
        /// <param name="value">
        /// The XML element to modify.
        /// </param>
        /// <returns>
        /// A new XML element with start and end tags on separate lines.
        /// </returns>
        internal static XmlElementSyntax WithTagsOnSeparateLines(this XmlElementSyntax value)
        {
            var contents = value.Content;

            var updateStartTag = true;
            var updateEndTag = true;

            if (contents.FirstOrDefault() is XmlTextSyntax firstText)
            {
                if (firstText.HasLeadingTrivia)
                {
                    updateStartTag = false;
                }
                else
                {
                    var textTokens = firstText.TextTokens;
                    var length = textTokens.Count;

                    if (length >= 2)
                    {
                        var firstToken = textTokens[0];
                        var nextToken = textTokens[1];

                        if (firstToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            updateStartTag = false;
                        }
                        else if (nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && firstToken.ValueText.IsNullOrWhiteSpace())
                        {
                            updateStartTag = false;
                        }
                    }
                }
            }

            if (contents.LastOrDefault() is XmlTextSyntax lastText)
            {
                var textTokens = lastText.TextTokens;
                var length = textTokens.Count;

                if (length >= 2)
                {
                    var lastToken = textTokens[length - 1];
                    var secondLastToken = textTokens[length - 2];

                    if (lastToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        updateEndTag = false;
                    }
                    else if (secondLastToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && lastToken.ValueText.IsNullOrWhiteSpace())
                    {
                        updateEndTag = false;
                    }
                }
            }

            if (updateStartTag)
            {
                value = value.WithStartTag(value.StartTag.WithTrailingXmlComment());
            }

            if (updateEndTag)
            {
                value = value.WithEndTag(value.EndTag.WithLeadingXmlComment());
            }

            return value;
        }

        /// <summary>
        /// Creates a new node from this node with a trailing empty line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a trailing empty line to.
        /// </param>
        /// <returns>
        /// A new syntax node with a trailing empty line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        /// <summary>
        /// Creates a new node from this node with a trailing new line.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a trailing new line to.
        /// </param>
        /// <returns>
        /// A new syntax node with a trailing new line.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithTrailingNewLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use an elastic one to enforce the line break

        /// <summary>
        /// Creates a new node from this node with a trailing space.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add a trailing space to.
        /// </param>
        /// <returns>
        /// A new syntax node with a trailing space.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithTrailingSpace<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.Space); // use non-elastic one to prevent formatting to be done automatically

        /// <summary>
        /// Creates a new node from this node with trailing spaces.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trailing spaces to.
        /// </param>
        /// <param name="spaces">
        /// The number of spaces to add.
        /// </param>
        /// <returns>
        /// A new syntax node with the specified number of trailing spaces.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static T WithTrailingSpaces<T>(this T value, in int spaces) where T : SyntaxNode => value.WithTrailingTrivia(Enumerable.Repeat(SyntaxFactory.Space, spaces)); // use non-elastic one to prevent formatting to be done automatically

        /// <summary>
        /// Creates a new node from this node with trailing trivia from another node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="node">
        /// The syntax node to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with trailing trivia from the specified node.
        /// </returns>
        internal static T WithTrailingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetTrailingTrivia();

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new node from this node with trailing trivia from a token.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="token">
        /// The syntax token to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with trailing trivia from the specified token.
        /// </returns>
        internal static T WithTrailingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        /// <summary>
        /// Creates a new node from this node with leading and trailing trivia from another node.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="node">
        /// The syntax node to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with leading and trailing trivia from the specified node.
        /// </returns>
        internal static T WithTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.WithLeadingTriviaFrom(node)
                                                                                                        .WithTrailingTriviaFrom(node);

        /// <summary>
        /// Creates a new node from this node with leading and trailing trivia from a token.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trivia to.
        /// </param>
        /// <param name="token">
        /// The syntax token to get trivia from.
        /// </param>
        /// <returns>
        /// A new syntax node with leading and trailing trivia from the specified token.
        /// </returns>
        internal static T WithTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode => value.WithLeadingTriviaFrom(token)
                                                                                                             .WithTrailingTriviaFrom(token);

        /// <summary>
        /// Calculates the appropriate whitespace trivia for lines containing comments.
        /// </summary>
        /// <param name="count">
        /// The number of spaces to use for indentation.
        /// </param>
        /// <param name="finalTrivia">
        /// The trivia to adjust.
        /// </param>
        /// <returns>
        /// An array of the adjusted trivia with proper whitespace indentation for comments.
        /// </returns>
        private static SyntaxTrivia[] CalculateWhitespaceTriviaWithComment(in int count, in SyntaxTrivia[] finalTrivia)
        {
            var triviaGroupedByLines = finalTrivia.GroupBy(_ => _.GetStartingLine());

            foreach (var triviaGroup in triviaGroupedByLines)
            {
                var trivia1 = triviaGroup.ElementAt(0);

                if (trivia1.IsWhiteSpace())
                {
                    var index1 = finalTrivia.IndexOf(trivia1);
                    var spaces = count;

                    if (triviaGroup.MoreThan(1))
                    {
                        var trivia2 = triviaGroup.ElementAt(1);

                        if (trivia2.IsMultiLineComment())
                        {
                            var commentLength = trivia2.FullSpan.Length;
                            var remainingSpaces = triviaGroup.Skip(2).Sum(_ => _.FullSpan.Length);

                            spaces = count - commentLength - remainingSpaces;

                            if (spaces < 0)
                            {
                                // it seems we want to remove some spaces, so 'count' is already correct
                                spaces = count;
                            }
                        }
                    }

                    finalTrivia[index1] = WhiteSpaces(spaces);
                }
            }

            return finalTrivia;
        }

        /// <summary>
        /// Creates a syntax trivia for the specified number of spaces.
        /// </summary>
        /// <param name="count">
        /// The number of spaces.
        /// </param>
        /// <returns>
        /// A syntax trivia consisting of the specified number of spaces.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SyntaxTrivia WhiteSpaces(in int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}