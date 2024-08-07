﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6034_CodeFixProvider)), Shared]
    public sealed class MiKo_6034_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6034";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MemberAccessExpressionSyntax maes)
            {
                var name = maes.Name;

                return maes.WithOperatorToken(maes.OperatorToken.WithoutTrivia().WithLeadingTriviaFrom(name).WithLeadingEmptyLine())
                           .WithName(name.WithoutLeadingTrivia());
            }

            return syntax;
        }
    }
}