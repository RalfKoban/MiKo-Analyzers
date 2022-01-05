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

        protected override string Title => string.Format(Resources.MiKo_2100_CodeFixTitle, Phrase);

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue) => CommentStartingWith((XmlElementSyntax)syntax, Phrase);
    }
}