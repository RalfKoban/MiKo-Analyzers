using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetStartingPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.StartingPhrase, out var s) ? s : string.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetEndingPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.EndingPhrase, out var s) ? s : string.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.Phrase, out var s) ? s : string.Empty;

//// ncrunch: rdi off
//// ncrunch: no coverage start

        protected static string[] GetTermsForQuickLookup(IReadOnlyCollection<string> terms)
        {
            var pool = ArrayPool<string>.Shared;

            var rentedArray = pool.Rent(terms.Count);

            var resultIndex = 0;

            // ReSharper disable once TooWideLocalVariableScope : it's done to have less memory pressure on garbage collector
            bool found;

            foreach (var term in terms.OrderBy(_ => _.Length))
            {
                var span = term.AsSpan();

                found = false;

                for (var index = 0; index < resultIndex; index++)
                {
                    if (span.StartsWith(rentedArray[index].AsSpan()))
                    {
                        found = true;

                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                rentedArray[resultIndex] = term;
                resultIndex++;
            }

            var result = new string[resultIndex];

            Array.Copy(rentedArray, result, resultIndex);

            pool.Return(rentedArray);

            return result;
        }

//// ncrunch: no coverage end
//// ncrunch: rdi default

        protected static XmlElementSyntax C(string text) => SyntaxFactory.XmlElement(Constants.XmlTag.C, XmlText(text).ToSyntaxList<XmlNodeSyntax>());

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, in SyntaxList<XmlNodeSyntax> content)
        {
            var result = comment.WithStartTag(comment.StartTag.WithoutLeadingTrivia().WithTrailingXmlComment())
                                .WithContent(content)
                                .WithEndTag(comment.EndTag.WithoutTrailingTrivia().WithLeadingXmlComment());

            return CombineTexts(result);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, IEnumerable<XmlNodeSyntax> nodes) => Comment(comment, nodes.ToSyntaxList());

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, in ReadOnlySpan<string> text, string additionalComment = null) => Comment(comment, text[0], additionalComment);

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, in ReadOnlySpan<string> text, in SyntaxList<XmlNodeSyntax> additionalComment) => Comment(comment, text[0], additionalComment);

//// ncrunch: rdi off

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, in SyntaxList<XmlNodeSyntax> additionalComment)
        {
            var end = CommentEnd(text, additionalComment.ToArray());

            return Comment(comment, end);
        }

        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, string additionalComment = null) => Comment(comment, additionalComment is null ? XmlText(text) : XmlText(text + additionalComment));

        protected static XmlElementSyntax Comment(XmlElementSyntax syntax, in ReadOnlySpan<string> terms, in ReadOnlySpan<Pair> replacementMap, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.KeepSingleLeadingSpace)
        {
            var result = Comment<XmlElementSyntax>(syntax, terms, replacementMap, firstWordAdjustment);

            return CombineTexts(result);
        }

        protected static T Comment<T>(T syntax, in ReadOnlySpan<string> terms, in ReadOnlySpan<Pair> replacementMap, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.KeepSingleLeadingSpace) where T : SyntaxNode
        {
            var minimumLength = MinimumLength(terms);

            var textMap = CreateReplacementTextMap(minimumLength, terms, replacementMap, firstWordAdjustment);

            if (textMap is null)
            {
                // nothing found, so nothing to replace
                return syntax;
            }

            return syntax.ReplaceNodes(textMap.Keys, (_, __) => textMap[_]);

//// ncrunch: no coverage start
            int MinimumLength(in ReadOnlySpan<string> source)
            {
                var sourceLength = source.Length;

                if (sourceLength <= 0)
                {
                    return 0;
                }

                var minimum = int.MaxValue;

                for (var index = 0; index < sourceLength; index++)
                {
                    var length = source[index].Length;

                    if (length < minimum)
                    {
                        minimum = length;
                    }
                }

                return minimum;
            }

            Dictionary<XmlTextSyntax, XmlTextSyntax> CreateReplacementTextMap(in int minLength, in ReadOnlySpan<string> phrases, in ReadOnlySpan<Pair> map, in FirstWordAdjustment adjustment)
            {
                Dictionary<XmlTextSyntax, XmlTextSyntax> result = null;

                foreach (var text in syntax.DescendantNodesAndSelf().OfType<XmlTextSyntax>())
                {
                    Dictionary<SyntaxToken, SyntaxToken> tokenMap = null;

                    // replace token in text
                    var textTokens = text.TextTokens;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
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

                        if (originalText.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase))
                        {
                            var replacedText = originalText.AsCachedBuilder()
                                                           .ReplaceAllWithProbe(map)
                                                           .AdjustFirstWord(adjustment)
                                                           .ToStringAndRelease();

                            if (originalText.AsSpan().SequenceEqual(replacedText.AsSpan()))
                            {
                                // replacement with itself does not make any sense
                                continue;
                            }

                            var newToken = token.WithText(replacedText);

                            if (tokenMap is null)
                            {
                                tokenMap = new Dictionary<SyntaxToken, SyntaxToken>(1);
                            }

                            tokenMap.Add(token, newToken);
                        }
                    }

                    if (tokenMap is null)
                    {
                        // nothing found, so nothing to replace
                        continue;
                    }

                    if (result is null)
                    {
                        result = new Dictionary<XmlTextSyntax, XmlTextSyntax>(1);
                    }

                    var newText = text.ReplaceTokens(tokenMap.Keys, (_, __) => tokenMap[_]);

                    result.Add(text, newText);
                }

                return result;
            }
        }

