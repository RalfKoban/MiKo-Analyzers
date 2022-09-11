using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, SyntaxList<XmlNodeSyntax> content)
        {
            return comment.WithStartTag(comment.StartTag.WithoutTrivia().WithTrailingXmlComment())
                          .WithContent(content)
                          .WithEndTag(comment.EndTag.WithoutTrivia().WithLeadingXmlComment());
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, IEnumerable<XmlNodeSyntax> nodes) => Comment(comment, SyntaxFactory.List(nodes));

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text, string additionalComment = null)
        {
            return Comment(comment, text[0], additionalComment);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text, SyntaxList<XmlNodeSyntax> additionalComment)
        {
            return Comment(comment, text[0], additionalComment);
        }

        protected static T Comment<T>(T syntax, IEnumerable<string> terms, IEnumerable<KeyValuePair<string, string>> replacementMap) where T : SyntaxNode
        {
            var textMap = new Dictionary<XmlTextSyntax, XmlTextSyntax>();

            foreach (var text in syntax.DescendantNodes<XmlTextSyntax>())
            {
                var tokenMap = new Dictionary<SyntaxToken, SyntaxToken>();

                // replace token in text
                foreach (var token in text.TextTokens)
                {
                    var originalText = token.Text;

                    if (originalText.ContainsAny(terms))
                    {
                        var replacedText = replacementMap.Aggregate(originalText, (current, term) => current.Replace(term.Key, term.Value));

                        var newToken = token.WithText(replacedText);

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

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, SyntaxList<XmlNodeSyntax> additionalComment)
        {
            var end = CommentEnd(text, additionalComment.ToArray());

            return Comment(comment, end);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, string additionalComment = null)
        {
            return Comment(comment, XmlText(text + additionalComment));
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string commentStart, TypeSyntax type, string commentEnd)
        {
            return Comment(comment, commentStart, SeeCref(type), commentEnd);
        }

        protected static XmlElementSyntax Comment(
                                                XmlElementSyntax comment,
                                                string commentStart,
                                                XmlNodeSyntax link,
                                                string commentEnd,
                                                SyntaxList<XmlNodeSyntax> commendEndNodes)
        {
            return Comment(comment, commentStart, link, commentEnd, commendEndNodes.ToArray());
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, params XmlNodeSyntax[] nodes) => Comment(comment, SyntaxFactory.List(nodes));

        protected static XmlElementSyntax Comment(
                                                XmlElementSyntax comment,
                                                string commentStart,
                                                XmlNodeSyntax link,
                                                string commentEnd,
                                                params XmlNodeSyntax[] commendEndNodes)
        {
            var start = new[] { XmlText(commentStart), link };
            var end = CommentEnd(commentEnd, commendEndNodes);

            return Comment(comment, start.Concat(end));
        }

        protected static XmlElementSyntax Comment(
                                                XmlElementSyntax comment,
                                                string commentStart,
                                                XmlNodeSyntax link1,
                                                string commentMiddle,
                                                XmlNodeSyntax link2,
                                                string commentEnd,
                                                params XmlNodeSyntax[] commendEndNodes)
        {
            return Comment(comment, commentStart, link1, new[] { XmlText(commentMiddle) }, link2, commentEnd, commendEndNodes);
        }

        protected static XmlElementSyntax Comment(
                                                XmlElementSyntax comment,
                                                string commentStart,
                                                XmlNodeSyntax link1,
                                                IEnumerable<XmlNodeSyntax> commentMiddle,
                                                XmlNodeSyntax link2,
                                                string commentEnd,
                                                params XmlNodeSyntax[] commendEndNodes)
        {
            var start = new[] { XmlText(commentStart), link1 };
            var middle = new[] { link2 };
            var end = CommentEnd(commentEnd, commendEndNodes);

            // TODO RKN: Fix XML escaping caused by string conversion
            return Comment(comment, start.Concat(commentMiddle).Concat(middle).Concat(end));
        }

        protected static XmlElementSyntax CommentEndingWith(XmlElementSyntax comment, string ending)
        {
            var lastNode = comment.Content.LastOrDefault();
            if (lastNode is null)
            {
                // we have an empty comment
                return comment.AddContent(XmlText(ending));
            }

            if (lastNode is XmlTextSyntax t)
            {
                // we have a text at the end, so we have to find the text
                var textTokens = t.TextTokens;
                var lastToken = textTokens.Reverse().FirstOrDefault(_ => _.ValueText.IsNullOrWhiteSpace() is false);

                if (lastToken.IsKind(SyntaxKind.None))
                {
                    // seems like we have a <see cref/> or something with a CRLF at the end
                    var token = ending.ToSyntaxToken(SyntaxKind.XmlTextLiteralToken);

                    return comment.InsertTokensBefore(textTokens.First(), new[] { token });
                }
                else
                {
                    var valueText = lastToken.ValueText.TrimEnd();

                    // in case there is any, get rid of last dot
                    if (valueText.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                    {
                        valueText = valueText.WithoutSuffix(".");
                    }

                    return comment.ReplaceToken(lastToken, lastToken.WithText(valueText + ending));
                }
            }

            // we have a <see cref/> or something at the end
            return comment.InsertNodeAfter(lastNode, XmlText(ending));
        }

        protected static XmlElementSyntax CommentEndingWith(XmlElementSyntax comment, string commentStart, XmlEmptyElementSyntax seeCref, string commentContinue)
        {
            var lastNode = comment.Content.LastOrDefault();
            if (lastNode is null)
            {
                // we have an empty comment
                return comment.AddContent(XmlText(commentStart), seeCref, XmlText(commentContinue));
            }

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

                return comment.ReplaceNode(t, XmlText(text))
                              .AddContent(seeCref, XmlText(commentContinue).WithTrailingXmlComment());
            }

            // we have a <see cref/> or something at the end
            return comment.InsertNodeAfter(lastNode, XmlText(commentContinue));
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string[] phrases, FirstWordHandling firstWordHandling = FirstWordHandling.None) => CommentStartingWith(comment, phrases[0], firstWordHandling);

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string phrase, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            var content = CommentStartingWith(comment.Content, phrase, firstWordHandling);

            return SyntaxFactory.XmlElement(comment.StartTag, content, comment.EndTag);
        }

        protected static SyntaxList<XmlNodeSyntax> CommentStartingWith(SyntaxList<XmlNodeSyntax> content, string phrase, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            // when necessary adjust beginning text
            // Note: when on new line, then the text is not the 1st one but the 2nd one
            var index = GetIndex(content);
            if (index < 0)
            {
                return content.Add(XmlText(phrase));
            }

            if (content[index] is XmlTextSyntax text)
            {
                // we have to remove the element as otherwise we duplicate the comment
                content = content.Remove(text);

                if (phrase.IsNullOrWhiteSpace())
                {
                    text = text.WithoutTrailingXmlComment();
                }

                var newText = text.WithStartText(phrase, firstWordHandling);

                return content.Insert(index, newText);
            }

            return content.Insert(index, XmlText(phrase));
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string commentStart, XmlEmptyElementSyntax seeCref, string commentContinue)
        {
            var content = comment.Content;

            // when necessary adjust beginning text
            // Note: when on new line, then the text is not the 1st one but the 2nd one
            var index = GetIndex(content);

            var startText = XmlText(commentStart).WithLeadingXmlComment();

            XmlTextSyntax continueText;
            var syntax = content[index];
            if (syntax is XmlTextSyntax text)
            {
                // we have to remove the element as otherwise we duplicate the comment
                content = content.Remove(text);

                // remove first "\r\n" token and remove '  /// ' trivia of second token
                var textTokens = text.TextTokens;
                if (textTokens[0].IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    var newTokens = textTokens.RemoveAt(0);
                    text = XmlText(newTokens.Replace(newTokens[0], newTokens[0].WithLeadingTrivia()));
                }

                continueText = text.WithStartText(commentContinue);
            }
            else
            {
                if (index == 1 && content[0].IsWhiteSpaceOnlyText())
                {
                    // seems that the non-text element is the first element, so we should remove the empty text element before
                    content = content.RemoveAt(0);
                }

                index = Math.Max(0, index - 1);
                continueText = XmlText(commentContinue);
            }

            var newContent = content.Insert(index, startText)
                                     .Insert(index + 1, seeCref)
                                     .Insert(index + 2, continueText);

            return SyntaxFactory.XmlElement(comment.StartTag, newContent, comment.EndTag);
        }

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

        protected static string GetParameterName(XmlElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>().First().Identifier.GetName();

        protected static string GetParameterName(XmlEmptyElementSyntax syntax) => syntax.Attributes.OfType<XmlNameAttributeSyntax>().First().Identifier.GetName();

        protected static XmlCrefAttributeSyntax GetSeeCref(SyntaxNode value) => value.GetCref(Constants.XmlTag.See);

        protected static DocumentationCommentTriviaSyntax GetXmlSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.Select(_ => _.GetDocumentationCommentTriviaSyntax())
                                                                                                                          .FirstOrDefault(_ => _ != null);

        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(string startTag, IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.SelectMany(_ => _.GetXmlSyntax(startTag));

        protected static XmlEmptyElementSyntax Inheritdoc() => SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Inheritdoc);

        protected static XmlEmptyElementSyntax Inheritdoc(XmlCrefAttributeSyntax cref) => Inheritdoc().WithAttributes(new SyntaxList<XmlAttributeSyntax>(cref));

        protected static bool IsSeeCref(SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(emptyElement.Attributes);
                }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax) => syntax.FirstOrDefault() is XmlCrefAttributeSyntax;
        }

        protected static bool IsSeeCref(SyntaxNode value, string type)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(emptyElement.Attributes, type);
                }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes, type);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, string content) => syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute && attribute.Cref.ToString() == content;
        }

        protected static bool IsSeeCref(SyntaxNode value, TypeSyntax type)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(emptyElement.Attributes, type);
                }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes, type);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, TypeSyntax t)
            {
                if (syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute)
                {
                    if (attribute.Cref is NameMemberCrefSyntax m)
                    {
                        return t is GenericNameSyntax
                                   ? IsSameGeneric(m.Name, t)
                                   : IsSameName(m.Name, t);
                    }
                }

                return false;
            }
        }

        protected static bool IsSeeCref(SyntaxNode value, TypeSyntax type, NameSyntax member)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(emptyElement.Attributes, type, member);
                }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.StartTag.Attributes, type, member);
                }

                default:
                {
                    return false;
                }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, TypeSyntax t, NameSyntax name)
            {
                if (syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute)
                {
                    if (attribute.Cref is QualifiedCrefSyntax q && IsSameGeneric(q.Container, t))
                    {
                        if (q.Member is NameMemberCrefSyntax m && IsSameName(m.Name, name))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        protected static XmlElementSyntax MakeFirstWordInfiniteVerb(XmlElementSyntax syntax)
        {
            if (syntax.Content.FirstOrDefault() is XmlTextSyntax text)
            {
                var modifiedText = MakeFirstWordInfiniteVerb(text);

                if (ReferenceEquals(text, modifiedText) is false)
                {
                    return syntax.ReplaceNode(text, modifiedText);
                }
            }

            return syntax;
        }

        protected static XmlEmptyElementSyntax Para() => SyntaxFactory.XmlEmptyElement(Constants.XmlTag.Para);

        protected static XmlElementSyntax Para(string text) => SyntaxFactory.XmlParaElement(XmlText(text));

        protected static XmlElementSyntax Para(SyntaxList<XmlNodeSyntax> nodes) => SyntaxFactory.XmlParaElement(nodes);

        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string[] comments) => ParameterComment(parameter, comments[0]);

        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string comment) => Comment(SyntaxFactory.XmlParamElement(parameter.GetName()), comment);

        protected static XmlEmptyElementSyntax ParamRef(ParameterSyntax parameter) => ParamRef(parameter.GetName());

        protected static XmlEmptyElementSyntax ParamRef(string parameterName)
        {
            var name = SyntaxFactory.XmlNameAttribute(parameterName);

            return SyntaxFactory.XmlEmptyElement(Constants.XmlTag.ParamRef).WithAttribute(name);
        }

        protected static XmlElementSyntax ParaOr() => Para(Constants.Comments.SpecialOrPhrase);

        protected static XmlElementSyntax RemoveBooleansTags(XmlElementSyntax comment) => comment.Without(comment.Content.Where(_ => _.IsBooleanTag()));

        protected static T ReplaceText<T>(T comment, XmlTextSyntax text, string phrase, string replacement) where T : SyntaxNode => ReplaceText(comment, text, new[] { phrase }, replacement);

        protected static T ReplaceText<T>(T comment, XmlTextSyntax text, string[] phrases, string replacement) where T : SyntaxNode
        {
            var modifiedText = text.ReplaceText(phrases, replacement);

            return ReferenceEquals(text, modifiedText)
                       ? comment
                       : comment.ReplaceNode(text, modifiedText);
        }

        protected static XmlEmptyElementSyntax SeeCref(string typeName) => Cref(Constants.XmlTag.See, SyntaxFactory.ParseTypeName(typeName));

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type) => Cref(Constants.XmlTag.See, type);

        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, NameSyntax member) => Cref(Constants.XmlTag.See, type, member);

        protected static XmlEmptyElementSyntax SeeLangword(string text)
        {
            var token = text.ToSyntaxToken();
            var attribute = SyntaxFactory.XmlTextAttribute(Constants.XmlTag.Attribute.Langword, token);

            return SyntaxFactory.XmlEmptyElement(Constants.XmlTag.See).WithAttribute(attribute);
        }

        protected static XmlEmptyElementSyntax SeeLangword_False() => SeeLangword("false");

        protected static XmlEmptyElementSyntax SeeLangword_Null() => SeeLangword("null");

        protected static XmlEmptyElementSyntax SeeLangword_True() => SeeLangword("true");

        protected static XmlElementSyntax XmlElement(string tag) => SyntaxFactory.XmlElement(tag, default);

        protected static XmlElementSyntax XmlElement(string tag, XmlNodeSyntax content) => SyntaxFactory.XmlElement(tag, new SyntaxList<XmlNodeSyntax>(content));

        protected static XmlElementSyntax XmlElement(string tag, IEnumerable<XmlNodeSyntax> contents) => SyntaxFactory.XmlElement(tag, new SyntaxList<XmlNodeSyntax>(contents));

        protected static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        protected static XmlTextSyntax XmlText(SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

        protected static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(SyntaxFactory.TokenList(textTokens));

        private static IEnumerable<XmlNodeSyntax> CommentEnd(string commentEnd, params XmlNodeSyntax[] commendEndNodes)
        {
            var skip = 0;
            XmlTextSyntax textCommentEnd;

            if (commendEndNodes.FirstOrDefault() is XmlTextSyntax text)
            {
                skip = 1;

                var textTokens = text.TextTokens.ToList();
                var textToken = textTokens.First(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken));
                var removals = textTokens.IndexOf(textToken);

                textTokens.RemoveRange(0, removals + 1);

                var replacementText = commentEnd + textToken.ValueText.TrimStart().ToLowerCaseAt(0);
                var replacement = replacementText.ToSyntaxToken();
                textTokens.Insert(0, replacement);

                textCommentEnd = XmlText(new SyntaxTokenList(textTokens).WithoutLastXmlNewLine());
            }
            else
            {
                textCommentEnd = XmlText(commentEnd);
            }

            // if there are more than 1 item contained, also remove the new line and /// from the last item
            if (commendEndNodes.Length > 1)
            {
                if (commendEndNodes.Last() is XmlTextSyntax additionalText)
                {
                    commendEndNodes[commendEndNodes.Length - 1] = XmlText(additionalText.TextTokens.WithoutLastXmlNewLine());
                }
            }

            var result = new List<XmlNodeSyntax>(commendEndNodes.Length);
            result.Add(textCommentEnd);
            result.AddRange(commendEndNodes.Skip(skip));

            return result;
        }

        private static XmlEmptyElementSyntax Cref(string tag, CrefSyntax syntax) => SyntaxFactory.XmlEmptyElement(tag).WithAttribute(SyntaxFactory.XmlCrefAttribute(syntax));

        private static int GetIndex(SyntaxList<XmlNodeSyntax> content)
        {
            if (content.Count == 0)
            {
                return -1;
            }

            return content[0].IsWhiteSpaceOnlyText() && content.Count > 1
                       ? 1
                       : 0;
        }

        private static bool IsSameGeneric(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is GenericNameSyntax g1 && t2 is GenericNameSyntax g2)
            {
                if (g1.Identifier.ValueText == g2.Identifier.ValueText)
                {
                    var arguments1 = g1.TypeArgumentList.Arguments;
                    var arguments2 = g2.TypeArgumentList.Arguments;

                    if (arguments1.Count == arguments2.Count)
                    {
                        for (var i = 0; i < arguments1.Count; i++)
                        {
                            if (IsSameName(arguments1[i], arguments2[i]) is false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsSameName(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is IdentifierNameSyntax n1 && t2 is IdentifierNameSyntax n2)
            {
                return n1.Identifier.ValueText == n2.Identifier.ValueText;
            }

            return t1.ToString() == t2.ToString();
        }

        private static XmlTextSyntax MakeFirstWordInfiniteVerb(XmlTextSyntax text)
        {
            foreach (var token in text.TextTokens)
            {
                var valueText = token.ValueText;

                if (valueText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                // first word
                var firstWord = valueText.FirstWord();
                var infiniteVerb = Verbalizer.MakeInfiniteVerb(firstWord);

                if (firstWord != infiniteVerb)
                {
                    return text.ReplaceToken(token, token.WithText(infiniteVerb + valueText.WithoutFirstWord()));
                }
            }

            return text;
        }
    }
}