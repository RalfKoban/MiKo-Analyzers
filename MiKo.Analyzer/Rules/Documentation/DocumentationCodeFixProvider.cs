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

        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(string startTag, IEnumerable<SyntaxNode> syntaxNodes)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true).OfType<XmlElementSyntax>())
                              .Where(_ => _.GetName() == startTag);
        }

        /// <summary>
        /// Only gets the XML elements that are NOT empty (have some content) and the given tag out of the list of syntax nodes.
        /// </summary>
        /// <param name="startTag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <param name="syntaxNodes">
        /// The starting points of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the XML elements that are NOT empty (have some content) and the given tag out of the list of syntax nodes.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode, IEnumerable{string})"/>
        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(string startTag, params SyntaxNode[] syntaxNodes)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true).OfType<XmlElementSyntax>())
                              .Where(_ => _.GetName() == startTag);
        }

        /// <summary>
        /// Only gets the XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </summary>
        /// <param name="syntaxNode">
        /// The starting point of the XML elements to consider.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </returns>
        /// <seealso cref="GetXmlSyntax(string, SyntaxNode[])"/>
        protected static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(SyntaxNode syntaxNode, IEnumerable<string> tags)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNode.DescendantNodes(__ => true, true).OfType<XmlEmptyElementSyntax>()
                             .Where(_ => tags.Contains(_.GetName()));
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string phrase)
        {
            var content = CommentStartingWith(comment.Content, phrase);

            return SyntaxFactory.XmlElement(comment.StartTag, content, comment.EndTag);
        }

        protected static SyntaxList<XmlNodeSyntax> CommentStartingWith(SyntaxList<XmlNodeSyntax> content, string phrase)
        {
            // when necessary adjust beginning text
            // Note: when on new line, then the text is not the 1st one but the 2nd one
            var index = GetIndex(content);
            if (index < 0)
            {
                return content.Add(SyntaxFactory.XmlText(phrase));
            }

            if (content[index] is XmlTextSyntax text)
            {
                // we have to remove the element as otherwise we duplicate the comment
                content = content.Remove(content[index]);

                if (phrase.IsNullOrWhiteSpace())
                {
                    var textTokens = text.TextTokens.Count;
                    if (textTokens > 2)
                    {
                        // TODO: RKN find a better solution this as it is not good code

                        // remove last "\r\n" token and remove '  /// ' trivia of last token
                        if (text.TextTokens[textTokens - 2].IsKind(SyntaxKind.XmlTextLiteralNewLineToken)
                         && text.TextTokens[textTokens - 1].ValueText.IsNullOrWhiteSpace())
                        {
                            var newTokens = text.TextTokens.Take(textTokens - 2).ToArray();
                            text = SyntaxFactory.XmlText(newTokens);
                        }
                    }
                }

                var newText = text.WithStartText(phrase);

                return content.Insert(index, newText);
            }

            return content.Insert(index, SyntaxFactory.XmlText(phrase));
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
                index = Math.Max(0, index - 1);
                continueText = SyntaxFactory.XmlText(commentContinue);
            }

            return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            content.Insert(index, startText).Insert(index + 1, seeCref).Insert(index + 2, continueText),
                                            comment.EndTag);
        }

        protected static XmlElementSyntax CommentEndingWith(XmlElementSyntax comment, string ending)
        {
            var lastNode = comment.Content.Last();
            if (lastNode is XmlTextSyntax t)
            {
                // we have a text at the end, so we have to find the text
                var lastToken = t.TextTokens.Reverse().FirstOrDefault(_ => _.ValueText.IsNullOrWhiteSpace() is false);

                if (lastToken.IsKind(SyntaxKind.None))
                {
                    // seems like we have a <see cref/> or something with a CRLF at the end
                    var token = SyntaxFactory.Token(default, SyntaxKind.XmlTextLiteralToken, ending, ending, default);
                    return comment.InsertTokensBefore(t.TextTokens.First(), new[] { token });
                }
                else
                {
                    var valueText = lastToken.ValueText.TrimEnd();

                    // in case there is any, get rid of last dot
                    if (valueText.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                    {
                        valueText = valueText.WithoutSuffix(".");
                    }

                    var text = valueText + ending;
                    var token = SyntaxFactory.Token(lastToken.LeadingTrivia, SyntaxKind.XmlTextLiteralToken, text, text, lastToken.TrailingTrivia);

                    return comment.ReplaceToken(lastToken, token);
                }
            }

            // we have a <see cref/> or something at the end
            return comment.InsertNodeAfter(lastNode, SyntaxFactory.XmlText(ending));
        }

        protected static XmlElementSyntax CommentEndingWith(XmlElementSyntax comment, string commentStart, XmlEmptyElementSyntax seeCref, string commentContinue)
        {
            var lastNode = comment.Content.Last();
            if (lastNode is XmlTextSyntax t)
            {
                var text = commentStart;

                // we have a text at the end, so we have to find the text
                var lastToken = t.TextTokens.Reverse().FirstOrDefault(_ => _.ValueText.IsNullOrWhiteSpace() is false);

                if (lastToken.IsKind(SyntaxKind.None))
                {
                    // seems like we have a <see cref/> or something with a CRLF at the end
                }
                else
                {
                    var valueText = lastToken.ValueText.TrimEnd();

                    // in case there is any, get rid of last dot
                    if (valueText.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                    {
                        valueText = valueText.WithoutSuffix(".");
                    }

                    text = valueText + commentStart;
                }

                return comment.ReplaceNode(t, SyntaxFactory.XmlText(text))
                              .AddContent(seeCref, SyntaxFactory.XmlText(commentContinue).WithTrailingXmlComment());
            }

            // we have a <see cref/> or something at the end
            return comment.InsertNodeAfter(lastNode, SyntaxFactory.XmlText(commentContinue));
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text, string additionalComment = null)
        {
            return Comment(comment, text[0], additionalComment);
        }

        protected static T Comment<T>(T syntax, IEnumerable<string> terms, IEnumerable<KeyValuePair<string, string>> replacementMap) where T : SyntaxNode
        {
            var textMap = new Dictionary<XmlTextSyntax, XmlTextSyntax>();

            foreach (var text in syntax.DescendantNodes().OfType<XmlTextSyntax>())
            {
                var tokenMap = new Dictionary<SyntaxToken, SyntaxToken>();

                // replace token in text
                foreach (var token in text.TextTokens)
                {
                    var originalText = token.Text;

                    if (originalText.ContainsAny(terms))
                    {
                        var replacedText = replacementMap.Aggregate(originalText, (current, term) => current.Replace(term.Key, term.Value));

                        var newToken = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), replacedText, replacedText, token.TrailingTrivia);

                        tokenMap.Add(token, newToken);
                    }
                }

                if (tokenMap.Any())
                {
                    var newText = text.ReplaceTokens(tokenMap.Keys, (_, __) => tokenMap[_]);
                    textMap.Add(text, newText);
                }
            }

            if (textMap.Any())
            {
                var result = syntax.ReplaceNodes(textMap.Keys, (_, __) => textMap[_]);
                return result;
            }

            return syntax;
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

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, params XmlNodeSyntax[] nodes)
        {
            return comment
                   .WithStartTag(comment.StartTag.WithoutTrivia().WithTrailingXmlComment())
                   .WithContent(SyntaxFactory.List(nodes))
                   .WithEndTag(comment.EndTag.WithLeadingXmlComment());
        }

        protected static XmlEmptyElementSyntax Para() => SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Para);

        protected static XmlElementSyntax Para(SyntaxList<XmlNodeSyntax> nodes) => SyntaxFactory.XmlElement(Constants.XmlTag.Para, nodes);

        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string[] comments) => ParameterComment(parameter, comments[0]);

        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string comment) => Comment(SyntaxFactory.XmlParamElement(parameter.GetName()), comment);

        protected static XmlEmptyElementSyntax SeeLangword_Null() => SeeLangword("null");

        protected static XmlEmptyElementSyntax SeeLangword_True() => SeeLangword("true");

        protected static XmlEmptyElementSyntax SeeLangword_False() => SeeLangword("false");

        protected static XmlEmptyElementSyntax SeeLangword(string text)
        {
            var token = SyntaxFactory.Token(default, SyntaxKind.StringLiteralToken, text, text, default);
            var attribute = SyntaxFactory.XmlTextAttribute("langword", token);

            return SyntaxFactory.XmlEmptyElement(Constants.XmlTag.See).WithAttributes(new SyntaxList<XmlAttributeSyntax>(attribute));
        }

        protected static XmlEmptyElementSyntax SeeCref(string typeName) => Cref(Constants.XmlTag.See, SyntaxFactory.ParseTypeName(typeName));

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type) => Cref(Constants.XmlTag.See, type);

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, NameSyntax member) => Cref(Constants.XmlTag.See, type, member);

        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type)
        {
            // fix trivia, to avoid situation as reported in https://github.com/dotnet/roslyn/issues/47550
            return Cref(tag, SyntaxFactory.TypeCref(type.WithoutTrivia()));
        }

        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type, NameSyntax member)
        {
            // fix trivia, to avoid situation as reported in https://github.com/dotnet/roslyn/issues/47550
            return Cref(tag, SyntaxFactory.QualifiedCref(type.WithoutTrivia(), SyntaxFactory.NameMemberCref(member.WithoutTrivia())));
        }

        private static XmlEmptyElementSyntax Cref(string tag, CrefSyntax syntax)
        {
            return SyntaxFactory.XmlEmptyElement(tag).WithAttributes(new SyntaxList<XmlAttributeSyntax>(SyntaxFactory.XmlCrefAttribute(syntax)));
        }

        private static int GetIndex(SyntaxList<XmlNodeSyntax> content)
        {
            if (content.Count == 0)
            {
                return -1;
            }

            var onlyWhitespaceText = content[0] is XmlTextSyntax t && GetText(t).IsNullOrWhiteSpace();
            return onlyWhitespaceText && content.Count > 1 ? 1 : 0;
        }

        private static string GetText(XmlTextSyntax text)
        {
            return string.Concat(text.TextTokens.Select(_ => _.WithoutTrivia()));
        }
    }
}