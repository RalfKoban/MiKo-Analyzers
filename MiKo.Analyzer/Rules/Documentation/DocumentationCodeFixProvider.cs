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
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true).OfType<XmlElementSyntax>())
                              .Where(_ => _.StartTag.Name.LocalName.ValueText == startTag);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text)
        {
            return comment.WithContent(new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText(text[0])));
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string commentStart, TypeSyntax type, string commentEnd)
        {
            return Comment(comment, commentStart, Cref(Constants.XmlTag.See, type), commentEnd);
        }

        protected static XmlElementSyntax Comment(
                                                XmlElementSyntax comment,
                                                string commentStart,
                                                XmlEmptyElementSyntax seeCref,
                                                string commentEnd)
        {
            var content = SyntaxFactory.List<XmlNodeSyntax>()
                                       .Add(SyntaxFactory.XmlText(commentStart))
                                       .Add(seeCref)
                                       .Add(SyntaxFactory.XmlText(commentEnd));

            return comment.WithContent(content);
        }

        protected static XmlElementSyntax Comment(
                                                XmlElementSyntax comment,
                                                string commentStart,
                                                XmlEmptyElementSyntax seeCref1,
                                                string commentMiddle,
                                                XmlEmptyElementSyntax seeCref2,
                                                string commentEnd)
        {
            var content = SyntaxFactory.List<XmlNodeSyntax>()
                                       .Add(SyntaxFactory.XmlText(commentStart))
                                       .Add(seeCref1)
                                       .Add(SyntaxFactory.XmlText(commentMiddle))
                                       .Add(seeCref2)
                                       .Add(SyntaxFactory.XmlText(commentEnd));

            return comment.WithContent(content);
        }

        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type)
        {
            return Cref(tag, SyntaxFactory.TypeCref(type.WithoutTrailingTrivia()));
        }

        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type, NameSyntax member)
        {
            return Cref(tag, SyntaxFactory.QualifiedCref(type, SyntaxFactory.NameMemberCref(member)));
        }

        private static XmlEmptyElementSyntax Cref(string tag, CrefSyntax syntax)
        {
            return SyntaxFactory.XmlEmptyElement(tag).WithAttributes(new SyntaxList<XmlAttributeSyntax>(SyntaxFactory.XmlCrefAttribute(syntax)));
        }
    }
}