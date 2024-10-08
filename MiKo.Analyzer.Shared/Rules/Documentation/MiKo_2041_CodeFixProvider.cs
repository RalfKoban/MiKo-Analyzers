﻿using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2041_CodeFixProvider)), Shared]
    public sealed class MiKo_2041_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2041";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var syntaxNodes = syntax.GetSummaryXmls(Constants.Comments.InvalidSummaryCrefXmlTags).ToList();
            var replacements = syntaxNodes.ToArray(_ => _.WithLeadingXmlCommentExterior().WithEndOfLine());

            var updatedSyntax = syntax.Without(syntaxNodes).AddContent(replacements);

            return updatedSyntax;
        }
    }
}