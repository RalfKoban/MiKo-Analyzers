﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6041_CodeFixProvider)), Shared]
    public sealed class MiKo_6041_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6041_AssignmentsAreOnSameLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_6041_CodeFixTitle;

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

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is AssignmentExpressionSyntax assignment)
            {
                return assignment.WithLeft(assignment.Left.WithoutTrailingTrivia())
                                 .WithOperatorToken(assignment.OperatorToken.WithLeadingSpace().WithTrailingSpace())
                                 .WithRight(assignment.Right.WithoutLeadingTrivia());
            }

            var clause = GetEqualsValueClause(syntax);

            var updatedClause = clause.WithEqualsToken(clause.EqualsToken.WithoutTrivia())
                                      .WithValue(clause.Value.WithLeadingSpace());

            var updatedSyntax = syntax.ReplaceNode(clause, updatedClause);

            // move comment to the end if it is a leading one
            if (syntax is StatementSyntax || syntax is MemberDeclarationSyntax)
            {
                var leadingComment = clause.GetLeadingComment();

                if (leadingComment.IsComment())
                {
                    updatedSyntax = updatedSyntax.WithTrailingTrivia(SyntaxFactory.Space, leadingComment, SyntaxFactory.CarriageReturnLineFeed);
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
    }
}