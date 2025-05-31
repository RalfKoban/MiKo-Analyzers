﻿using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2016_CodeFixProvider)), Shared]
    public sealed class MiKo_2016_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.AsynchronouslyStartingPhrase;

        public override string FixableDiagnosticId => "MiKo_2016";

        protected override string Title => Resources.MiKo_2016_CodeFixTitle.FormatWith(Phrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => CommentStartingWith((XmlElementSyntax)syntax, Phrase, FirstWordHandling.StartLowerCase | FirstWordHandling.MakeThirdPersonSingular);
    }
}