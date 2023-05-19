using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2100_CodeFixProvider)), Shared]
    public sealed class MiKo_2100_CodeFixProvider : ExampleDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.ExampleDefaultPhrase;

        public override string FixableDiagnosticId => MiKo_2100_ExampleDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2100_CodeFixTitle.FormatWith(Phrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => CommentStartingWith((XmlElementSyntax)syntax, Phrase);
    }
}