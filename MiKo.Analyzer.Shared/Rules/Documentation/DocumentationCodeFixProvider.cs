using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        protected static string GetStartingPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.StartingPhrase, out var s) ? s : string.Empty;

        protected static string GetEndingPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.EndingPhrase, out var s) ? s : string.Empty;

        protected static string GetPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.Phrase, out var s) ? s : string.Empty;

//// ncrunch: rdi off
//// ncrunch: no coverage start

        protected static string[] GetTermsForQuickLookup(IEnumerable<string> terms)
        {
            var orderedTerms = terms.ToHashSet(_ => _.ToUpperInvariant())
                                    .OrderBy(_ => _.Length)
                                    .ThenBy(_ => _);

            var lookupTerms = new List<string>();

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var term in orderedTerms)
            {
                if (term.StartsWithAny(lookupTerms, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                lookupTerms.Add(term);
            }

            return lookupTerms.ToArray();
        }

//// ncrunch: no coverage end
//// ncrunch: rdi default

        protected static XmlElementSyntax C(string text)
        {
            return SyntaxFactory.XmlElement(Constants.XmlTag.C, new SyntaxList<XmlNodeSyntax>(XmlText(text)));
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, SyntaxList<XmlNodeSyntax> content)
        {
            var result = comment.WithStartTag(comment.StartTag.WithoutTrivia().WithTrailingXmlComment())
                                .WithContent(content)
                                .WithEndTag(comment.EndTag.WithoutTrivia().WithLeadingXmlComment());

            return CombineTexts(result);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, IEnumerable<XmlNodeSyntax> nodes) => Comment(comment, nodes.ToSyntaxList());

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text, string additionalComment = null)
        {
            return Comment(comment, text[0], additionalComment);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text, SyntaxList<XmlNodeSyntax> additionalComment)
        {
            return Comment(comment, text[0], additionalComment);
        }

//// ncrunch: rdi off

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, SyntaxList<XmlNodeSyntax> additionalComment)
        {
            var end = CommentEnd(text, additionalComment.ToArray());

            return Comment(comment, end);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, string additionalComment = null)
        {
            return Comment(comment, XmlText(text + additionalComment));
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax syntax, IReadOnlyCollection<string> terms, IEnumerable<KeyValuePair<string, string>> replacementMap, FirstWordHandling firstWordHandling = FirstWordHandling.KeepLeadingSpace)
        {
            var result = Comment<XmlElementSyntax>(syntax, terms, replacementMap, firstWordHandling);

            return CombineTexts(result);
        }

        protected static T Comment<T>(T syntax, IReadOnlyCollection<string> terms, IEnumerable<KeyValuePair<string, string>> replacementMap, FirstWordHandling firstWordHandling = FirstWordHandling.KeepLeadingSpace) where T : SyntaxNode
        {
            var minimumLength = terms.Min(_ => _.Length);

            var textMap = CreateReplacementTextMap(minimumLength);

            if (textMap is null)
            {
                // nothing found, so nothing to replace
                return syntax;
            }

            var result = syntax.ReplaceNodes(textMap.Keys, (_, __) => textMap[_]);

            return result;

            Dictionary<XmlTextSyntax, XmlTextSyntax> CreateReplacementTextMap(int minLength)
            {
                Dictionary<XmlTextSyntax, XmlTextSyntax> map = null;

                foreach (var text in syntax.DescendantNodes<XmlTextSyntax>())
                {
                    Dictionary<SyntaxToken, SyntaxToken> tokenMap = null;

                    // replace token in text
                    var textTokens = text.TextTokens;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var textTokensCount = textTokens.Count;

                    for (var index = 0; index < textTokensCount; index++)
                    {
                        var token = textTokens[index];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            continue;
                        }

                        var originalText = token.Text;

                        if (originalText.Length < minLength)
                        {
                            // length is smaller than minimum provided, so no replacement possible
                            continue;
                        }

                        if (originalText.ContainsAny(terms, StringComparison.OrdinalIgnoreCase))
                        {
                            var replacedText = new StringBuilder(originalText).ReplaceAllWithCheck(replacementMap)
                                                                              .ToString()
                                                                              .AdjustFirstWord(firstWordHandling);

                            if (originalText.Equals(replacedText))
                            {
                                // replacement with itself does not make any sense
                                continue;
                            }

                            var newToken = token.WithText(replacedText);

                            if (tokenMap is null)
                            {
                                tokenMap = new Dictionary<SyntaxToken, SyntaxToken>();
                            }

                            tokenMap.Add(token, newToken);
                        }
                    }

                    if (tokenMap is null)
                    {
                        // nothing found, so nothing to replace
                    }
                    else
                    {
                        var newText = text.ReplaceTokens(tokenMap.Keys, (_, __) => tokenMap[_]);

                        if (map is null)
                        {
                            map = new Dictionary<XmlTextSyntax, XmlTextSyntax>();
                        }

                        map.Add(text, newText);
                    }
                }

                return map;
            }
        }

