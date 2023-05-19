using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2211_CodeFixProvider)), Shared]
    public sealed class MiKo_2211_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer.Id;

        protected override string Title => Resources.MiKo_2211_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var remarks = syntax.GetXmlSyntax(Constants.XmlTag.Remarks).First();
            var summary = syntax.GetXmlSyntax(Constants.XmlTag.Summary).FirstOrDefault();

            // add remarks into summary
            if (summary is null)
            {
                var newSummary = SyntaxFactory.XmlSummaryElement(remarks.Content.ToArray());

                return syntax.ReplaceNode(remarks, newSummary);
            }
            else
            {
                var newSummary = summary.AddContent(Para()).AddContent(remarks.Content.ToArray());

                return SyntaxFactory.DocumentationComment(newSummary).WithEndOfLine();
            }
        }
    }
}