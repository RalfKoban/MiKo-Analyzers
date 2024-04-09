using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2059_CodeFixProvider)), Shared]
    public sealed class MiKo_2059_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2059";

        protected override string Title => Resources.MiKo_2059_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var exceptionComments = syntax.GetExceptionXmls();

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
                foreach (var duplicate in duplicates)
                {
                    obsoleteNodes.Add(duplicate);

                    // find the remaining ' /// ' text nodes and mark them as obsolete as they are not part of the elements
                    var previousSibling = duplicate.PreviousSibling();

                    if (previousSibling is XmlTextSyntax)
                    {
                        obsoleteNodes.Add(previousSibling);
                    }

                    newContent = newContent
                                        .Add(ParaOr().WithLeadingXmlComment())
                                        .AddRange(duplicate.Content.WithLeadingXmlComment());
                }

                var consolidatedException = exception.WithContent(newContent.WithLeadingXmlComment().WithTrailingXmlComment());

                // remember the modified node
                newNodes.Add(entry.Key, consolidatedException);
            }

            var commentWithoutObsoleteNodes = syntax.Without(obsoleteNodes);

            // find and replace the nodes with the new fixed ones
            var finalContent = commentWithoutObsoleteNodes.ReplaceNodes(
                                                                    commentWithoutObsoleteNodes.GetExceptionXmls(),
                                                                    (original, rewritten) =>
                                                                                            {
                                                                                                var exceptionName = GetReferencedExceptionName(original);

                                                                                                return newNodes.TryGetValue(exceptionName, out var replacement) ? replacement : rewritten;
                                                                                            });

            return finalContent;
        }

        private static string GetReferencedExceptionName(XmlElementSyntax e) => e.GetAttributes<XmlCrefAttributeSyntax>().Select(__ => __.Cref.ToString()).FirstOrDefault();
    }
}