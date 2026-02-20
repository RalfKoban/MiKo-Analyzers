using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2211_CodeFixProvider)), Shared]
    public sealed class MiKo_2211_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2211";

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax)
        {
            var remarks = syntax.GetXmlSyntax(Constants.XmlTag.Remarks)[0];
            var summaries = syntax.GetXmlSyntax(Constants.XmlTag.Summary);

            // add remarks into summary
            if (summaries.Count is 0)
            {
                var newSummary = SyntaxFactory.XmlSummaryElement(remarks.Content.ToArray());

                return syntax.ReplaceNode(remarks, newSummary);
            }
            else
            {
                var newSummary = summaries[0].AddContent(Para()).AddContent(remarks.Content.ToArray());

                return SyntaxFactory.DocumentationComment(newSummary).WithEndOfLine();
            }
        }
    }
}