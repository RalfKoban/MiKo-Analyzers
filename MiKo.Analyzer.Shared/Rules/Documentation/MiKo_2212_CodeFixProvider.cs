﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2212_CodeFixProvider)), Shared]
    public sealed class MiKo_2212_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2212";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue)
        {
            var token = syntax.FindToken(issue);

            return syntax.ReplaceToken(token, token.WithText(token.ValueText.Replace(Constants.Comments.WasNotSuccessfulPhrase, "failed")));
        }
    }
}