//// ncrunch: no coverage end
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
                                              in SyntaxList<XmlNodeSyntax> commendEndNodes)
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

            switch (lastNode)
            {
                case null:
                {
                    // we have an empty comment
                    return comment.AddContent(XmlText(ending));
                }

                case XmlTextSyntax t:
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

                default:
                {
                    // we have a <see cref/> or something at the end
                    return comment.InsertNodeAfter(lastNode, XmlText(ending));
                }
            }
        }

        protected static XmlElementSyntax CommentEndingWith(XmlElementSyntax comment, string commentStart, XmlEmptyElementSyntax seeCref, string commentContinue)
        {
            var lastNode = comment.Content.LastOrDefault();

            switch (lastNode)
            {
                case null:
                {
                    // we have an empty comment
                    return comment.AddContent(XmlText(commentStart), seeCref, XmlText(commentContinue));
                }

                case XmlTextSyntax t:
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
                                  .AddContent(seeCref, XmlText(commentContinue))
                                  .WithTagsOnSeparateLines();
                }

                default:
                {
                    // we have a <see cref/> or something at the end
                    return comment.InsertNodeAfter(lastNode, XmlText(commentContinue));
                }
            }
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, in ReadOnlySpan<string> phrases, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
        {
            return CommentStartingWith(comment, phrases[0], firstWordAdjustment);
        }

        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string phrase, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
        {
            var content = CommentStartingWith(comment.Content, phrase, firstWordAdjustment);

            return CommentWithContent(comment, content);
        }

        protected static SyntaxList<XmlNodeSyntax> CommentStartingWith(SyntaxList<XmlNodeSyntax> content, string phrase, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
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

                var newText = text.WithStartText(phrase, firstWordAdjustment);

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

            var startText = XmlText(commentStart);

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
                if (index is 1 && content[0].IsWhiteSpaceOnlyText())
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

            return CommentWithContent(comment, newContent);
        }

        protected static XmlElementSyntax CommentWithContent(XmlElementSyntax value, in SyntaxList<XmlNodeSyntax> content) => SyntaxFactory.XmlElement(value.StartTag, content, value.EndTag).WithTagsOnSeparateLines();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static TypeCrefSyntax Cref(string typeName) => Cref(typeName.AsTypeSyntax());

        protected static TypeCrefSyntax Cref(TypeSyntax type)
        {
            // fix trivia, to avoid situation as reported in https://github.com/dotnet/roslyn/issues/47550
            return SyntaxFactory.TypeCref(type.WithoutTrivia());
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

        protected static string GetParameterName(XmlElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>()[0].Identifier.GetName();

        protected static string GetParameterName(XmlEmptyElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>()[0].Identifier.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlCrefAttributeSyntax GetSeeCref(SyntaxNode value) => value.GetCref(Constants.XmlTag.See);

        protected static DocumentationCommentTriviaSyntax GetXmlSyntax(IEnumerable<SyntaxNode> syntaxNodes, in SyntaxKind kind = SyntaxKind.SingleLineDocumentationCommentTrivia)
        {
            foreach (var node in syntaxNodes)
            {
                var comments = node.GetDocumentationCommentTriviaSyntax(kind);

                if (comments.Length > 0)
                {
                    return comments[0];
                }
            }

            return null;
        }

        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(string startTag, IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.SelectMany(_ => _.GetXmlSyntax(startTag));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax Inheritdoc() => XmlEmptyElement(Constants.XmlTag.Inheritdoc);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax Inheritdoc(XmlCrefAttributeSyntax cref) => Inheritdoc().WithAttributes(cref.ToSyntaxList<XmlAttributeSyntax>());

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

            for (int index = 0, textTokensCount = textTokens.Count; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var valueText = token.ValueText;

                var fixedText = Verbalizer.MakeFirstWordInfiniteVerb(valueText);

                if (fixedText.Length != valueText.Length)
                {
                    return text.ReplaceToken(token, token.WithText(fixedText));
                }
            }

            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax Para() => XmlEmptyElement(Constants.XmlTag.Para);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax Para(string text) => SyntaxFactory.XmlParaElement(XmlText(text));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax Para(in SyntaxList<XmlNodeSyntax> nodes) => SyntaxFactory.XmlParaElement(nodes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, in ReadOnlySpan<string> comments) => ParameterComment(parameter, comments[0]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string comment) => Comment(SyntaxFactory.XmlParamElement(parameter.GetName()), comment);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax ParamRef(ParameterSyntax parameter) => ParamRef(parameter.GetName());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax ParamRef(string parameterName) => XmlEmptyElement(Constants.XmlTag.ParamRef).WithAttribute(SyntaxFactory.XmlNameAttribute(parameterName));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax ParaOr() => Para(Constants.Comments.SpecialOrPhrase);

        protected static XmlElementSyntax RemoveBooleansTags(XmlElementSyntax comment)
        {
            var withoutBooleans = comment.Without(comment.Content.Where(_ => _.IsBooleanTag()));
            var combinedTexts = CombineTexts(withoutBooleans);

            return CombineTexts(combinedTexts);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static T ReplaceText<T>(T comment, XmlTextSyntax text, string phrase, string replacement) where T : SyntaxNode => ReplaceText(comment, text, new[] { phrase }, replacement);

        protected static T ReplaceText<T>(T comment, XmlTextSyntax text, in ReadOnlySpan<string> phrases, string replacement) where T : SyntaxNode
        {
            var modifiedText = text.ReplaceText(phrases, replacement);

            return ReferenceEquals(text, modifiedText)
                   ? comment
                   : comment.ReplaceNode(text, modifiedText);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(string typeName) => SeeCref(typeName.AsTypeSyntax());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type) => Cref(Constants.XmlTag.See, type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(string typeName, NameSyntax member) => SeeCref(typeName.AsTypeSyntax(), member);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, NameSyntax member) => Cref(Constants.XmlTag.See, type, member);

        protected static XmlEmptyElementSyntax SeeLangword(string text)
        {
            var attribute = XmlAttribute(Constants.XmlTag.Attribute.Langword, text);

            return XmlEmptyElement(Constants.XmlTag.See).WithAttribute(attribute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeLangword_False() => SeeLangword("false");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeLangword_Null() => SeeLangword("null");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeLangword_True() => SeeLangword("true");

#pragma warning disable CA1021
        protected static XmlElementSyntax SplitCommentAfterFirstSentence(XmlElementSyntax comment, out SyntaxList<XmlNodeSyntax> partsAfterSentence)
#pragma warning restore CA1021
        {
            var partsForFirstSentence = new List<XmlNodeSyntax>();
            var partsForOtherSentences = new List<XmlNodeSyntax>();

            var commentContents = comment.Content;

            for (int index = 0, commentContentsCount = commentContents.Count; index < commentContentsCount; index++)
            {
                var node = commentContents[index];

                if (node is XmlTextSyntax text)
                {
                    var textTokens = text.TextTokens;

                    for (int tokenIndex = 0, textTokensCount = textTokens.Count; tokenIndex < textTokensCount; tokenIndex++)
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

                        var firstText = valueText.AsSpan(0, dotPos);
                        var lastText = valueText.AsSpan(dotPos).TrimStart();

                        tokensBeforeDot.Add(token.WithText(firstText));

                        if (lastText.Length > 0)
                        {
                            tokensAfterDot.Add(token.WithText(lastText));
                        }

                        if (tokensBeforeDot.Count > 0)
                        {
                            partsForFirstSentence.Add(XmlText(tokensBeforeDot).WithLeadingTriviaFrom(text));
                        }

                        if (tokensAfterDot.Count > 0)
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
            var attribute = XmlAttribute(Constants.XmlTag.Attribute.Name, name);

            return XmlEmptyElement(Constants.XmlTag.TypeParamRef).WithAttribute(attribute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextAttributeSyntax XmlAttribute(string tag, string text) => SyntaxFactory.XmlTextAttribute(tag, text.AsToken());

        /// <summary>
        /// Creates an XML list of given list type.
        /// </summary>
        /// <remarks>
        /// Adds leading XML comments (<c>/// </c>) to each item and a trailing XML comment (<c>/// </c>) to the last one.
        /// </remarks>
        /// <param name="listType">
        /// The type of the list.
        /// </param>
        /// <param name="items">
        /// The items of the list.
        /// </param>
        /// <returns>
        /// The created XML list.
        /// </returns>
        protected static XmlElementSyntax XmlList(string listType, IList<XmlElementSyntax> items)
        {
            var itemsCount = items.Count;

            if (itemsCount > 0)
            {
                for (var index = 0; index < itemsCount; index++)
                {
                    items[index] = items[index].WithLeadingXmlComment();
                }

                items[itemsCount - 1] = items[itemsCount - 1].WithTrailingXmlComment();
            }

            var list = XmlElement(Constants.XmlTag.List, items);
            var type = SyntaxFactory.XmlTextAttribute(Constants.XmlTag.Attribute.Type, listType.AsToken());

            return list.AddStartTagAttributes(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax XmlElement(string tag) => SyntaxFactory.XmlElement(tag, default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax XmlElement(string tag, XmlNodeSyntax content) => SyntaxFactory.XmlElement(tag, content.ToSyntaxList());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax XmlElement(string tag, IEnumerable<XmlNodeSyntax> contents) => SyntaxFactory.XmlElement(tag, contents.ToSyntaxList());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax XmlEmptyElement(string tag) => SyntaxFactory.XmlEmptyElement(tag);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax NewLineXmlText() => XmlText(string.Empty).WithLeadingXmlComment();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax TrailingNewLineXmlText() => XmlText(string.Empty).WithTrailingXmlComment();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        protected static XmlTextSyntax XmlText(in SyntaxTokenList textTokens)
        {
            if (textTokens.Count is 0)
            {
                return SyntaxFactory.XmlText();
            }

            return SyntaxFactory.XmlText(textTokens);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(textTokens.ToTokenList());

        private static List<XmlNodeSyntax> CommentEnd(string commentEnd, params XmlNodeSyntax[] commendEndNodes)
        {
            var skip = 0;
            XmlTextSyntax textCommentEnd;

            var length = commendEndNodes.Length;

            // add a white space at the end of the comment in case we have further texts
            if (length > 1 && commentEnd.Length > 0 && commentEnd[commentEnd.Length - 1] != ' ')
            {
                commentEnd += " ";
            }

            if (commendEndNodes.FirstOrDefault() is XmlTextSyntax text)
            {
                skip = 1;

                var textTokens = text.TextTokens.ToList();
                var textToken = textTokens.First(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken));
                var removals = textTokens.IndexOf(textToken);

                textTokens.RemoveRange(0, removals + 1);

                var tokenText = textToken.ValueText;

                var continuationText = tokenText is Constants.TODO ? tokenText : tokenText.AsSpan().TrimStart().ToLowerCaseAt(0);

                var replacementText = commentEnd + continuationText;
                var replacement = replacementText.AsToken();
                textTokens.Insert(0, replacement);

                textCommentEnd = XmlText(textTokens.ToTokenList().WithoutLastXmlNewLine());
            }
            else
            {
                textCommentEnd = XmlText(commentEnd);
            }

            // if there are more than 1 item contained, also remove the new line and /// from the last item
            if (length > 1)
            {
                var last = length - 1;

                if (commendEndNodes[last] is XmlTextSyntax additionalText)
                {
                    commendEndNodes[last] = XmlText(additionalText.TextTokens.WithoutLastXmlNewLine());
                }
            }

            var result = new List<XmlNodeSyntax>(1 + length - skip);
            result.Add(textCommentEnd);
            result.AddRange(commendEndNodes.Skip(skip));

            return result;
        }

        private static XmlEmptyElementSyntax Cref(string tag, CrefSyntax syntax) => XmlEmptyElement(tag).WithAttribute(SyntaxFactory.XmlCrefAttribute(syntax));

        private static int GetIndex(in SyntaxList<XmlNodeSyntax> content)
        {
            var contentCount = content.Count;

            if (contentCount is 0)
            {
                return -1;
            }

            if (contentCount > 1 && content[0].IsWhiteSpaceOnlyText())
            {
                return 1;
            }

            return 0;
        }

//// ncrunch: rdi off
        private static XmlElementSyntax CombineTexts(XmlElementSyntax comment)
        {
            var contents = comment.Content;
            var contentsCount = contents.Count - 2; // risky operation, fails when 'contents' gets re-assigned so perform a careful review of the code

            if (contentsCount >= 0)
            {
                var modified = false;

                for (var index = 0; index <= contentsCount; index++)
                {
                    if (contents[index + 1] is XmlTextSyntax nextText && contents[index] is XmlTextSyntax currentText)
                    {
                        var currentTextTokens = currentText.TextTokens;
                        var nextTextTokens = nextText.TextTokens;

                        var lastToken = currentTextTokens.Last();
                        var firstToken = nextTextTokens.First();

                        var token = lastToken.WithText(lastToken.Text + firstToken.Text)
                                             .WithLeadingTriviaFrom(lastToken)
                                             .WithTrailingTriviaFrom(firstToken);

                        var tokens = currentTextTokens.Replace(lastToken, token).AddRange(nextTextTokens.Skip(1));
                        var newText = currentText.WithTextTokens(tokens);

                        contents = contents.Replace(currentText, newText).RemoveAt(index + 1);
                        contentsCount = contents.Count - 2; // risky operation, fails when 'contents' gets re-assigned

                        modified = true;
                    }
                }

                if (modified)
                {
                    return comment.WithContent(contents);
                }
            }

            return comment;
        }
//// ncrunch: rdi default
    }
}