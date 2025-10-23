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
        /// <summary>
        /// Gets the starting phrase proposal from the specified diagnostic issue.
        /// </summary>
        /// <param name="issue">
        /// The diagnostic issue containing the starting phrase proposal.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the starting phrase proposal, or an empty string if not found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetStartingPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.StartingPhrase, out var s) ? s : string.Empty;

        /// <summary>
        /// Gets the ending phrase proposal from the specified diagnostic issue.
        /// </summary>
        /// <param name="issue">
        /// The diagnostic issue containing the ending phrase proposal.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the ending phrase proposal, or an empty string if not found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetEndingPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.EndingPhrase, out var s) ? s : string.Empty;

        /// <summary>
        /// Gets the phrase proposal from the specified diagnostic issue.
        /// </summary>
        /// <param name="issue">
        /// The diagnostic issue containing the phrase proposal.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the phrase proposal, or an empty string if not found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetPhraseProposal(Diagnostic issue) => issue.Properties.TryGetValue(Constants.AnalyzerCodeFixSharedData.Phrase, out var s) ? s : string.Empty;

//// ncrunch: rdi off
//// ncrunch: no coverage start

        /// <summary>
        /// Gets an optimized array of terms for quick lookup by removing terms that are prefixes of other terms.
        /// </summary>
        /// <param name="terms">
        /// The collection of terms to optimize.
        /// </param>
        /// <returns>
        /// An array of terms where no term is a prefix of another term, ordered by specificity.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This optimization reduces the number of comparisons needed during text replacement operations by ensuring that longer, more specific terms are checked without being shadowed by their prefixes.
        /// For example, if input contains ["get", "getter"], only "getter" is returned since "get" is a prefix.
        /// </para>
        /// <para>
        /// Uses <see cref="ArrayPool{T}"/> for efficient memory allocation.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Gets an XML code element containing the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to include in the code element.
        /// </param>
        /// <returns>
        /// The XML code element containing the specified text.
        /// </returns>
        protected static XmlElementSyntax C(string text) => SyntaxFactory.XmlElement(Constants.XmlTag.C, XmlText(text).ToSyntaxList<XmlNodeSyntax>());

        /// <summary>
        /// Gets an XML comment element with the specified content and proper XML comment formatting.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="content">
        /// The content nodes for the comment.
        /// </param>
        /// <returns>
        /// The XML comment element with properly formatted tags and combined text nodes.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, in SyntaxList<XmlNodeSyntax> content)
        {
            var result = comment.WithStartTag(comment.StartTag.WithoutLeadingTrivia().WithTrailingXmlComment())
                                .WithContent(content)
                                .WithEndTag(comment.EndTag.WithoutTrailingTrivia().WithLeadingXmlComment());

            return CombineTexts(result);
        }

        /// <summary>
        /// Gets an XML comment element with the specified nodes and proper XML comment formatting.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="nodes">
        /// The XML nodes for the comment.
        /// </param>
        /// <returns>
        /// The XML comment element with properly formatted tags and combined text nodes.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, IEnumerable<XmlNodeSyntax> nodes) => Comment(comment, nodes.ToSyntaxList());

        /// <summary>
        /// Gets an XML comment element with text from the specified span.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="text">
        /// The text span containing the comment text.
        /// </param>
        /// <param name="additionalComment">
        /// The optional additional comment text to append.
        /// The default is <see langword="null"/>.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, in ReadOnlySpan<string> text, string additionalComment = null) => Comment(comment, text[0], additionalComment);

        /// <summary>
        /// Gets an XML comment element with text from the specified span and additional content nodes.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="text">
        /// The text span containing the comment text.
        /// </param>
        /// <param name="additionalComment">
        /// The additional content nodes to append.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text and additional content.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, in ReadOnlySpan<string> text, in SyntaxList<XmlNodeSyntax> additionalComment) => Comment(comment, text[0], additionalComment);

        //// ncrunch: rdi off

        /// <summary>
        /// Gets an XML comment element with the specified text and additional content nodes.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="text">
        /// The comment text.
        /// </param>
        /// <param name="additionalComment">
        /// The additional content nodes to append.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text and additional content.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, in SyntaxList<XmlNodeSyntax> additionalComment)
        {
            var end = CommentEnd(text, additionalComment.ToArray());

            return Comment(comment, end);
        }

        /// <summary>
        /// Gets an XML comment element with the specified text.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="text">
        /// The comment text.
        /// </param>
        /// <param name="additionalComment">
        /// The optional additional comment text to append.
        /// The default is <see langword="null"/>.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string text, string additionalComment = null) => Comment(comment, additionalComment is null ? XmlText(text) : XmlText(text + additionalComment));

        /// <summary>
        /// Gets an XML comment element with text replacements based on the specified lookup terms and replacement map.
        /// </summary>
        /// <param name="syntax">
        /// The XML element to update.
        /// </param>
        /// <param name="lookupTerms">
        /// The terms to search for in the comment text.
        /// </param>
        /// <param name="replacementMap">
        /// The map of terms to their replacements.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word.
        /// The default is <see cref="FirstWordAdjustment.KeepSingleLeadingSpace"/>.
        /// </param>
        /// <returns>
        /// The XML comment element with replaced text and combined text nodes.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax syntax, in ReadOnlySpan<string> lookupTerms, in ReadOnlySpan<Pair> replacementMap, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.KeepSingleLeadingSpace)
        {
            var result = Comment<XmlElementSyntax>(syntax, lookupTerms, replacementMap, firstWordAdjustment);

            return CombineTexts(result);
        }

        /// <summary>
        /// Gets a syntax node with text replacements based on the specified lookup terms and replacement map.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node (typically <see cref="XmlElementSyntax"/>).
        /// </typeparam>
        /// <param name="syntax">
        /// The syntax node to update.
        /// </param>
        /// <param name="lookupTerms">
        /// The terms to search for in the text. Use <see cref="GetTermsForQuickLookup"/> to optimize this array.
        /// </param>
        /// <param name="replacementMap">
        /// The map of terms to their replacements (key = original, value = replacement).
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word (casing, verb form, etc.).
        /// The default is <see cref="FirstWordAdjustment.KeepSingleLeadingSpace"/>.
        /// </param>
        /// <returns>
        /// The syntax node with replaced text, or the original node if no replacements were made.
        /// </returns>
        /// <remarks>
        /// This is the core text replacement method that performs pattern matching and substitution across all XML text nodes within the syntax tree.
        /// It performs a deep search through all <see cref="XmlTextSyntax"/> descendants and replaces matching phrases while respecting the minimum length optimization for performance.
        /// </remarks>
        protected static T Comment<T>(T syntax, in ReadOnlySpan<string> lookupTerms, in ReadOnlySpan<Pair> replacementMap, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.KeepSingleLeadingSpace) where T : SyntaxNode
        {
            var minimumLength = MinimumLength(lookupTerms);

            var textMap = CreateReplacementTextMap(minimumLength, lookupTerms, replacementMap, firstWordAdjustment);

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

            Dictionary<XmlTextSyntax, XmlTextSyntax> CreateReplacementTextMap(in int minLength, in ReadOnlySpan<string> lookupPhrases, in ReadOnlySpan<Pair> map, in FirstWordAdjustment adjustment)
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

                        if (originalText.ContainsAny(lookupPhrases, StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Gets an XML comment element with the specified text surrounding a type reference.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the type reference.
        /// </param>
        /// <param name="type">
        /// The type to reference.
        /// </param>
        /// <param name="commentEnd">
        /// The text after the type reference.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text and type reference.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string commentStart, TypeSyntax type, string commentEnd)
        {
            return Comment(comment, commentStart, SeeCref(type), commentEnd);
        }

        /// <summary>
        /// Gets an XML comment element with the specified text surrounding a link and additional content nodes.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the link.
        /// </param>
        /// <param name="link">
        /// The link node.
        /// </param>
        /// <param name="commentEnd">
        /// The text after the link.
        /// </param>
        /// <param name="commentEndNodes">
        /// The additional content nodes to append.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text, link, and additional content.
        /// </returns>
        protected static XmlElementSyntax Comment(
                                              XmlElementSyntax comment,
                                              string commentStart,
                                              XmlNodeSyntax link,
                                              string commentEnd,
                                              in SyntaxList<XmlNodeSyntax> commentEndNodes)
        {
            return Comment(comment, commentStart, link, commentEnd, commentEndNodes.ToArray());
        }

        /// <summary>
        /// Gets an XML comment element with the specified XML nodes.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="nodes">
        /// The XML nodes for the comment.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified nodes.
        /// </returns>
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, params XmlNodeSyntax[] nodes) => Comment(comment, nodes.ToSyntaxList());

        /// <summary>
        /// Gets an XML comment element with the specified text surrounding a link and additional content nodes.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the link.
        /// </param>
        /// <param name="link">
        /// The link node.
        /// </param>
        /// <param name="commentEnd">
        /// The text after the link.
        /// </param>
        /// <param name="commentEndNodes">
        /// The additional content nodes to append.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text, link, and additional content.
        /// </returns>
        protected static XmlElementSyntax Comment(
                                              XmlElementSyntax comment,
                                              string commentStart,
                                              XmlNodeSyntax link,
                                              string commentEnd,
                                              params XmlNodeSyntax[] commentEndNodes)
        {
            // TODO RKN: Check array creation to see if it can be optimized
            var start = new[] { XmlText(commentStart), link };
            var end = CommentEnd(commentEnd, commentEndNodes);

            return Comment(comment, start.Concat(end));
        }

        /// <summary>
        /// Gets an XML comment element with the specified text surrounding two links.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the first link.
        /// </param>
        /// <param name="link1">
        /// The first link node.
        /// </param>
        /// <param name="commentMiddle">
        /// The text between the two links.
        /// </param>
        /// <param name="link2">
        /// The second link node.
        /// </param>
        /// <param name="commentEnd">
        /// The text after the second link.
        /// </param>
        /// <param name="commentEndNodes">
        /// The additional content nodes to append.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text, links, and additional content.
        /// </returns>
        protected static XmlElementSyntax Comment(
                                              XmlElementSyntax comment,
                                              string commentStart,
                                              XmlNodeSyntax link1,
                                              string commentMiddle,
                                              XmlNodeSyntax link2,
                                              string commentEnd,
                                              params XmlNodeSyntax[] commentEndNodes)
        {
            // TODO RKN: Check array creation to see if it can be optimized
            return Comment(comment, commentStart, link1, new[] { XmlText(commentMiddle) }, link2, commentEnd, commentEndNodes);
        }

        /// <summary>
        /// Gets an XML comment element with the specified text surrounding two links with middle content nodes.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the first link.
        /// </param>
        /// <param name="link1">
        /// The first link node.
        /// </param>
        /// <param name="commentMiddle">
        /// The content nodes between the two links.
        /// </param>
        /// <param name="link2">
        /// The second link node.
        /// </param>
        /// <param name="commentEnd">
        /// The text after the second link.
        /// </param>
        /// <param name="commentEndNodes">
        /// The additional content nodes to append.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified text, links, and content.
        /// </returns>
        protected static XmlElementSyntax Comment(
                                              XmlElementSyntax comment,
                                              string commentStart,
                                              XmlNodeSyntax link1,
                                              IEnumerable<XmlNodeSyntax> commentMiddle,
                                              XmlNodeSyntax link2,
                                              string commentEnd,
                                              params XmlNodeSyntax[] commentEndNodes)
        {
            // TODO RKN: Check array creation to see if it can be optimized
            var start = new[] { XmlText(commentStart), link1 };
            var middle = new[] { link2 };
            var end = CommentEnd(commentEnd, commentEndNodes);

            // TODO RKN: Fix XML escaping caused by string conversion
            return Comment(comment, start.Concat(commentMiddle).Concat(middle).Concat(end));
        }

        /// <summary>
        /// Gets an XML comment element with the specified ending text appended, replacing any existing trailing period with the new ending. This ensures consistent sentence termination.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="ending">
        /// The ending text to apply.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified ending, with any previous trailing period removed.
        /// </returns>
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

        /// <summary>
        /// Gets an XML comment element with the specified ending text and reference appended or replaced.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the reference.
        /// </param>
        /// <param name="seeCref">
        /// The reference to append.
        /// </param>
        /// <param name="commentContinue">
        /// The text after the reference.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified ending content.
        /// </returns>
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

        /// <summary>
        /// Gets an XML comment element with a given phrase added at the beginning.
        /// The first word of the original comment is adjusted based on the specified rule.
        /// For example, the word may be lowercased to continue the sentence smoothly.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="phrase">
        /// The phrase to add at the start.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the original first word.
        /// The default is <see cref="FirstWordAdjustment.StartLowerCase"/>.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified starting phrase and adjusted continuation.
        /// </returns>
        protected static XmlElementSyntax CommentStartingWith(XmlElementSyntax comment, string phrase, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
        {
            var content = CommentStartingWith(comment.Content, phrase, firstWordAdjustment);

            return CommentWithContent(comment, content);
        }

        /// <summary>
        /// Gets a content list with the specified phrase at the start and adjusted first word.
        /// </summary>
        /// <param name="content">
        /// The content nodes to update.
        /// </param>
        /// <param name="phrase">
        /// The phrase to add at the start.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word.
        /// The default is <see cref="FirstWordAdjustment.StartLowerCase"/>.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlNodeSyntax"/> that contains the specified starting phrase.
        /// </returns>
        protected static SyntaxList<XmlNodeSyntax> CommentStartingWith(in SyntaxList<XmlNodeSyntax> content, string phrase, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
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
                var updatedContent = content.Remove(text);

                if (phrase.IsNullOrWhiteSpace())
                {
                    text = text.WithoutTrailingXmlComment();
                }

                var newText = text.WithStartText(phrase, firstWordAdjustment);

                return updatedContent.Insert(index, newText);
            }

            return content.Insert(index, XmlText(phrase));
        }

        /// <summary>
        /// Gets an XML comment element with the specified starting text and reference.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <param name="commentStart">
        /// The text before the reference.
        /// </param>
        /// <param name="seeCref">
        /// The reference to add.
        /// </param>
        /// <param name="commentContinue">
        /// The text after the reference.
        /// </param>
        /// <returns>
        /// The XML comment element with the specified starting content.
        /// </returns>
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
                    var tokens = textTokens.ToList();
                    tokens.RemoveAt(0);

                    tokens[0] = tokens[0].WithLeadingTrivia();

                    text = XmlText(tokens);
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

            var newContent = content.InsertRange(index, new XmlNodeSyntax[] { startText, seeCref, continueText });

            return CommentWithContent(comment, newContent);
        }

        /// <summary>
        /// Gets an XML element with the specified content and properly formatted tags.
        /// </summary>
        /// <param name="value">
        /// The XML element to update.
        /// </param>
        /// <param name="content">
        /// The content nodes for the element.
        /// </param>
        /// <returns>
        /// The XML element with the specified content and tags on separate lines.
        /// </returns>
        protected static XmlElementSyntax CommentWithContent(XmlElementSyntax value, in SyntaxList<XmlNodeSyntax> content) => SyntaxFactory.XmlElement(value.StartTag, content, value.EndTag).WithTagsOnSeparateLines();

        /// <summary>
        /// Gets a type reference syntax for the specified type name.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type to reference.
        /// </param>
        /// <returns>
        /// The type reference syntax for the specified type name.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static TypeCrefSyntax Cref(string typeName) => Cref(typeName.AsTypeSyntax());

        /// <summary>
        /// Gets a type reference syntax for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to reference.
        /// </param>
        /// <returns>
        /// The type reference syntax without trivia.
        /// </returns>
        protected static TypeCrefSyntax Cref(TypeSyntax type)
        {
            // fix trivia, to avoid situation as reported in https://github.com/dotnet/roslyn/issues/47550
            return SyntaxFactory.TypeCref(type.WithoutTrivia());
        }

        /// <summary>
        /// Gets an empty XML element with a reference to the specified type.
        /// </summary>
        /// <param name="tag">
        /// The XML tag name.
        /// </param>
        /// <param name="type">
        /// The type to reference.
        /// </param>
        /// <returns>
        /// The empty XML element with the type reference.
        /// </returns>
        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type)
        {
            // fix trivia, to avoid situation as reported in https://github.com/dotnet/roslyn/issues/47550
            return Cref(tag, SyntaxFactory.TypeCref(type.WithoutTrivia()));
        }

        /// <summary>
        /// Gets an empty XML element with a reference to the specified type member.
        /// </summary>
        /// <param name="tag">
        /// The XML tag name.
        /// </param>
        /// <param name="type">
        /// The type containing the member.
        /// </param>
        /// <param name="member">
        /// The member to reference.
        /// </param>
        /// <returns>
        /// The empty XML element with the type member reference.
        /// </returns>
        protected static XmlEmptyElementSyntax Cref(string tag, TypeSyntax type, NameSyntax member)
        {
            // fix trivia, to avoid situation as reported in https://github.com/dotnet/roslyn/issues/47550
            return Cref(tag, SyntaxFactory.QualifiedCref(type.WithoutTrivia(), SyntaxFactory.NameMemberCref(member.WithoutTrivia())));
        }

        /// <summary>
        /// Gets the parameter name from the specified XML element.
        /// </summary>
        /// <param name="syntax">
        /// The XML element containing the parameter name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the parameter name.
        /// </returns>
        protected static string GetParameterName(XmlElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>()[0].Identifier.GetName();

        /// <summary>
        /// Gets the parameter name from the specified empty XML element.
        /// </summary>
        /// <param name="syntax">
        /// The empty XML element containing the parameter name.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the parameter name.
        /// </returns>
        protected static string GetParameterName(XmlEmptyElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>()[0].Identifier.GetName();

        /// <summary>
        /// Gets the <c>cref</c> attribute from the specified syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node containing the <c>cref</c> attribute.
        /// </param>
        /// <returns>
        /// The <c>cref</c> attribute.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlCrefAttributeSyntax GetSeeCref(SyntaxNode value) => value.GetCref(Constants.XmlTag.See);

        /// <summary>
        /// Gets the first documentation comment trivia from the specified syntax nodes.
        /// </summary>
        /// <param name="syntaxNodes">
        /// The syntax nodes to search.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of documentation comment to seek.
        /// The default is <see cref="SyntaxKind.SingleLineDocumentationCommentTrivia"/>.
        /// </param>
        /// <returns>
        /// The documentation comment trivia, or <see langword="null"/> if not found.
        /// </returns>
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

        /// <summary>
        /// Gets an <c>&lt;inheritdoc/&gt;</c> XML element.
        /// </summary>
        /// <returns>
        /// The <c>&lt;inheritdoc/&gt;</c> XML element.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax Inheritdoc() => XmlEmptyElement(Constants.XmlTag.Inheritdoc);

        /// <summary>
        /// Gets an <c>&lt;inheritdoc cref="…"/&gt;</c> XML element with the specified <c>cref</c> attribute.
        /// </summary>
        /// <param name="cref">
        /// The <c>cref</c> attribute for the inheritdoc element.
        /// </param>
        /// <returns>
        /// The <c>&lt;inheritdoc cref="…"/&gt;</c> XML element  with the <c>cref</c> attribute.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax Inheritdoc(XmlCrefAttributeSyntax cref) => Inheritdoc().WithAttributes(cref.ToSyntaxList<XmlAttributeSyntax>());

        /// <summary>
        /// Gets an XML element with the first word converted to infinite verb form.
        /// </summary>
        /// <param name="syntax">
        /// The XML element to update.
        /// </param>
        /// <returns>
        /// The XML element with the first word converted to infinite verb form.
        /// The original element if the first word is not a verb or is already in infinite form.
        /// </returns>
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

        /// <summary>
        /// Gets an XML text with the first word converted to infinite verb form.
        /// </summary>
        /// <param name="text">
        /// The XML text to update.
        /// </param>
        /// <returns>
        /// The XML text with the first word converted to infinite verb form.
        /// </returns>
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

        /// <summary>
        /// Gets an XML para empty element.
        /// </summary>
        /// <returns>
        /// The XML para empty element.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax Para() => XmlEmptyElement(Constants.XmlTag.Para);

        /// <summary>
        /// Gets an XML para element with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text for the para element.
        /// </param>
        /// <returns>
        /// The XML para element with the specified text.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax Para(string text) => SyntaxFactory.XmlParaElement(XmlText(text));

        /// <summary>
        /// Gets an XML para element with the specified content nodes.
        /// </summary>
        /// <param name="nodes">
        /// The content nodes for the para element.
        /// </param>
        /// <returns>
        /// The XML para element with the specified content.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax Para(in SyntaxList<XmlNodeSyntax> nodes) => SyntaxFactory.XmlParaElement(nodes);

        /// <summary>
        /// Gets an XML parameter comment for the specified parameter using text from the span.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to document.
        /// </param>
        /// <param name="comments">
        /// The text span containing the comment text.
        /// </param>
        /// <returns>
        /// The XML parameter comment.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, in ReadOnlySpan<string> comments) => ParameterComment(parameter, comments[0]);

        /// <summary>
        /// Gets an XML parameter comment for the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to document.
        /// </param>
        /// <param name="comment">
        /// The comment text.
        /// </param>
        /// <returns>
        /// The XML parameter comment.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax ParameterComment(ParameterSyntax parameter, string comment) => Comment(SyntaxFactory.XmlParamElement(parameter.GetName()), comment);

        /// <summary>
        /// Gets an XML paramref element for the specified parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to reference.
        /// </param>
        /// <returns>
        /// The XML paramref element.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax ParamRef(ParameterSyntax parameter) => ParamRef(parameter.GetName());

        /// <summary>
        /// Gets an XML paramref element for the specified parameter name.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the parameter to reference.
        /// </param>
        /// <returns>
        /// The XML paramref element.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax ParamRef(string parameterName) => XmlEmptyElement(Constants.XmlTag.ParamRef).WithAttribute(SyntaxFactory.XmlNameAttribute(parameterName));

        /// <summary>
        /// Gets an XML para element with special or phrase text.
        /// </summary>
        /// <returns>
        /// The XML para element with special or phrase text.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax ParaOr() => Para(Constants.Comments.SpecialOrPhrase);

        /// <summary>
        /// Gets an XML element with boolean tags removed and text nodes combined.
        /// </summary>
        /// <param name="comment">
        /// The XML element to update.
        /// </param>
        /// <returns>
        /// The XML element without boolean tags and with combined text nodes.
        /// </returns>
        protected static XmlElementSyntax RemoveBooleansTags(XmlElementSyntax comment)
        {
            var withoutBooleans = comment.Without(comment.Content.Where(_ => _.IsBooleanTag()));
            var combinedTexts = CombineTexts(withoutBooleans);

            return CombineTexts(combinedTexts);
        }

        /// <summary>
        /// Gets a syntax node with the specified phrase replaced.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="comment">
        /// The syntax node to update.
        /// </param>
        /// <param name="text">
        /// The XML text containing the phrase to replace.
        /// </param>
        /// <param name="phrase">
        /// The phrase to search for.
        /// </param>
        /// <param name="replacement">
        /// The replacement text.
        /// </param>
        /// <returns>
        /// The syntax node with the replaced text, or the original node if the phrase was not found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static T ReplaceText<T>(T comment, XmlTextSyntax text, string phrase, string replacement) where T : SyntaxNode => ReplaceText(comment, text, new[] { phrase }, replacement);

        /// <summary>
        /// Gets a syntax node with the specified phrases replaced.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node.
        /// </typeparam>
        /// <param name="comment">
        /// The syntax node to update.
        /// </param>
        /// <param name="text">
        /// The XML text containing the phrases to replace.
        /// </param>
        /// <param name="phrases">
        /// The phrases to search for.
        /// </param>
        /// <param name="replacement">
        /// The replacement text.
        /// </param>
        /// <returns>
        /// The syntax node with the replaced text, or the original node if none of the phrases were found.
        /// </returns>
        protected static T ReplaceText<T>(T comment, XmlTextSyntax text, in ReadOnlySpan<string> phrases, string replacement) where T : SyntaxNode
        {
            var modifiedText = text.ReplaceText(phrases, replacement);

            return ReferenceEquals(text, modifiedText)
                   ? comment
                   : comment.ReplaceNode(text, modifiedText);
        }

        /// <summary>
        /// Gets an XML see element with a reference to the specified type name.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type to reference.
        /// </param>
        /// <returns>
        /// The XML see element with the type reference.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(string typeName) => SeeCref(typeName.AsTypeSyntax());

        /// <summary>
        /// Gets an XML see element with a reference to the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to reference.
        /// </param>
        /// <returns>
        /// The XML see element with the type reference.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type) => Cref(Constants.XmlTag.See, type);

        /// <summary>
        /// Gets an XML see element with a reference to a member of the specified type.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type containing the member.
        /// </param>
        /// <param name="member">
        /// The member name.
        /// </param>
        /// <returns>
        /// The XML see element with the member reference.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(string typeName, string member) => SeeCref(typeName.AsTypeSyntax(), member);

        /// <summary>
        /// Gets an XML see element with a reference to a member of the specified type.
        /// </summary>
        /// <param name="typeName">
        /// The name of the type containing the member.
        /// </param>
        /// <param name="member">
        /// The member syntax.
        /// </param>
        /// <returns>
        /// The XML see element with the member reference.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(string typeName, NameSyntax member) => SeeCref(typeName.AsTypeSyntax(), member);

        /// <summary>
        /// Gets an XML see element with a reference to a member of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type containing the member.
        /// </param>
        /// <param name="member">
        /// The member name.
        /// </param>
        /// <returns>
        /// The XML see element with the member reference.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, string member) => SeeCref(type, SyntaxFactory.ParseName(member));

        /// <summary>
        /// Gets an XML see element with a reference to a member of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type containing the member.
        /// </param>
        /// <param name="member">
        /// The member syntax.
        /// </param>
        /// <returns>
        /// The XML see element with the member reference.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeCref(TypeSyntax type, NameSyntax member) => Cref(Constants.XmlTag.See, type, member);

        /// <summary>
        /// Gets an XML see element with a langword attribute for the specified text.
        /// </summary>
        /// <param name="text">
        /// The langword value.
        /// </param>
        /// <returns>
        /// The XML see element with the langword attribute.
        /// </returns>
        protected static XmlEmptyElementSyntax SeeLangword(string text)
        {
            var attribute = XmlAttribute(Constants.XmlTag.Attribute.Langword, text);

            return XmlEmptyElement(Constants.XmlTag.See).WithAttribute(attribute);
        }

        /// <summary>
        /// Gets an <c>&lt;see langword="false"/&gt;</c> XML element.
        /// </summary>
        /// <returns>
        /// The XML element <c>&lt;see langword="false"/&gt;</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeLangword_False() => SeeLangword("false");

        /// <summary>
        /// Gets an <c>&lt;see langword="null"/&gt;</c> XML element.
        /// </summary>
        /// <returns>
        /// The XML element <c>&lt;see langword="null"/&gt;</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeLangword_Null() => SeeLangword("null");

        /// <summary>
        /// Gets an <c>&lt;see langword="true"/&gt;</c> XML element.
        /// </summary>
        /// <returns>
        /// The XML element <c>&lt;see langword="true"/&gt;</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax SeeLangword_True() => SeeLangword("true");

        /// <summary>
        /// Splits an XML comment at the first sentence boundary (first period), returning the first sentence in the element and outputting the remaining content for separate processing.
        /// </summary>
        /// <param name="comment">
        /// The XML element to split.
        /// </param>
        /// <param name="partsAfterSentence">
        /// On successful return, contains the content nodes after the first sentence, which can be moved to additional <c>&lt;para/&gt;</c> elements or remarks sections.
        /// </param>
        /// <returns>
        /// The XML element containing only the first sentence (up to and including the first period).
        /// </returns>
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

        /// <summary>
        /// Gets an XML typeparamref element for the specified type parameter name.
        /// </summary>
        /// <param name="name">
        /// The name of the type parameter to reference.
        /// </param>
        /// <returns>
        /// The XML typeparamref element.
        /// </returns>
        protected static XmlEmptyElementSyntax TypeParamRef(string name)
        {
            var attribute = XmlAttribute(Constants.XmlTag.Attribute.Name, name);

            return XmlEmptyElement(Constants.XmlTag.TypeParamRef).WithAttribute(attribute);
        }

        /// <summary>
        /// Gets an XML text attribute with the specified tag and text.
        /// </summary>
        /// <param name="tag">
        /// The attribute tag name.
        /// </param>
        /// <param name="text">
        /// The attribute text value.
        /// </param>
        /// <returns>
        /// The XML text attribute.
        /// </returns>
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

        /// <summary>
        /// Gets an XML element with the specified tag and no content.
        /// </summary>
        /// <param name="tag">
        /// The XML tag name.
        /// </param>
        /// <returns>
        /// The XML element with the specified tag.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax XmlElement(string tag) => SyntaxFactory.XmlElement(tag, default);

        /// <summary>
        /// Gets an XML element with the specified tag and content.
        /// </summary>
        /// <param name="tag">
        /// The XML tag name.
        /// </param>
        /// <param name="content">
        /// The content node.
        /// </param>
        /// <returns>
        /// The XML element with the specified tag and content.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax XmlElement(string tag, XmlNodeSyntax content) => SyntaxFactory.XmlElement(tag, content.ToSyntaxList());

        /// <summary>
        /// Gets an XML element with the specified tag and content nodes.
        /// </summary>
        /// <param name="tag">
        /// The XML tag name.
        /// </param>
        /// <param name="contents">
        /// The content nodes.
        /// </param>
        /// <returns>
        /// The XML element with the specified tag and content.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlElementSyntax XmlElement(string tag, IEnumerable<XmlNodeSyntax> contents) => SyntaxFactory.XmlElement(tag, contents.ToSyntaxList());

        /// <summary>
        /// Gets an empty XML element with the specified tag.
        /// </summary>
        /// <param name="tag">
        /// The XML tag name.
        /// </param>
        /// <returns>
        /// The empty XML element.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlEmptyElementSyntax XmlEmptyElement(string tag) => SyntaxFactory.XmlEmptyElement(tag);

        /// <summary>
        /// Gets an empty XML text with leading XML comment trivia.
        /// </summary>
        /// <returns>
        /// The empty XML text with a new line and leading XML comment.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax NewLineXmlText() => XmlText(string.Empty).WithLeadingXmlComment();

        /// <summary>
        /// Gets an empty XML text with trailing XML comment trivia.
        /// </summary>
        /// <returns>
        /// The empty XML text with trailing XML comment.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax TrailingNewLineXmlText() => XmlText(string.Empty).WithTrailingXmlComment();

        /// <summary>
        /// Gets an XML text with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text value.
        /// </param>
        /// <returns>
        /// The XML text.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        /// <summary>
        /// Gets an XML text with the specified syntax token list.
        /// </summary>
        /// <param name="textTokens">
        /// The syntax token list for the XML text.
        /// </param>
        /// <returns>
        /// The XML text with the specified tokens, or an empty XML text if the token list is empty.
        /// </returns>
        protected static XmlTextSyntax XmlText(in SyntaxTokenList textTokens)
        {
            if (textTokens.Count is 0)
            {
                return SyntaxFactory.XmlText();
            }

            return SyntaxFactory.XmlText(textTokens);
        }

        /// <summary>
        /// Gets an XML text with the specified syntax tokens.
        /// </summary>
        /// <param name="textTokens">
        /// The syntax tokens for the XML text.
        /// </param>
        /// <returns>
        /// The XML text with the specified tokens.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(textTokens.ToTokenList());

        private static List<XmlNodeSyntax> CommentEnd(string commentEnd, params XmlNodeSyntax[] commentEndNodes)
        {
            var skip = 0;
            XmlTextSyntax textCommentEnd;

            var length = commentEndNodes.Length;

            // add a white space at the end of the comment in case we have further texts
            if (length > 1 && commentEnd.Length > 0 && commentEnd[commentEnd.Length - 1] != ' ')
            {
                commentEnd += " ";
            }

            if (commentEndNodes.FirstOrDefault() is XmlTextSyntax text)
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

                if (commentEndNodes[last] is XmlTextSyntax additionalText)
                {
                    commentEndNodes[last] = XmlText(additionalText.TextTokens.WithoutLastXmlNewLine());
                }
            }

            var result = new List<XmlNodeSyntax>(1 + length - skip);
            result.Add(textCommentEnd);
            result.AddRange(commentEndNodes.Skip(skip));

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
                        var currentTextTokens = currentText.TextTokens.ToList();
                        var count = currentTextTokens.Count;
                        var nextTextTokens = nextText.TextTokens;

                        var lastToken = currentTextTokens[count - 1];
                        var firstToken = nextTextTokens[0];

                        var token = lastToken.WithText(lastToken.Text + firstToken.Text)
                                             .WithLeadingTriviaFrom(lastToken)
                                             .WithTrailingTriviaFrom(firstToken);

                        currentTextTokens[count - 1] = token;

                        currentTextTokens.AddRange(nextTextTokens.Skip(1));

                        var newText = currentText.WithTextTokens(currentTextTokens.ToTokenList());

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