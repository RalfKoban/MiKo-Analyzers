using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using MiKoSolutions.Analyzers.Linguistics;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s related to comments and XML documentation.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        internal static readonly SyntaxTrivia XmlCommentExterior = SyntaxFactory.DocumentationCommentExterior(Constants.Comments.XmlCommentExterior + " ");

        internal static readonly SyntaxTrivia[] XmlCommentStart =
                                                                  {
                                                                      SyntaxFactory.ElasticCarriageReturnLineFeed, // use elastic one to allow formatting to be done automatically
                                                                      XmlCommentExterior,
                                                                  };

        private static readonly string[] Booleans = { "true", "false", "True", "False", "TRUE", "FALSE" };

        private static readonly string[] Nulls = { "null", "Null", "NULL" };

        internal static Location GetContentsLocation(this XmlElementSyntax value)
        {
            var contents = value.Content;
            var span = contents.Span;

            if (contents.Count > 0)
            {
                var start = FindStart(contents);
                var end = FindEnd(contents);

                span = TextSpan.FromBounds(start, end);
            }

            if (span.IsEmpty)
            {
                var start = value.StartTag.GreaterThanToken.SpanStart;
                var end = value.EndTag.LessThanSlashToken.SpanStart + 1;

                span = TextSpan.FromBounds(start, end);
            }

            return Location.Create(value.SyntaxTree, span);

            int FindStart(SyntaxList<XmlNodeSyntax> list)
            {
                XmlNodeSyntax first = null;

                // try to find the first syntax that is not only an XmlCommentExterior
                for (int index = 0, listCount = list.Count; index < listCount; index++)
                {
                    first = list[index];

                    if (first is XmlTextSyntax t && t.IsWhiteSpaceOnlyText())
                    {
                        continue;
                    }

                    break;
                }

                var start = first?.SpanStart ?? -1;

                // try to get rid of white-spaces at the beginning
                if (first is XmlTextSyntax firstText)
                {
                    var token = firstText.TextTokens.FirstOrDefault(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken) && _.Text.IsNullOrWhiteSpace() is false);

                    if (token.IsDefaultValue())
                    {
                        // we did not find it, so it seems like an empty text
                        return firstText.SpanStart;
                    }

                    var text = token.Text;

                    var offset = text.Length - text.AsSpan().TrimStart().Length;

                    start = token.SpanStart + offset;
                }

                return start;
            }

            int FindEnd(SyntaxList<XmlNodeSyntax> list)
            {
                XmlNodeSyntax last = null;

                // try to find the last syntax that is not only an XmlCommentExterior
                for (var i = list.Count - 1; i > -1; i--)
                {
                    last = list[i];

                    if (last is XmlTextSyntax t && t.IsWhiteSpaceOnlyText())
                    {
                        continue;
                    }

                    break;
                }

                var end = last?.Span.End ?? -1;

                // try to get rid of white-spaces at the end
                if (last is XmlTextSyntax lastText)
                {
                    var token = lastText.TextTokens.LastOrDefault(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken) && _.Text.IsNullOrWhiteSpace() is false);

                    if (token.IsDefaultValue())
                    {
                        // we did not find it
                    }
                    else
                    {
                        var text = token.Text;

                        var offset = text.Length - text.AsSpan().TrimEnd().Length;

                        end = token.Span.End - offset;
                    }
                }

                return end;
            }
        }

        internal static XmlTextAttributeSyntax GetNameAttribute(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlElementSyntax e: return e.GetAttributes<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Name);
                case XmlEmptyElementSyntax ee: return ee.Attributes.OfType<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Name);
                default: return null;
            }
        }

        internal static SyntaxTrivia[] GetComment(this SyntaxNode value) => value.GetLeadingTrivia().Concat(value.GetTrailingTrivia()).Where(_ => _.IsComment()).ToArray();

        internal static SyntaxNode FindSyntaxNodeWithLeadingComment(this SyntaxNode value)
        {
            while (true)
            {
                if (value is null)
                {
                    return null;
                }

                var list = value.GetLeadingTrivia();

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var listCount = list.Count;

                if (listCount > 0)
                {
                    for (var index = 0; index < listCount; index++)
                    {
                        var trivia = list[index];

                        if (trivia.IsComment())
                        {
                            return value;
                        }
                    }
                }

                value = value.FirstChild();
            }
        }

        internal static string[] GetLeadingComments(this SyntaxNode value)
        {
            if (value is null)
            {
                return Array.Empty<string>();
            }

            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count is 0)
            {
                return Array.Empty<string>();
            }

            return leadingTrivia.Where(_ => _.IsComment())
                                .Select(_ => _.ToTextOnlyString())
                                .Where(_ => _.Length > 0)
                                .ToArray();
        }

        internal static XmlTextAttributeSyntax GetListType(this XmlElementSyntax list) => list.GetAttributes<XmlTextAttributeSyntax>()
                                                                                              .FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Type);

        internal static string GetListType(this XmlTextAttributeSyntax listType) => listType.GetTextWithoutTrivia();

        internal static ParameterSyntax[] GetParameters(this XmlElementSyntax value)
        {
            foreach (var ancestor in value.Ancestors())
            {
                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax method:
                        return method.ParameterList.Parameters.ToArray();

                    case IndexerDeclarationSyntax indexer:
                        return indexer.ParameterList.Parameters.ToArray();

                    case BaseTypeDeclarationSyntax _:
                        return Array.Empty<ParameterSyntax>();
                }
            }

            return Array.Empty<ParameterSyntax>();
        }

        internal static bool HasDocumentationCommentTriviaSyntax(this SyntaxNode value)
        {
            var token = value.FindStructuredTriviaToken();

            return token.HasStructuredTrivia && token.HasDocumentationCommentTriviaSyntax();
        }

        internal static DocumentationCommentTriviaSyntax[] GetDocumentationCommentTriviaSyntax(this SyntaxNode value, in SyntaxKind kind = SyntaxKind.SingleLineDocumentationCommentTrivia)
        {
            var token = value.FindStructuredTriviaToken();

            if (token.HasStructuredTrivia)
            {
                var comment = token.GetDocumentationCommentTriviaSyntax(kind);

                if (comment != null)
                {
                    return comment;
                }
            }

            return Array.Empty<DocumentationCommentTriviaSyntax>();
        }

        internal static string GetTextTrimmed(this XmlElementSyntax value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            var builder = StringBuilderCache.Acquire();

            var trimmed = value.GetTextWithoutTrivia(builder)
                               .WithoutParaTags()
                               .WithoutNewLines()
                               .WithoutMultipleWhiteSpaces()
                               .Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static string GetTextTrimmed(this XmlTextSyntax value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            var builder = StringBuilderCache.Acquire();

            var trimmed = value.GetTextWithoutTrivia(builder)
                               .WithoutParaTags()
                               .WithoutNewLines()
                               .WithoutMultipleWhiteSpaces()
                               .Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static string GetTextWithoutTrivia(this XmlTextAttributeSyntax value)
        {
            if (value is null)
            {
                return null;
            }

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return string.Empty;
            }

            var builder = StringBuilderCache.Acquire();

            textTokens.GetTextWithoutTrivia(builder);

            var trimmed = builder.Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static string GetTextWithoutTrivia(this XmlTextSyntax value)
        {
            if (value is null)
            {
                return null;
            }

            var builder = StringBuilderCache.Acquire();

            GetTextWithoutTrivia(value, builder);

            var trimmed = builder.Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static StringBuilder GetTextWithoutTrivia(this XmlTextSyntax value, StringBuilder builder)
        {
            if (value is null)
            {
                return builder;
            }

            return value.TextTokens.GetTextWithoutTrivia(builder);
        }

        internal static StringBuilder GetTextWithoutTrivia(this XmlElementSyntax value, StringBuilder builder)
        {
            if (value is null)
            {
                return builder;
            }

            var content = value.Content;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var contentCount = content.Count;

            if (contentCount > 0)
            {
                for (var index = 0; index < contentCount; index++)
                {
                    var node = content[index];
                    var tagName = node.GetXmlTagName();

                    switch (tagName)
                    {
                        case "i":
                        case "b":
                        case Constants.XmlTag.C:
                            continue; // ignore code
                    }

                    switch (node)
                    {
                        case XmlTextSyntax text:
                            GetTextWithoutTrivia(text, builder);

                            break;

                        case XmlEmptyElementSyntax empty:
                            builder.Append(empty.WithoutTrivia());

                            break;

                        case XmlElementSyntax e:
                            GetTextWithoutTrivia(e, builder);

                            break;
                    }
                }
            }

            return builder.WithoutXmlCommentExterior();
        }

        internal static IEnumerable<string> GetTextWithoutTriviaLazy(this XmlTextSyntax value)
        {
            if (value is null)
            {
                yield break;
            }

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount > 0)
            {
                for (var index = 0; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        continue;
                    }

                    yield return token.ValueText;
                }
            }
        }

        internal static IReadOnlyList<XmlElementSyntax> GetExampleXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Example);

        internal static IReadOnlyList<XmlElementSyntax> GetExceptionXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Exception);

        internal static IReadOnlyList<XmlElementSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Summary);

        internal static IEnumerable<XmlNodeSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value, ISet<string> tags)
        {
            var summaryXmls = value.GetSummaryXmls();

            if (summaryXmls.Count is 0)
            {
                return Array.Empty<XmlNodeSyntax>();
            }

            return GetSummaryXmlsCore(summaryXmls, tags);
        }

        internal static IReadOnlyList<XmlElementSyntax> GetRemarksXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Remarks);

        internal static IReadOnlyList<XmlElementSyntax> GetReturnsXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Returns);

        internal static IReadOnlyList<XmlElementSyntax> GetValueXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Value);

        /// <summary>
        /// Gets only those XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,ISet{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,ISet{string})"/>
        internal static IReadOnlyList<XmlElementSyntax> GetXmlSyntax(this SyntaxNode value, string tag)
        {
            List<XmlElementSyntax> elements = null;

            foreach (var element in value.AllDescendantNodes<XmlElementSyntax>())
            {
                if (element.GetName() == tag)
                {
                    if (elements is null)
                    {
                        elements = new List<XmlElementSyntax>(1);
                    }

                    elements.Add(element);
                }
            }

            return (IReadOnlyList<XmlElementSyntax>)elements ?? Array.Empty<XmlElementSyntax>();
        }

        /// <summary>
        /// Gets only those XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation syntax.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,ISet{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        internal static IReadOnlyList<XmlElementSyntax> GetXmlSyntax(this SyntaxNode value, ISet<string> tags)
        {
            // we have to delve into the trivia to find the XML syntax nodes
            List<XmlElementSyntax> elements = null;

            foreach (var element in value.AllDescendantNodes<XmlElementSyntax>())
            {
                if (tags.Contains(element.GetName()))
                {
                    if (elements is null)
                    {
                        elements = new List<XmlElementSyntax>(1);
                    }

                    elements.Add(element);
                }
            }

            return (IReadOnlyList<XmlElementSyntax>)elements ?? Array.Empty<XmlElementSyntax>();
        }

        /// <summary>
        /// Gets only those XML elements that are empty (have NO content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the XML elements that are empty (have NO content) and the given tag out of the documentation syntax.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,ISet{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,ISet{string})"/>
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode value, string tag)
        {
            // we have to delve into the trivia to find the XML syntax nodes
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var element in value.AllDescendantNodes<XmlEmptyElementSyntax>())
            {
                if (element.GetName() == tag)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Gets only those XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </summary>
        /// <param name="value">
        /// The starting point of the XML elements to consider.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,ISet{string})"/>
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode value, ISet<string> tags)
        {
            // we have to delve into the trivia to find the XML syntax nodes
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var element in value.AllDescendantNodes<XmlEmptyElementSyntax>())
            {
                if (tags.Contains(element.GetName()))
                {
                    yield return element;
                }
            }
        }

        internal static XmlCrefAttributeSyntax GetCref(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax e: return e.Attributes.GetCref();
                case XmlElementSyntax e: return e.StartTag.Attributes.GetCref();
                default: return null;
            }
        }

        internal static XmlCrefAttributeSyntax GetCref(this SyntaxNode value, string name)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax e when e.GetName() == name: return e.Attributes.GetCref();
                case XmlElementSyntax e when e.GetName() == name: return e.StartTag.Attributes.GetCref();
                default: return null;
            }
        }

        internal static TypeSyntax GetCrefType(this XmlCrefAttributeSyntax value)
        {
            if (value != null)
            {
                switch (value.Cref)
                {
                    case NameMemberCrefSyntax n: return n.Name;
                    case QualifiedCrefSyntax q when q.Member is NameMemberCrefSyntax n: return n.Name;
                }
            }

            return null;
        }

        internal static bool HasComment(this SyntaxNode value) => value.HasLeadingComment() || value.HasTrailingComment();

        internal static bool HasLeadingComment(this SyntaxNode value) => value.GetLeadingTrivia().HasComment();

        internal static bool HasTrailingComment(this SyntaxNode value) => value != null && value.GetTrailingTrivia().HasComment();

        internal static bool IsWrongBooleanTag(this SyntaxNode value) => value.IsCBool() || value.IsBBool() || value.IsValueBool() || value.IsCodeBool();

        internal static bool IsWrongNullTag(this SyntaxNode value) => value.IsCNull() || value.IsBNull() || value.IsValueNull() || value.IsCodeNull();

        internal static bool IsBooleanTag(this SyntaxNode value) => value.IsSeeLangwordBool() || value.IsWrongBooleanTag();

        internal static bool IsBBool(this SyntaxNode value) => value.Is("b", Booleans);

        internal static bool IsBNull(this SyntaxNode value) => value.Is("b", Nulls);

        internal static bool IsCBool(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Booleans);

        internal static bool IsCNull(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Nulls);

        internal static bool IsCodeBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Code, Booleans);

        internal static bool IsCodeNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Code, Nulls);

        internal static bool IsCode(this SyntaxNode value) => value is XmlElementSyntax xes && xes.IsCode();

        internal static bool IsCode(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.Code;

        internal static bool IsException(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.Exception;

        internal static bool IsExceptionCommentFor<T>(this XmlElementSyntax value) where T : Exception => IsExceptionComment(value, typeof(T));

        internal static bool IsExceptionComment(this XmlElementSyntax value, Type exceptionType)
        {
            var list = value.GetAttributes<XmlCrefAttributeSyntax>();

            if (list.Count > 0)
            {
                var type = list[0].GetCrefType();

                return type != null && type.IsException(exceptionType);
            }

            return false;
        }

        internal static bool IsWhiteSpaceOnlyText(this SyntaxNode value) => value is XmlTextSyntax text && text.IsWhiteSpaceOnlyText();

        internal static bool IsWhiteSpaceOnlyText(this XmlTextSyntax value)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var text in value.GetTextWithoutTriviaLazy())
            {
                if (text.IsNullOrWhiteSpace() is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsPara(this SyntaxNode value) => value.IsXmlTag(Constants.XmlTag.Para);

        internal static bool IsSeeLangword(this SyntaxNode value)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() is Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                switch (attribute?.GetName())
                {
                    case Constants.XmlTag.Attribute.Langword:
                    case Constants.XmlTag.Attribute.Langref:
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool IsSeeLangwordBool(this SyntaxNode value)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() is Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                switch (attribute?.GetName())
                {
                    case Constants.XmlTag.Attribute.Langword:
                    case Constants.XmlTag.Attribute.Langref:
                    {
                        foreach (var token in attribute.DescendantTokens())
                        {
                            var tokenValueText = token.ValueText;

                            if ("true".Equals(tokenValueText, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }

                            if ("false".Equals(tokenValueText, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }

                        break;
                    }
                }
            }

            return false;
        }

        internal static bool IsSee(this XmlEmptyElementSyntax value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.See, attributeNames);

        internal static bool IsSee(this XmlElementSyntax value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.See, attributeNames);

        internal static bool IsSeeAlso(this XmlEmptyElementSyntax value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        internal static bool IsSeeAlso(this XmlElementSyntax value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        internal static bool IsValueBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Booleans);

        internal static bool IsValueNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Nulls);

        internal static bool IsXml(this SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.XmlElement:
                case SyntaxKind.XmlEmptyElement:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsXmlTag(this SyntaxNode value, string tagName)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax xees when xees.GetName() == tagName:
                case XmlElementSyntax xes when xes.GetName() == tagName:
                    return true;

                default:
                    return false;
            }
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, string phrase, string replacement)
        {
            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return value;
            }

            var map = new Dictionary<SyntaxToken, SyntaxToken>(1);

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var replaced = false;

                var result = text;

                if (text.Contains(phrase))
                {
                    result = result.AsCachedBuilder().ReplaceWithProbe(phrase, replacement).ToStringAndRelease();

                    replaced = true;
                }

                if (replaced)
                {
                    map[token] = token.WithText(result);
                }
            }

            if (map.Count is 0)
            {
                return value;
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, in ReadOnlySpan<string> phrases, string replacement)
        {
            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return value;
            }

            var phrasesLength = phrases.Length;

            var map = new Dictionary<SyntaxToken, SyntaxToken>(1);

            for (var tokenIndex = 0; tokenIndex < textTokensCount; tokenIndex++)
            {
                var token = textTokens[tokenIndex];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var replaced = false;

                var result = text.AsCachedBuilder();

                for (var phraseIndex = 0; phraseIndex < phrasesLength; phraseIndex++)
                {
                    var phrase = phrases[phraseIndex];

                    if (text.Contains(phrase))
                    {
                        result.ReplaceWithProbe(phrase, replacement);

                        replaced = true;
                    }
                }

                if (replaced)
                {
                    map[token] = token.WithText(result.ToStringAndRelease());
                }
                else
                {
                    StringBuilderCache.Release(result);
                }
            }

            if (map.Count is 0)
            {
                return value;
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static XmlTextSyntax ReplaceFirstText(this XmlTextSyntax value, string phrase, string replacement)
        {
            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return value;
            }

            var map = new Dictionary<SyntaxToken, SyntaxToken>(1);

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var index = text.IndexOf(phrase, StringComparison.Ordinal);

                if (index > -1)
                {
                    var result = text.AsSpan(0, index).ConcatenatedWith(replacement, text.AsSpan(index + phrase.Length));

                    map[token] = token.WithText(result);
                }
            }

            if (map.Count is 0)
            {
                return value;
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static T WithAttribute<T>(this T value, XmlAttributeSyntax attribute) where T : XmlNodeSyntax
        {
            switch (value)
            {
                case XmlElementSyntax xes:
                {
                    var newAttributes = xes.StartTag.Attributes.Add(attribute);
                    var newStartTag = xes.StartTag.WithAttributes(newAttributes);

                    return xes.ReplaceNode(xes.StartTag, newStartTag) as T;
                }

                case XmlEmptyElementSyntax xees:
                {
                    var newAttributes = xees.Attributes.Add(attribute);

                    return xees.WithAttributes(newAttributes) as T;
                }

                default:
                    return value;
            }
        }

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(contents.ToSyntaxList());

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(contents.ToSyntaxList());

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(contents.ToSyntaxList());

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(contents.ToSyntaxList());

        internal static T WithLeadingXmlComment<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentStart);

        internal static T WithLeadingXmlCommentExterior<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentExterior);

        internal static XmlElementSyntax WithoutFirstXmlNewLine(this XmlElementSyntax value)
        {
            return value.WithContent(value.Content.WithoutFirstXmlNewLine());
        }

        internal static XmlTextSyntax WithoutFirstXmlNewLine(this XmlTextSyntax value)
        {
            return value.WithTextTokens(value.TextTokens.WithoutFirstXmlNewLine()).WithoutLeadingTrivia();
        }

        internal static XmlTextSyntax WithoutLastXmlNewLine(this XmlTextSyntax value)
        {
            var textTokens = value.TextTokens.WithoutLastXmlNewLine();

            return value.WithTextTokens(textTokens);
        }

        internal static XmlTextSyntax WithoutLeadingXmlComment(this XmlTextSyntax value)
        {
            var tokens = value.TextTokens;

            if (tokens.Count >= 2)
            {
                var newTokens = tokens.WithoutFirstXmlNewLine();

                if (newTokens.Count > 0)
                {
                    var token = newTokens[0];

                    newTokens = newTokens.Replace(token, token.WithText(token.Text.AsSpan().TrimStart()));
                }

                return newTokens.AsXmlText();
            }

            return value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, string text) => value.Content.WithoutText(text);

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, in ReadOnlySpan<string> texts) => value.Content.WithoutText(texts);

        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, string text) => value.WithoutTrailing(new[] { text });

        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, in ReadOnlySpan<string> texts)
        {
            var textTokens = value.TextTokens.ToArray();

            var replaced = false;

            for (var i = textTokens.Length - 1; i >= 0; i--)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var originalText = token.Text.AsSpan();

                if (originalText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                for (int textIndex = 0, textsLength = texts.Length; textIndex < textsLength; textIndex++)
                {
                    var text = texts[textIndex];

                    if (originalText.EndsWith(text, StringComparison.OrdinalIgnoreCase))
                    {
                        var modifiedText = originalText.WithoutSuffix(text, StringComparison.OrdinalIgnoreCase);

                        textTokens[i] = token.WithText(modifiedText);
                        replaced = true;

                        break;
                    }
                }
            }

            if (replaced)
            {
                return textTokens.AsXmlText();
            }

            return value;
        }

        internal static XmlTextSyntax WithoutTrailingCharacters(this XmlTextSyntax value, in ReadOnlySpan<char> characters)
        {
            var textTokens = value.TextTokens.ToList();

            var replaced = false;

            for (var i = textTokens.Count - 1; i >= 0; i--)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var originalText = token.Text.AsSpan();

                if (originalText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var modifiedText = originalText.TrimEnd(characters);

                textTokens[i] = token.WithText(modifiedText);
                replaced = true;

                break;
            }

            if (replaced)
            {
                return textTokens.AsXmlText();
            }

            return value;
        }

        internal static XmlTextSyntax WithoutTrailingXmlComment(this XmlTextSyntax value)
        {
            var tokens = value.TextTokens;

            switch (tokens.Count)
            {
                case 0:
                    return value;

                case 1:
                    var token = tokens[0];

                    if (token.HasTrailingTrivia)
                    {
                        return value.WithTextTokens(tokens.Replace(token, token.WithoutTrailingTrivia()));
                    }

                    return value;

                default:
                {
                    // remove last "\r\n" token and remove '  /// ' trivia of last token
                    return value.WithoutLastXmlNewLine();
                }
            }
        }

        internal static XmlElementSyntax WithoutWhitespaceOnlyComment(this XmlElementSyntax value)
        {
            var texts = value.Content.OfType<XmlTextSyntax>();
            var textsCount = texts.Count;

            if (textsCount > 0)
            {
                var text = textsCount is 1
                           ? texts[0]
                           : texts[textsCount - 2];

                return WithoutWhitespaceOnlyCommentLocal(text);
            }

            return value;

            XmlElementSyntax WithoutWhitespaceOnlyCommentLocal(XmlTextSyntax text)
            {
                var newText = text.WithoutLeadingXmlComment();
                var newTextTokens = newText.TextTokens;

                switch (newTextTokens.Count)
                {
                    case 0:
                    case 1 when newTextTokens[0].ValueText.IsNullOrWhiteSpace():
                        return value.Without(text);

                    default:
                        return value.ReplaceNode(text, newText);
                }
            }
        }

        internal static StringBuilder WithoutXmlCommentExterior(this StringBuilder value) => value.Without(Constants.Comments.XmlCommentExterior);

        internal static string WithoutXmlCommentExterior(this SyntaxNode value)
        {
            var builder = StringBuilderCache.Acquire();

            var trimmed = builder.Append(value).WithoutXmlCommentExterior().Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this XmlElementSyntax value, in ReadOnlySpan<string> startTexts) => value.Content.WithoutStartText(startTexts);

        internal static XmlTextSyntax WithoutStartText(this XmlTextSyntax value, in ReadOnlySpan<string> startTexts)
        {
            if (startTexts.Length is 0)
            {
                return value;
            }

            var tokens = value.TextTokens;

            if (tokens.Count is 0)
            {
                return value;
            }

            var textTokens = tokens.ToList();

            for (int i = 0, textTokensCount = textTokens.Count; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var originalText = token.Text.AsSpan().TrimStart();

                if (originalText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                foreach (var startText in startTexts)
                {
                    if (originalText.StartsWith(startText))
                    {
                        var modifiedText = originalText.Slice(startText.Length);

                        textTokens[i] = token.WithText(modifiedText);

                        return textTokens.AsXmlText();
                    }
                }
            }

            return value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this XmlElementSyntax value, string startText, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase) => value.Content.WithStartText(startText, firstWordAdjustment);

        internal static XmlTextSyntax WithStartText(this XmlTextSyntax value, string startText, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase)
        {
            var tokens = value.TextTokens;

            if (tokens.Count is 0)
            {
                return startText.AsXmlText();
            }

            var textTokens = tokens.ToList();

            var replaced = false;

            if (startText.IsNullOrWhiteSpace())
            {
                // get rid of first new line token as we do not need it anymore
                if (textTokens.Count > 0 && textTokens[0].IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    textTokens.RemoveAt(0);

                    replaced = true;
                }
            }

            for (int i = 0, count = textTokens.Count; i < count; i++)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var originalText = token.Text;

                if (originalText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var space = i is 0 ? string.Empty : " ";

                // replace 3rd person word by infinite word if configured
                var continuation = originalText.AdjustFirstWord(firstWordAdjustment);

                var modifiedText = space + startText + continuation;

                textTokens[i] = token.WithText(modifiedText);

                replaced = true;

                break;
            }

            if (replaced)
            {
                return textTokens.AsXmlText();
            }

            return startText.AsXmlText();
        }

        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        private static IEnumerable<XmlNodeSyntax> GetSummaryXmlsCore(IReadOnlyList<XmlElementSyntax> summaryXmls, ISet<string> tags)
        {
            for (int index = 0, count = summaryXmls.Count; index < count; index++)
            {
                var summary = summaryXmls[index];

                // we have to delve into the trivia to find the XML syntax nodes
                foreach (var node in summary.AllDescendantNodes())
                {
                    switch (node)
                    {
                        case XmlElementSyntax e when tags.Contains(e.GetName()):
                            yield return e;

                            break;

                        case XmlEmptyElementSyntax ee when tags.Contains(ee.GetName()):
                            yield return ee;

                            break;
                    }
                }
            }
        }

        private static bool Is(this SyntaxNode value, string tagName, in ReadOnlySpan<string> contents)
        {
            if (value is XmlElementSyntax syntax && string.Equals(tagName, syntax.GetName(), StringComparison.OrdinalIgnoreCase))
            {
                var content = syntax.Content.ToString().AsSpan().Trim();

                return content.EqualsAny(contents, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool IsEmpty(this SyntaxNode value, string tagName, IEnumerable<string> attributeNames)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

        private static bool IsNonEmpty(this SyntaxNode value, string tagName, IEnumerable<string> attributeNames)
        {
            if (value is XmlElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.StartTag.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

        private static SyntaxToken FindStructuredTriviaToken(this SyntaxNode value)
        {
            if (value != null)
            {
                if (value.HasStructuredTrivia)
                {
                    var children = value.ChildNodesAndTokens();

                    var count = children.Count;

                    if (count > 0)
                    {
                        for (var index = 0; index < count; index++)
                        {
                            var child = children[index];

                            if (child.IsToken)
                            {
                                var childToken = child.AsToken();

                                if (childToken.HasStructuredTrivia)
                                {
                                    return childToken;
                                }

                                // no structure, so maybe it is the first descendant token to use
                                break;
                            }
                        }
                    }

                    var token = value.FirstDescendantToken();

                    if (token.HasStructuredTrivia)
                    {
                        return token;
                    }
                }
            }

            return default;
        }
    }
}