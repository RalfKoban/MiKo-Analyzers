using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2059_CodeFixProvider)), Shared]
    public sealed class MiKo_2059_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2059_DuplicateExceptionAnalyzer.Id;

        protected override string Title => Resources.MiKo_2059_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var exceptionComments = GetExceptionXmls(comment);

            var newNodes = new Dictionary<string, XmlElementSyntax>();
            var obsoleteNodes = new List<SyntaxNode>();

            // 1. find duplicate exception types
            // 2. find all the duplicates
            // 3. create a new node that contains content of duplicates incl. <para>
            // 4. remove duplicates as we need a new document
            // 5. find exception type again and replace the found instance with the new created instance
            var map = exceptionComments.ToLookup(GetReferencedExceptionName);
            foreach (var entry in map)
            {
                var duplicates = new Queue<XmlElementSyntax>(entry);
                if (duplicates.Count <= 1)
                {
                    continue;
                }

                var exception = duplicates.Dequeue();
                var newContent = exception.Content;

                // combine all remaining nodes into first one
                var siblings = exception.Parent?.ChildNodes().ToList();
                foreach (var duplicate in duplicates)
                {
                    obsoleteNodes.Add(duplicate);

                    // find the remaining ' /// ' text nodes and mark them as obsolete as they are not part of the elements
                    var exceptionIndex = siblings.IndexOf(duplicate);
                    var previousSibling = siblings[exceptionIndex - 1];
                    if (previousSibling is XmlTextSyntax)
                    {
                        obsoleteNodes.Add(previousSibling);
                    }

                    newContent = newContent.Add(CreateParaOr().WithLeadingXmlComment())
                                           .AddRange(duplicate.Content.WithLeadingXmlComment());
                }

                var consolidatedException = exception.WithContent(newContent.WithLeadingXmlComment().WithTrailingXmlComment());

                // remember the modified node
                newNodes.Add(entry.Key, consolidatedException);
            }

            var commentWithoutObsoleteNodes = comment.Without(obsoleteNodes);

            // find and replace the nodes with the new fixed ones
            var finalContent = commentWithoutObsoleteNodes.ReplaceNodes(
                                                                        GetExceptionXmls(commentWithoutObsoleteNodes),
                                                                        (original, rewritten) =>
                                                                            {
                                                                                var exceptionName = GetReferencedExceptionName(original);

                                                                                return newNodes.TryGetValue(exceptionName, out var replacement) ? replacement : rewritten;
                                                                            });

            return finalContent;
        }

        private static IEnumerable<XmlElementSyntax> GetExceptionXmls(DocumentationCommentTriviaSyntax comment) => GetXmlSyntax(Constants.XmlTag.Exception, comment);

        private static XmlElementSyntax CreateParaOr() => SyntaxFactory.XmlParaElement(SyntaxFactory.XmlText("-or-"));

        private static string GetReferencedExceptionName(XmlElementSyntax e) => e.GetAttributes<XmlCrefAttributeSyntax>().Select(__ => __.Cref.ToString()).FirstOrDefault();
    }
}