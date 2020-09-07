using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2211_CodeFixProvider)), Shared]
    public sealed class MiKo_2211_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2211_EnumerationMemberDocumentationHasNoRemarksAnalyzer.Id;

        protected override string Title => Resources.MiKo_2211_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var remarks = GetXmlSyntax(Constants.XmlTag.Remarks, comment).First();
            var summary = GetXmlSyntax(Constants.XmlTag.Summary, comment).FirstOrDefault();

            // add remarks into summary
            if (summary is null)
            {
                var newSummary = SyntaxFactory.XmlSummaryElement(remarks.Content.ToArray());

                return comment.ReplaceNode(remarks, newSummary);
            }
            else
            {
                var newSummary = summary.AddContent(Para()).AddContent(remarks.Content.ToArray());

                return SyntaxFactory.DocumentationComment(newSummary).WithEndOfLine();
            }
        }
    }
}