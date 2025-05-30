﻿using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6047_CodeFixProvider)), Shared]
    public sealed class MiKo_6047_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6047";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is SwitchExpressionSyntax expression)
            {
                var spaces = GetProposedSpaces(issue);
                var armSpaces = spaces + Constants.Indentation;

                return expression.WithOpenBraceToken(expression.OpenBraceToken.WithLeadingSpaces(spaces))
                                 .WithArms(SyntaxFactory.SeparatedList(
                                                                   expression.Arms.Select(_ => _.WithLeadingSpaces(armSpaces)),
                                                                   expression.Arms.GetSeparators()))
                                 .WithCloseBraceToken(expression.CloseBraceToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}