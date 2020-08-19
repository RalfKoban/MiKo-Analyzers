using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        protected static DocumentationCommentTriviaSyntax GetXmlSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true).OfType<DocumentationCommentTriviaSyntax>()).FirstOrDefault();
        }

        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(IEnumerable<SyntaxNode> syntaxNodes, string startTag)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true).OfType<XmlElementSyntax>())
                              .Where(_ => _.StartTag.Name.LocalName.ValueText == startTag);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text, string additionalComment = null)
        {
            return Comment(comment, text[0], additionalComment);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, string additionalComment = null)
        {
            var content = SyntaxFactory.List<XmlNodeSyntax>().Add(SyntaxFactory.XmlText(text + additionalComment));

            return comment
                   .WithStartTag(comment.StartTag.WithoutTrivia().WithTrailingXmlComment())
                   .WithContent(content)
                   .WithEndTag(comment.EndTag.WithoutTrivia().WithLeadingXmlComment());
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string commentStart, TypeSyntax type, string commentEnd)
        {
            return Comment(comment, commentStart, SeeCref(type), commentEnd);
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

            return comment
                   .WithStartTag(comment.StartTag.WithoutTrivia().WithTrailingXmlComment())
                   .WithContent(content)
                   .WithEndTag(comment.EndTag.WithoutTrivia().WithLeadingXmlComment());
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

            return comment
                   .WithStartTag(comment.StartTag.WithoutTrivia().WithTrailingXmlComment())
                   .WithContent(content)
                   .WithEndTag(comment.EndTag.WithLeadingXmlComment());
        }

        protected static XmlEmptyElementSyntax SeeCref(string typeName) => Cref(Constants.XmlTag.See, SyntaxFactory.ParseTypeName(typeName));

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type) => Cref(Constants.XmlTag.See, type);

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, NameSyntax member) => Cref(Constants.XmlTag.See, type, member);

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