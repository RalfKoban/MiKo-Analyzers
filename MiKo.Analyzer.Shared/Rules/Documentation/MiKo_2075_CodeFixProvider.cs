﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2075_CodeFixProvider)), Shared]
    public sealed class MiKo_2075_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
        private static readonly Pair[] ReplacementMap = Constants.Comments.ActionTerms.ToArray(_ => new Pair(_, Constants.Comments.CallbackTerm));

        public override string FixableDiagnosticId => "MiKo_2075";

        protected override string Title => Resources.MiKo_2075_CodeFixTitle.FormatWith(Constants.Comments.CallbackTerm);

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => GetUpdatedSyntax(syntax, issue, ReplacementMap);
    }
}