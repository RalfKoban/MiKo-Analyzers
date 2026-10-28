using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6041_CodeFixProvider)), Shared]
    public sealed class MiKo_6041_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6041";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntaxNode = syntaxNodes.First();

            switch (syntaxNode)
            {
                case EqualsValueClauseSyntax clause: return clause.Parent?.Parent?.Parent;
                case AssignmentExpressionSyntax assignment: return assignment;
                default:
                    return null;
            }
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            // TODO RKN: Move this into SyntaxNodeExtensions as method 'PlacedOnSameLine' ?
            if (syntax is AssignmentExpressionSyntax assignment)
            {
                return assignment.WithLeft(assignment.Left.WithoutTrailingTrivia())
                                 .WithOperatorToken(assignment.OperatorToken.WithSurroundingSpace())
                                 .WithRight(assignment.Right.WithoutLeadingTrivia());
            }

            return GetUpdatedSyntaxWithEqualsValueClause(syntax);
        }

        private static SyntaxNode GetUpdatedSyntaxWithEqualsValueClause(SyntaxNode syntax)
        {
            var clause = GetEqualsValueClause(syntax);

            var updatedClause = clause.WithEqualsToken(clause.EqualsToken.WithoutTrivia())
                                      .WithValue(GetUpdatedEqualsValueClauseValue(clause));

            var updatedSyntax = syntax.ReplaceNode(clause, updatedClause);

            // move comment to the end if it is a leading one
            if (syntax is StatementSyntax || syntax is MemberDeclarationSyntax)
            {
                var leadingComments = clause.FindSyntaxNodeWithLeadingComment()
                                           .GetLeadingComments();

                if (leadingComments.Length > 0)
                {
                    var comment = SyntaxFactory.Comment(Constants.Comments.CommentExterior + Constants.SingleSpace + leadingComments.ConcatenatedWith(Constants.SingleSpace));

                    updatedSyntax = updatedSyntax.WithTrailingTrivia(SyntaxFactory.Space, comment, SyntaxFactory.CarriageReturnLineFeed);
                }
            }

            var sibling = GetEqualsValueClause(updatedSyntax).PreviousSiblingNodeOrToken();

            if (sibling.IsToken)
            {
                var token = sibling.AsToken();
                var updatedToken = token.WithoutTrailingTrivia().WithTrailingSpace();

                return updatedSyntax.ReplaceToken(token, updatedToken);
            }

            return updatedSyntax;
        }

        private static EqualsValueClauseSyntax GetEqualsValueClause(SyntaxNode syntax) => syntax.FirstDescendant<EqualsValueClauseSyntax>();

        private static ExpressionSyntax GetUpdatedEqualsValueClauseValue(EqualsValueClauseSyntax clause)
        {
            var expression = clause.Value;

            switch (expression)
            {
                case BinaryExpressionSyntax binary:
                    return binary.PlacedOnSameLine().WithLeadingSpace();

#if VS2022 || VS2026
                case CollectionExpressionSyntax collectionExpression:
                    return collectionExpression.WithOpenBracketToken(collectionExpression.OpenBracketToken.WithoutLeadingTrivia().WithLeadingSpace());
#endif
                case InvocationExpressionSyntax invocation:
                    return invocation.WithLeadingSpace();

                default:
                    return expression.WithLeadingSpace();
            }
        }
    }
}