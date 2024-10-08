﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6058_CodeFixProvider)), Shared]
    public sealed class MiKo_6058_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6058";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<TypeParameterConstraintClauseSyntax>().First();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is TypeParameterConstraintClauseSyntax node)
            {
                var spaces = GetProposedSpaces(issue);

                return node.WithLeadingSpaces(spaces);
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }
    }
}