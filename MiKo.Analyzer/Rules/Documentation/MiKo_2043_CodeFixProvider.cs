using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2043_CodeFixProvider)), Shared]
    public sealed class MiKo_2043_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2043_DelegateSummaryAnalyzer.Id;

        protected override string Title => "Start summary with '" + Constants.Comments.DelegateSummaryStartingPhrase + "'";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;

            return CommentStartingWith(comment, Constants.Comments.DelegateSummaryStartingPhrase);
        }
    }
}