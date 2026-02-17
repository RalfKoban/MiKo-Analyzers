using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2048_CodeFixProvider)), Shared]
    public sealed class MiKo_2048_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.ValueConverterSummaryStartingPhrase;

        public override string FixableDiagnosticId => "MiKo_2048";

        protected override string Title => Resources.MiKo_2048_CodeFixTitle.FormatWith(Phrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return updatedSyntax;
        }

        private static XmlElementSyntax GetUpdatedSyntax(SyntaxNode syntax) => CommentStartingWith((XmlElementSyntax)syntax, Phrase);
    }
}