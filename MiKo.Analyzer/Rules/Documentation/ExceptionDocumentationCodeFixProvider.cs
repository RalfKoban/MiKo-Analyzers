using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationCodeFixProvider : OverallDocumentationCodeFixProvider
    {
        protected static IEnumerable<XmlElementSyntax> GetExceptionXmls(DocumentationCommentTriviaSyntax comment) => GetXmlSyntax(Constants.XmlTag.Exception, comment);

        protected DocumentationCommentTriviaSyntax FixComment(Document document, SyntaxNode syntax, DocumentationCommentTriviaSyntax comment)
        {
            return comment.Content.OfType<XmlElementSyntax>()
                          .Where(_ => _.IsException())
                          .Select(e => FixExceptionComment(document, syntax, e, comment))
                          .FirstOrDefault(fix => fix != null);
        }

        protected virtual DocumentationCommentTriviaSyntax FixExceptionComment(Document document, SyntaxNode syntax, XmlElementSyntax exception, DocumentationCommentTriviaSyntax comment) => null;
    }
}