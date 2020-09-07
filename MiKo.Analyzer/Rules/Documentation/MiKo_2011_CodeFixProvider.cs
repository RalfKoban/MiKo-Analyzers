using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2011_CodeFixProvider)), Shared]
    public sealed class MiKo_2011_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2011_UnsealedClassSummaryAnalyzer.Id;

        protected override string Title => "Remove sealed text to comment";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            return SyntaxFactory.XmlElement(comment.StartTag, comment.WithoutText(Constants.Comments.SealedClassPhrase), comment.EndTag);
        }
    }
}