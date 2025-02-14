﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6031_CodeFixProvider)), Shared]
    public sealed class MiKo_6031_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6031";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ConditionalExpressionSyntax expression)
            {
                var spaces = GetProposedSpaces(issue);

                return expression.WithQuestionToken(expression.QuestionToken.WithLeadingSpaces(spaces))
                                 .WithColonToken(expression.ColonToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}