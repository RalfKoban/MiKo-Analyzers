﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6040_CodeFixProvider)), Shared]
    public sealed class MiKo_6040_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer.Id;

        protected override string Title => Resources.MiKo_6040_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberAccessExpressionSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MemberAccessExpressionSyntax m)
            {
                var spaces = MiKo_6040_MultiLineCallChainsAreOnSamePositionAnalyzer.GetSpaces(issue);

                return m.WithOperatorToken(m.OperatorToken.WithLeadingSpaces(spaces));
            }

            return syntax;
        }
    }
}