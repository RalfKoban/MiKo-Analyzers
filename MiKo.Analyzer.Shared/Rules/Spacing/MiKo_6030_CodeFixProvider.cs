﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6030_CodeFixProvider)), Shared]
    public sealed class MiKo_6030_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6030";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var spaces = GetProposedSpaces(issue);

            switch (syntax)
            {
                case InitializerExpressionSyntax initializer:
                {
                    return GetUpdatedSyntax(initializer, spaces);
                }

                case AnonymousObjectCreationExpressionSyntax anonymous:
                {
                    return GetUpdatedSyntax(anonymous, spaces);
                }

                default:
                    return syntax;
            }
        }

        private static AnonymousObjectCreationExpressionSyntax GetUpdatedSyntax(AnonymousObjectCreationExpressionSyntax anonymous, int spaces)
        {
            var openBraceToken = anonymous.OpenBraceToken;
            var closeBraceToken = anonymous.CloseBraceToken;

            var closeBraceTokenSpaces = openBraceToken.IsOnSameLineAs(closeBraceToken) ? 0 : spaces;

            return anonymous.WithOpenBraceToken(openBraceToken.WithLeadingSpaces(spaces))
                            .WithInitializers(GetUpdatedSyntax(anonymous.Initializers, openBraceToken, spaces + Constants.Indentation))
                            .WithCloseBraceToken(closeBraceToken.WithLeadingSpaces(closeBraceTokenSpaces));
        }

        private static InitializerExpressionSyntax GetUpdatedSyntax(InitializerExpressionSyntax initializer, int spaces)
        {
            var openBraceToken = initializer.OpenBraceToken;
            var closeBraceToken = initializer.CloseBraceToken;

            var closeBraceTokenSpaces = openBraceToken.IsOnSameLineAs(closeBraceToken) ? 0 : spaces;

            return initializer.WithOpenBraceToken(openBraceToken.WithLeadingSpaces(spaces))
                              .WithExpressions(GetUpdatedSyntax(initializer.Expressions, openBraceToken, spaces + Constants.Indentation))
                              .WithCloseBraceToken(closeBraceToken.WithLeadingSpaces(closeBraceTokenSpaces));
        }
    }
}