using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2232_CodeFixProvider)), Shared]
    public sealed class MiKo_2232_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2232";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var node = syntax.FindNode(diagnostic.Location.SourceSpan, true, true);

            if (node is XmlElementSyntax issue)
            {
                if (issue.Parent == syntax)
                {
                    // we are directly within the DocumentationCommentTriviaSyntax
                    var updatedContent = GetUpdatedXmlContent(syntax.Content, issue);

                    if (updatedContent.Count != 0)
                    {
                        var updatedSyntax = syntax.WithContent(updatedContent.WithoutFirstXmlNewLine().WithLeadingXmlComment().WithIndentation());

                        return updatedSyntax;
                    }

                    return null;
                }
            }

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var token = syntax is DocumentationCommentTriviaSyntax comment
                        ? comment.ParentTrivia.Token
                        : root.FindToken(issue.Location.SourceSpan.Start); // XML comment should be removed here, so adjust empty line on token

            return root.ReplaceToken(token, token.WithLeadingEmptyLine());
        }

        private static SyntaxList<XmlNodeSyntax> GetUpdatedXmlContent(SyntaxList<XmlNodeSyntax> originalContent, XmlElementSyntax issue)
        {
            var content = originalContent.ToList();
            var index = content.IndexOf(issue);

            if (index > 0)
            {
                if (content.Count > index + 2)
                {
                    if (content[index + 2] is XmlTextSyntax)
                    {
                        RemoveEmptyText(index + 1);
                    }
                }
                else
                {
                    RemoveEmptyText(index + 1);
                }

                for (var i = index - 1; i >= 0; i--)
                {
                    RemoveEmptyText(i);
                }

                content.Remove(issue);
            }

            return SyntaxFactory.List(content);

            void RemoveEmptyText(int i)
            {
                if (content.Count > i && content[i] is XmlTextSyntax text && text.GetTextWithoutTrivia().IsEmpty)
                {
                    content.RemoveAt(i);
                }
            }
        }
    }
}