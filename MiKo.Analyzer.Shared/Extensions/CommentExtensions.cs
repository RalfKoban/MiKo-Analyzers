using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

// ncrunch: rdi off
// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for extracting and cleaning XML documentation comments from symbols.
    /// </summary>
    internal static class CommentExtensions
    {
        /// <summary>
        /// Gets the cleaned comment for the specified symbol.
        /// </summary>
        /// <param name="value">
        /// The symbol to get the comment for.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the cleaned comment, or <see langword="null"/> if no comment is available.
        /// </returns>
        internal static string GetComment(this ISymbol value)
        {
            if (value is IParameterSymbol p)
            {
                // parameter might be method or property (setter or indexer)
                var containingSymbol = p.ContainingSymbol;

                if (containingSymbol is null)
                {
                    return null;
                }

                return GetComment(p, containingSymbol.GetDocumentationCommentXml());
            }

            return Cleaned(GetCommentElement(value));
        }

        /// <summary>
        /// Gets the cleaned <c>&lt;param&gt;</c> comment for the specified parameter from the provided XML documentation.
        /// </summary>
        /// <param name="value">
        /// The parameter symbol to get the <c>&lt;param&gt;</c> comment for.
        /// </param>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;param&gt;</c> comment from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the cleaned <c>&lt;param&gt;</c> comment for the parameter, or <see langword="null"/> if no comment is available.
        /// </returns>
        internal static string GetComment(this IParameterSymbol value, string commentXml)
        {
            var parameterName = value.Name;

            return FlattenComment(GetCommentElements(commentXml, Constants.XmlTag.Param).Where(_ => _.Attribute("name")?.Value == parameterName));
        }

        /// <summary>
        /// Gets all cleaned comments for the specified XML tag from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the comments from.
        /// </param>
        /// <param name="xmlTag">
        /// The XML tag to search for.
        /// </param>
        /// <returns>
        /// A sequence that contains the cleaned comments.
        /// </returns>
        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag) => Cleaned(GetCommentElements(commentXml, xmlTag));

        /// <summary>
        /// Gets all cleaned comments for the specified XML tag and sub-element from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the comments from.
        /// </param>
        /// <param name="xmlTag">
        /// The XML tag to search for.
        /// </param>
        /// <param name="xmlSubElement">
        /// The XML sub-element within the tag to extract.
        /// </param>
        /// <returns>
        /// A sequence that contains the cleaned comments.
        /// </returns>
        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag, string xmlSubElement) => Cleaned(GetCommentElements(commentXml, xmlTag).Descendants(xmlSubElement));

        /// <summary>
        /// Gets all cleaned <c>&lt;summary&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;summary&gt;</c> comments from.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;summary&gt;</c> comments.
        /// </returns>
        internal static string[] GetSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Summary));

        /// <summary>
        /// Gets all cleaned <c>&lt;summary&gt;</c> comments from within <c>&lt;overloads&gt;</c> tags for the specified method symbol.
        /// </summary>
        /// <param name="value">
        /// The method symbol to get the <c>&lt;summary&gt;</c> comments from within <c>&lt;overloads&gt;</c> tags for.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;summary&gt;</c> comments found within <c>&lt;overloads&gt;</c> tags.
        /// </returns>
        internal static string[] GetOverloadSummaries(this IMethodSymbol value) => GetOverloadSummaries(value.GetDocumentationCommentXml());

        /// <summary>
        /// Gets all cleaned <c>&lt;summary&gt;</c> comments from within <c>&lt;overloads&gt;</c> tags from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;summary&gt;</c> comments from within <c>&lt;overloads&gt;</c> tags from.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;summary&gt;</c> comments found within <c>&lt;overloads&gt;</c> tags.
        /// </returns>
        internal static string[] GetOverloadSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Overloads, Constants.XmlTag.Summary));

        /// <summary>
        /// Gets all cleaned <c>&lt;remarks&gt;</c> comments for the specified symbol.
        /// </summary>
        /// <param name="value">
        /// The symbol to get the <c>&lt;remarks&gt;</c> comments for.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;remarks&gt;</c> comments.
        /// </returns>
        internal static string[] GetRemarks(this ISymbol value) => GetRemarks(value.GetDocumentationCommentXml());

        /// <summary>
        /// Gets all cleaned <c>&lt;remarks&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;remarks&gt;</c> comments from.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;remarks&gt;</c> comments.
        /// </returns>
        internal static string[] GetRemarks(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Remarks));

        /// <summary>
        /// Gets all cleaned <c>&lt;returns&gt;</c> comments for the specified method symbol.
        /// </summary>
        /// <param name="value">
        /// The method symbol to get the <c>&lt;returns&gt;</c> comments for.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;returns&gt;</c> comments.
        /// </returns>
        internal static string[] GetReturns(this IMethodSymbol value) => GetReturns(value.GetDocumentationCommentXml());

        /// <summary>
        /// Gets all cleaned <c>&lt;returns&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;returns&gt;</c> comments from.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;returns&gt;</c> comments.
        /// </returns>
        internal static string[] GetReturns(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Returns));

        /// <summary>
        /// Gets all cleaned <c>&lt;value&gt;</c> comments for the specified method symbol.
        /// </summary>
        /// <param name="value">
        /// The method symbol to get the <c>&lt;value&gt;</c> comments for.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;value&gt;</c> comments.
        /// </returns>
        internal static string[] GetValue(this IMethodSymbol value) => GetValue(value.GetDocumentationCommentXml());

        /// <summary>
        /// Gets all cleaned <c>&lt;value&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;value&gt;</c> comments from.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;value&gt;</c> comments.
        /// </returns>
        internal static string[] GetValue(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Value));

        /// <summary>
        /// Gets all cleaned <c>&lt;example&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;example&gt;</c> comments from.
        /// </param>
        /// <returns>
        /// The array of cleaned <c>&lt;example&gt;</c> comments.
        /// </returns>
        internal static string[] GetExamples(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Example));

        /// <summary>
        /// Gets the comment element for the specified symbol.
        /// </summary>
        /// <param name="value">
        /// The symbol to get the comment element for.
        /// </param>
        /// <returns>
        /// The comment element, or <see langword="null"/> if no comment is available.
        /// </returns>
        internal static XElement GetCommentElement(this ISymbol value) => GetCommentElement(value.GetDocumentationCommentXml());

        /// <summary>
        /// Gets the comment element from the provided XML documentation <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The XML documentation <see cref="string"/> to parse.
        /// </param>
        /// <returns>
        /// The parsed comment element, or <see langword="null"/> if the XML is invalid or <see langword="null"/>.
        /// </returns>
        internal static XElement GetCommentElement(this string value)
        {
            if (value is null)
            {
                return null;
            }

            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var start = value.CountLeadingWhitespaces();
            var end = value.CountTrailingWhitespaces(start);

            var count = value.Length - end - start;

            var xml = StringBuilderCache.Acquire(13 + count)
                                        .Append("<root>")
                                        .Append(value, start, count)
                                        .Append("</root>")
                                        .ToStringAndRelease();

            try
            {
                return XElement.Parse(xml, LoadOptions.None);
            }
            catch (XmlException)
            {
                // happens in case of an invalid character
                return null;
            }
        }

        /// <summary>
        /// Gets all comment elements for the specified XML tag from the provided XML documentation <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The XML documentation <see cref="string"/> to extract the comment elements from.
        /// </param>
        /// <param name="xmlTag">
        /// The XML tag to search for.
        /// </param>
        /// <returns>
        /// A sequence that contains the comment elements.
        /// </returns>
        internal static IEnumerable<XElement> GetCommentElements(this string value, string xmlTag)
        {
            var element = GetCommentElement(value);

            return GetCommentElements(element, xmlTag);
        }

        /// <summary>
        /// Gets all comment elements for the specified XML tag from the provided XML element.
        /// </summary>
        /// <param name="value">
        /// The XML element to extract the comment elements from.
        /// </param>
        /// <param name="xmlTag">
        /// The XML tag to search for.
        /// </param>
        /// <returns>
        /// A sequence that contains the comment elements, or an empty collection if the element is <see langword="null"/>.
        /// </returns>
        internal static IEnumerable<XElement> GetCommentElements(this XElement value, string xmlTag) => value is null
                                                                                                        ? Array.Empty<XElement>() // happens in case of an invalid character
                                                                                                        : value.Descendants(xmlTag);

        /// <summary>
        /// Gets all <c>&lt;exception&gt;</c> comment elements from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;exception&gt;</c> comment elements from.
        /// </param>
        /// <returns>
        /// A sequence that contains the <c>&lt;exception&gt;</c> comment elements.
        /// </returns>
        internal static IEnumerable<XElement> GetExceptionCommentElements(string commentXml)
        {
            var comment = commentXml.AsCachedBuilder().Without(Constants.Markers.Symbols).ToStringAndRelease();
            var commentElements = GetCommentElements(comment, Constants.XmlTag.Exception);

            return commentElements;
        }

        /// <summary>
        /// Gets the cleaned <c>&lt;exception&gt;</c> comment for the specified exception type from the provided XML documentation.
        /// </summary>
        /// <param name="exceptionTypeFullName">
        /// The full name of the exception type to get the <c>&lt;exception&gt;</c> comment for.
        /// </param>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;exception&gt;</c> comment from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the cleaned <c>&lt;exception&gt;</c> comment, or <see langword="null"/> if no matching comment is found.
        /// </returns>
        internal static string GetExceptionComment(string exceptionTypeFullName, string commentXml)
        {
            return FlattenComment(GetExceptionCommentElements(commentXml).Where(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value == exceptionTypeFullName));
        }

        /// <summary>
        /// Gets all cleaned <c>&lt;exception&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="commentXml">
        /// The XML documentation to extract the <c>&lt;exception&gt;</c> comments from.
        /// </param>
        /// <returns>
        /// A sequence that contains the cleaned <c>&lt;exception&gt;</c> comments.
        /// </returns>
        internal static IEnumerable<string> GetExceptionComments(string commentXml) => Cleaned(GetExceptionCommentElements(commentXml)).WhereNotNull();

        /// <summary>
        /// Gets all exception types referenced in the <c>&lt;exception&gt;</c> comments from the provided XML documentation.
        /// </summary>
        /// <param name="value">
        /// The XML documentation to extract the exception types from.
        /// </param>
        /// <returns>
        /// A sequence that contains the exception type names.
        /// </returns>
        internal static IEnumerable<string> GetExceptionsOfExceptionComments(this string value)
        {
            return GetExceptionCommentElements(value).Select(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value).WhereNotNull();
        }

        /// <summary>
        /// Gets the cleaned and deduplicated array of comments from the provided collection.
        /// </summary>
        /// <param name="comments">
        /// The collection of comments to clean.
        /// </param>
        /// <returns>
        /// The array of cleaned and deduplicated comments.
        /// </returns>
        internal static string[] Cleaned(IEnumerable<string> comments)
        {
            List<string> cleanedComments = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var s in comments)
            {
                if (s is null)
                {
                    continue;
                }

                if (cleanedComments is null)
                {
                    cleanedComments = new List<string>(1);
                }

                cleanedComments.Add(s);
            }

            if (cleanedComments is null)
            {
                return Array.Empty<string>();
            }

            if (cleanedComments.Count is 1)
            {
                return new[] { TrimComment(cleanedComments[0]) };
            }

            return cleanedComments.ToHashSet(TrimComment).ToArray();

            string TrimComment(string comment) => comment.AsCachedBuilder().WithoutParaTags().Trimmed().ToStringAndRelease();
        }

        /// <summary>
        /// Gets the cleaned comment text from the provided XML element.
        /// </summary>
        /// <param name="element">
        /// The XML element to extract the comment from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the cleaned comment text, or <see langword="null"/> if the element is <see langword="null"/>.
        /// </returns>
        internal static string Cleaned(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            // remove all code elements
            List<XElement> codeElements = null;

            foreach (var descendant in element.Descendants(Constants.XmlTag.Code))
            {
                if (codeElements is null)
                {
                    codeElements = new List<XElement>(1);
                }

                codeElements.Add(descendant);
            }

            if (codeElements?.Count > 0)
            {
                codeElements.ForEach(_ => _.Remove());
            }

            return Cleaned(element.Nodes());
        }

        /// <summary>
        /// Gets the collection of cleaned comment texts from the provided XML elements.
        /// </summary>
        /// <param name="elements">
        /// The XML elements to extract the comments from.
        /// </param>
        /// <returns>
        /// A sequence that contains the cleaned comment texts.
        /// </returns>
        private static IEnumerable<string> Cleaned(IEnumerable<XElement> elements)
        {
            foreach (var e in elements)
            {
                yield return Cleaned(e);
            }
        }

        /// <summary>
        /// Gets the cleaned comment text from the provided XML nodes.
        /// </summary>
        /// <param name="nodes">
        /// The XML nodes to extract the comment from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the cleaned comment text.
        /// </returns>
        private static string Cleaned(IEnumerable<XNode> nodes)
        {
            var builder = StringBuilderCache.Acquire();

            foreach (var node in nodes)
            {
                if (node != null)
                {
                    builder.Append(node);
                }
            }

            var cleaned = string.Empty;

            if (builder.Length > 0)
            {
                cleaned = builder.WithoutParaTags()
                                 .Without(Constants.Markers.SymbolsAndLineBreaks)
                                 .WithoutMultipleWhiteSpaces()
                                 .Trim();
            }

            StringBuilderCache.Release(builder);

            return cleaned;
        }

        /// <summary>
        /// Gets the flattened and cleaned comment text from the provided XML elements.
        /// </summary>
        /// <param name="comments">
        /// The XML elements containing the comments to flatten.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the flattened and cleaned comment text, or <see langword="null"/> if no comments are available.
        /// </returns>
        private static string FlattenComment(IEnumerable<XElement> comments)
        {
            if (comments.Any())
            {
                return Cleaned(comments.Nodes());
            }

            return null;
        }
    }
}