﻿using System;
using System.Composition;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2213_CodeFixProvider)), Shared]
    public sealed class MiKo_2213_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2213";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var token = syntax.FindToken(diagnostic);
            var text = token.ValueText.AsCachedBuilder().ReplaceAllWithProbe(Constants.Comments.NotContractionReplacementMap.AsSpan()).ToStringAndRelease();

            return syntax.ReplaceToken(token, token.WithText(text));
        }
    }
}