using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2016_CodeFixProvider)), Shared]
    public sealed class MiKo_2016_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.AsynchrounouslyStartingPhrase;

        public override string FixableDiagnosticId => MiKo_2016_AsyncMethodDefaultPhraseAnalyzer.Id;

        protected override string Title => "Start comment with '" + Phrase + "'";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => CommentStartingWith((XmlElementSyntax)syntax, Phrase);
    }
}