﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_6050_CodeFixProvider)), Shared]
    public sealed class MiKo_6050_CodeFixProvider : SpacingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_6050";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ArgumentSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ArgumentSyntax argument)
            {
                var spaces = GetProposedSpaces(issue);

                var additionalSpaces = spaces - argument.GetPositionWithinStartLine();

                var descendants = SelfAndDescendantsOnSeparateLines(argument);

                return argument.WithAdditionalLeadingSpacesOnDescendants(descendants, additionalSpaces);
            }

            return syntax;
        }
    }
}