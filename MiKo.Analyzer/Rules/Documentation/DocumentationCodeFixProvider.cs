using System;
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

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string phrase)
        {
            var content = comment.Content;

            // when necessary adjust beginning text
            // Note: when on new line, then the text is not the 1st one but the 2nd one
            var index = GetIndex(content);

            XmlTextSyntax newText;
            if (content[index] is XmlTextSyntax text)
            {
                // we have to remove the element as otherwise we duplicate the comment
                content = content.Remove(content[index]);
                newText = text.WithStartText(phrase);
            }
            else
            {
                newText = SyntaxFactory.XmlText(phrase);
            }

            return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            content.Insert(index, newText),
                                            comment.EndTag);
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string commentStart, XmlEmptyElementSyntax seeCref, string commentContinue)
        {
            var content = comment.Content;

            // when necessary adjust beginning text
            // Note: when on new line, then the text is not the 1st one but the 2nd one
            var index = GetIndex(content);

            var startText = SyntaxFactory.XmlText(commentStart).WithLeadingXmlComment();

            XmlTextSyntax continueText;
            if (content[index] is XmlTextSyntax text)
            {
                // we have to remove the element as otherwise we duplicate the comment
                content = content.Remove(content[index]);

                // remove first "\r\n" token and remove '  /// ' trivia of second token
                if (text.TextTokens[0].IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    var newTokens = text.TextTokens.RemoveAt(0);
                    text = SyntaxFactory.XmlText(newTokens.Replace(newTokens[0], newTokens[0].WithLeadingTrivia()));
                }

                continueText = text.WithStartText(commentContinue);
            }
            else
            {
                continueText = SyntaxFactory.XmlText(commentContinue);
            }

            return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            content.Insert(index, startText).Insert(index + 1, seeCref).Insert(index + 2, continueText),
                                            comment.EndTag);
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

        protected static SyntaxNode Comment(DocumentationCommentTriviaSyntax syntax, string[] terms, IEnumerable<KeyValuePair<string, string>> replacementMap)
        {
            var result = syntax;

            foreach (var text in syntax.DescendantNodes().OfType<XmlTextSyntax>())
            {
                var newText = text;

                // replace token in text
                foreach (var token in text.TextTokens)
                {
                    var originalText = token.Text;

                    if (originalText.ContainsAny(terms))
                    {
                        var replacedText = originalText;

                        foreach (var term in replacementMap)
                        {
                            replacedText = replacedText.Replace(term.Key, term.Value);
                        }

                        var newToken = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), replacedText, replacedText, token.TrailingTrivia);

                        newText = newText.ReplaceToken(token, newToken);
                    }
                }

                result = result.ReplaceNode(text, newText);
            }

            return result;
        }

        protected static XmlEmptyElementSyntax SeeLangword(string text)
        {
            var token = SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, text, text, default);
            var attribute = SyntaxFactory.XmlTextAttribute("langword", token);

            return SyntaxFactory.XmlEmptyElement(Constants.XmlTag.See).WithAttributes(new SyntaxList<XmlAttributeSyntax>(attribute));
        }

        protected static XmlEmptyElementSyntax SeeCref(string typeName) => Cref(Constants.XmlTag.See, SyntaxFactory.ParseTypeName(typeName));

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type) => Cref(Constants.XmlTag.See, type);

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, NameSyntax member) => Cref(Constants.XmlTag.See, type, member);

        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type) => Cref(tag, SyntaxFactory.TypeCref(type.WithoutTrailingTrivia()));

        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type, NameSyntax member)
        {
            return Cref(tag, SyntaxFactory.QualifiedCref(type, SyntaxFactory.NameMemberCref(member)));
        }

        private static XmlEmptyElementSyntax Cref(string tag, CrefSyntax syntax)
        {
            return SyntaxFactory.XmlEmptyElement(tag).WithAttributes(new SyntaxList<XmlAttributeSyntax>(SyntaxFactory.XmlCrefAttribute(syntax)));
        }

        private static int GetIndex(SyntaxList<XmlNodeSyntax> content)
        {
            var onlyWhitespaceText = content[0] is XmlTextSyntax t && GetText(t).IsNullOrWhiteSpace();
            return onlyWhitespaceText ? 1 : 0;
        }

        private static string GetText(XmlTextSyntax text)
        {
            return string.Concat(text.TextTokens.Select(_ => _.WithoutTrivia()));
        }
    }
}