using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(string startTag, IEnumerable<SyntaxNode> syntaxNodes)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true))
                              .OfType<XmlElementSyntax>()
                              .Where(_ => _.StartTag.Name.LocalName.ValueText == startTag);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text) => comment.WithContent(new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText(text[0])));

        protected static XmlElementSyntax CommentWithSeeCRef(XmlElementSyntax comment, string commentStart, TypeSyntax type, string commentEnd)
        {
            var content = SyntaxFactory.List<XmlNodeSyntax>()
                                       .Add(SyntaxFactory.XmlText(commentStart))
                                       .Add(Cref(Constants.XmlTag.See, type))
                                       .Add(SyntaxFactory.XmlText(commentEnd));

            return comment.WithContent(content);
        }

        private static XmlEmptyElementSyntax Cref(string name, TypeSyntax type)
        {
            var attribute = SyntaxFactory.XmlCrefAttribute(SyntaxFactory.TypeCref(type.WithoutTrailingTrivia()));

            return SyntaxFactory.XmlEmptyElement(name)
                                .WithAttributes(new SyntaxList<XmlAttributeSyntax>(attribute));
        }
    }
}