//// ncrunch: rdi default

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

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, params XmlNodeSyntax[] nodes) => Comment(comment, nodes.ToSyntaxList());

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
                var lastToken = textTokens.Reverse().FirstOrDefault(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken) && _.ValueText.IsNullOrWhiteSpace() is false);

                if (lastToken.IsDefaultValue())
                {
                    // seems like we have a <see cref/> or something with a CRLF at the end
                    var token = ending.AsToken(SyntaxKind.XmlTextLiteralToken);

                    return comment.InsertTokensBefore(textTokens.First(), new[] { token });
                }
                else
                {
                    // in case there is any, get rid of last dot
                    var valueText = lastToken.ValueText.AsSpan().TrimEnd().TrimEnd('.').ConcatenatedWith(ending);

                    return comment.ReplaceToken(lastToken, lastToken.WithText(valueText));
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
                var lastToken = t.TextTokens.Reverse().FirstOrDefault(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken) && _.ValueText.IsNullOrWhiteSpace() is false);

                if (lastToken.IsDefaultValue())
                {
                    // seems like we have a <see cref/> or something with a CRLF at the end
                }
                else
                {
                    // in case there is any, get rid of last dot
                    text = lastToken.ValueText.AsSpan().TrimEnd().TrimEnd('.').ConcatenatedWith(commentStart);
                }

                return comment.ReplaceNode(t, XmlText(text))
                              .AddContent(seeCref, XmlText(commentContinue).WithTrailingXmlComment());
            }

            // we have a <see cref/> or something at the end
            return comment.InsertNodeAfter(lastNode, XmlText(commentContinue));
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string[] phrases, FirstWordHandling firstWordHandling = FirstWordHandling.MakeLowerCase)
        {
            return CommentStartingWith(comment, phrases[0], firstWordHandling);
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string phrase, FirstWordHandling firstWordHandling = FirstWordHandling.MakeLowerCase)
        {
            var content = CommentStartingWith(comment.Content, phrase, firstWordHandling);

            return SyntaxFactory.XmlElement(comment.StartTag, content, comment.EndTag);
        }

        protected static SyntaxList<XmlNodeSyntax> CommentStartingWith(SyntaxList<XmlNodeSyntax> content, string phrase, FirstWordHandling firstWordHandling = FirstWordHandling.MakeLowerCase)
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

            if (index < 0)
            {
                return comment;
            }

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
                    var newToken = newTokens[0];

                    text = XmlText(newTokens.Replace(newToken, newToken.WithLeadingTrivia()));
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

        protected static string GetParameterName(XmlEmptyElementSyntax syntax) => syntax.Attributes.OfType<XmlAttributeSyntax, XmlNameAttributeSyntax>()[0].Identifier.GetName();

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
                case XmlEmptyElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes);
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
                case XmlEmptyElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes, type);
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
                case XmlEmptyElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes, type);
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
                case XmlEmptyElementSyntax element when element.GetName() == Constants.XmlTag.See:
                {
                    return IsCref(element.Attributes, type, member);
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

        protected static XmlTextSyntax MakeFirstWordInfiniteVerb(XmlTextSyntax text)
        {
            var textTokens = text.TextTokens;

            for (var index = 0; index < textTokens.Count; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var valueText = token.ValueText;

                var fixedText = MakeFirstWordInfiniteVerb(valueText);

                if (fixedText.Length != valueText.Length)
                {
                    return text.ReplaceToken(token, token.WithText(fixedText));
                }
            }

            return text;
        }

        protected static string MakeFirstWordInfiniteVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordInfiniteVerb(text.AsSpan());
        }

        protected static string MakeFirstWordInfiniteVerb(ReadOnlySpan<char> text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var valueText = text.TrimStart(Constants.SentenceMarkers);

            if (valueText.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // first word
            var firstWord = valueText.FirstWord().ToString();
            var infiniteVerb = Verbalizer.MakeInfiniteVerb(firstWord);

            if (firstWord != infiniteVerb)
            {
                return infiniteVerb.ConcatenatedWith(valueText.WithoutFirstWord());
            }

            return text.ToString();
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

        protected static XmlElementSyntax RemoveBooleansTags(XmlElementSyntax comment)
        {
            var withoutBooleans = comment.Without(comment.Content.Where(_ => _.IsBooleanTag()));
            var combinedTexts = CombineTexts(withoutBooleans);

            return CombineTexts(combinedTexts);
        }

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
            var token = text.AsToken();
            var attribute = SyntaxFactory.XmlTextAttribute(Constants.XmlTag.Attribute.Langword, token);

            return SyntaxFactory.XmlEmptyElement(Constants.XmlTag.See).WithAttribute(attribute);
        }

        protected static XmlEmptyElementSyntax SeeLangword_False() => SeeLangword("false");

        protected static XmlEmptyElementSyntax SeeLangword_Null() => SeeLangword("null");

        protected static XmlEmptyElementSyntax SeeLangword_True() => SeeLangword("true");

        protected static XmlElementSyntax SplitCommentAfterFirstSentence(XmlElementSyntax comment, out SyntaxList<XmlNodeSyntax> partsAfterSentence)
        {
            var partsForFirstSentence = new List<XmlNodeSyntax>();
            var partsForOtherSentences = new List<XmlNodeSyntax>();

            var commentContents = comment.Content;
            var commentContentsCount = commentContents.Count;

            for (var index = 0; index < commentContentsCount; index++)
            {
                var node = commentContents[index];

                if (node is XmlTextSyntax text)
                {
                    var textTokens = text.TextTokens;
                    var textTokensCount = textTokens.Count;

                    for (var tokenIndex = 0; tokenIndex < textTokensCount; tokenIndex++)
                    {
                        var token = textTokens[tokenIndex];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            continue;
                        }

                        var dotPosition = token.Text.IndexOf('.');

                        if (dotPosition < 0)
                        {
                            continue;
                        }

                        // we found the dot
                        var tokensBeforeDot = textTokens.Take(tokenIndex).ToList();
                        var tokensAfterDot = textTokens.Skip(tokenIndex + 1).ToList();

                        // split text into 2 parts
                        var valueText = token.ValueText;
                        var dotPos = valueText.IndexOf('.') + 1;

                        var firstText = valueText.Substring(0, dotPos);
                        var lastText = valueText.AsSpan(dotPos).TrimStart().ToString();

                        tokensBeforeDot.Add(token.WithText(firstText));

                        if (lastText.Length > 0)
                        {
                            tokensAfterDot.Add(token.WithText(lastText));
                        }

                        if (tokensBeforeDot.Any())
                        {
                            partsForFirstSentence.Add(XmlText(tokensBeforeDot).WithLeadingTriviaFrom(text));
                        }

                        if (tokensAfterDot.Any())
                        {
                            partsForOtherSentences.Add(XmlText(tokensAfterDot));
                        }

                        partsForOtherSentences.AddRange(commentContents.Skip(index + 1));

                        partsAfterSentence = partsForOtherSentences.ToSyntaxList();

                        return comment.WithContent(partsForFirstSentence.ToSyntaxList());
                    }
                }

                partsForFirstSentence.Add(node);
            }

            partsAfterSentence = SyntaxFactory.List<XmlNodeSyntax>();

            return comment;
        }

        protected static XmlEmptyElementSyntax TypeParamRef(string name)
        {
            var token = name.AsToken();
            var attribute = SyntaxFactory.XmlTextAttribute(Constants.XmlTag.Attribute.Name, token);

            return SyntaxFactory.XmlEmptyElement(Constants.XmlTag.TypeParamRef).WithAttribute(attribute);
        }

        protected static XmlElementSyntax XmlElement(string tag) => SyntaxFactory.XmlElement(tag, default);

        protected static XmlElementSyntax XmlElement(string tag, XmlNodeSyntax content) => SyntaxFactory.XmlElement(tag, new SyntaxList<XmlNodeSyntax>(content));

        protected static XmlElementSyntax XmlElement(string tag, IEnumerable<XmlNodeSyntax> contents) => SyntaxFactory.XmlElement(tag, contents.ToSyntaxList());

        protected static XmlTextSyntax NewLineXmlText() => XmlText(string.Empty).WithLeadingXmlComment();

        protected static XmlTextSyntax TrailingNewLineXmlText() => XmlText(string.Empty).WithTrailingXmlComment();

        protected static XmlTextSyntax XmlText(ReadOnlySpan<char> text) => XmlText(text.ToString());

        protected static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        protected static XmlTextSyntax XmlText(SyntaxTokenList textTokens)
        {
            if (textTokens.Count == 0)
            {
                return SyntaxFactory.XmlText();
            }

            return SyntaxFactory.XmlText(textTokens);
        }

        protected static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(textTokens.ToTokenList());

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

                var replacementText = commentEnd + textToken.ValueText.AsSpan().TrimStart().ToLowerCaseAt(0);
                var replacement = replacementText.AsToken();
                textTokens.Insert(0, replacement);

                textCommentEnd = XmlText(new SyntaxTokenList(textTokens).WithoutLastXmlNewLine());
            }
            else
            {
                textCommentEnd = XmlText(commentEnd);
            }

            var length = commendEndNodes.Length;

            // if there are more than 1 item contained, also remove the new line and /// from the last item
            if (length > 1)
            {
                if (commendEndNodes[length - 1] is XmlTextSyntax additionalText)
                {
                    commendEndNodes[length - 1] = XmlText(additionalText.TextTokens.WithoutLastXmlNewLine());
                }
            }

            var result = new List<XmlNodeSyntax>(1 + length - skip);
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

            if (content.Count > 1 && content[0].IsWhiteSpaceOnlyText())
            {
                return 1;
            }

            return 0;
        }

        private static bool IsSameGeneric(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is GenericNameSyntax g1 && t2 is GenericNameSyntax g2)
            {
                if (g1.Identifier.ValueText == g2.Identifier.ValueText)
                {
                    var arguments1 = g1.TypeArgumentList.Arguments;
                    var arguments2 = g2.TypeArgumentList.Arguments;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var arguments1Count = arguments1.Count;
                    var arguments2Count = arguments2.Count;

                    if (arguments1Count == arguments2Count)
                    {
                        for (var i = 0; i < arguments1Count; i++)
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

//// ncrunch: rdi off
        private static XmlElementSyntax CombineTexts(XmlElementSyntax comment)
        {
            var modified = false;
            var contents = comment.Content;

            for (var index = 0; index <= contents.Count - 2; index++)
            {
                var nextIndex = index + 1;

                var content1 = contents[index];
                var content2 = contents[nextIndex];

                if (content1 is XmlTextSyntax text1 && content2 is XmlTextSyntax text2)
                {
                    var text1TextTokens = text1.TextTokens;
                    var text2TextTokens = text2.TextTokens;

                    var lastToken = text1TextTokens.Last();
                    var firstToken = text2TextTokens.First();

                    var token = lastToken.WithText(lastToken.Text + firstToken.Text)
                                         .WithLeadingTriviaFrom(lastToken)
                                         .WithTrailingTriviaFrom(firstToken);

                    var tokens = text1TextTokens.Replace(lastToken, token).AddRange(text2TextTokens.Skip(1));
                    var newText = text1.WithTextTokens(tokens);

                    contents = contents.Replace(text1, newText).RemoveAt(nextIndex);

                    modified = true;
                }
            }

            if (modified)
            {
                return comment.WithContent(contents);
            }

            return comment;
        }
//// ncrunch: rdi default
    }
}