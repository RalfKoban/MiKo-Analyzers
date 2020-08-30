using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2048_CodeFixProvider)), Shared]
    public sealed class MiKo_2048_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string Phrase = Constants.Comments.ValueConverterSummaryStartingPhrase;

        public override string FixableDiagnosticId => MiKo_2048_ValueConverterSummaryDefaultPhraseAnalyzer.Id;

        protected override string Title => "Start comment with '" + Phrase + "'";

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => CommentStartingWith((XmlElementSyntax)syntax, Phrase);
    }
}