﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2074_CodeFixProvider)), Shared]
    public sealed class MiKo_2074_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2074_CodeFixTitle;

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index)
        {
            return CommentEndingWith(comment, MiKo_2074_ContainsParameterDefaultPhraseAnalyzer.Phrase);
        }
    }
}