﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6048_CodeFixProvider)), Shared]
    public sealed class MiKo_6048_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6048";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            switch (syntax.Parent)
            {
                case IfStatementSyntax statement:
                {
                    var updated = statement.PlacedOnSameLine();

                    return root.ReplaceNode(statement, updated);
                }

                case ArrowExpressionClauseSyntax arrowClause:
                {
                    switch (arrowClause.Parent)
                    {
                        case BaseMethodDeclarationSyntax method:
                        {
                            var updatedMethod = method.WithSemicolonToken(method.SemicolonToken.WithoutLeadingTrivia());

                            return root.ReplaceNode(method, updatedMethod);
                        }

                        case PropertyDeclarationSyntax property:
                        {
                            var updatedProperty = property.WithSemicolonToken(property.SemicolonToken.WithoutLeadingTrivia());

                            return root.ReplaceNode(property, updatedProperty);
                        }
                    }

                    break;
                }

                case ReturnStatementSyntax returnStatement:
                {
                    var updatedReturnStatement = returnStatement.WithSemicolonToken(returnStatement.SemicolonToken.WithoutLeadingTrivia());

                    return root.ReplaceNode(returnStatement, updatedReturnStatement);
                }
            }

            return base.GetUpdatedSyntaxRoot(document, root, syntax, annotationOfSyntax, issue);
        }

        private static T GetUpdatedSyntax<T>(T syntax) where T : SyntaxNode
        {
            switch (syntax)
            {
                case BinaryExpressionSyntax binary:
                {
                    var updated = binary.WithLeft(GetUpdatedSyntax(binary.Left))
                                        .WithOperatorToken(binary.OperatorToken.WithoutTrivia().WithLeadingSpace())
                                        .WithRight(GetUpdatedSyntax(binary.Right));

                    return updated as T;
                }

                case ParenthesizedExpressionSyntax parenthesized:
                {
                    var updated = parenthesized.WithOpenParenToken(parenthesized.OpenParenToken.WithoutTrivia())
                                               .WithExpression(GetUpdatedSyntax(parenthesized.Expression))
                                               .WithCloseParenToken(parenthesized.CloseParenToken.WithoutTrivia());

                    return updated as T;
                }

                default:
                    return syntax.WithoutTrivia();
            }
        }
    }
}