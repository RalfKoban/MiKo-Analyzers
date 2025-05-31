using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

// ncrunch: rdi off
// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal static class CommentExtensions
    {
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

        internal static string GetComment(this IParameterSymbol value, string commentXml)
        {
            var parameterName = value.Name;

            return FlattenComment(GetCommentElements(commentXml, Constants.XmlTag.Param).Where(_ => _.Attribute("name")?.Value == parameterName));
        }

        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag) => Cleaned(GetCommentElements(commentXml, xmlTag));

        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag, string xmlSubElement) => Cleaned(GetCommentElements(commentXml, xmlTag).Descendants(xmlSubElement));

        internal static string[] GetSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Summary));

        internal static string[] GetOverloadSummaries(this IMethodSymbol value) => GetOverloadSummaries(value.GetDocumentationCommentXml());

        internal static string[] GetOverloadSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Overloads, Constants.XmlTag.Summary));

        internal static string[] GetRemarks(this ISymbol value) => GetRemarks(value.GetDocumentationCommentXml());

        internal static string[] GetRemarks(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Remarks));

        internal static string[] GetReturns(this IMethodSymbol value) => GetReturns(value.GetDocumentationCommentXml());

        internal static string[] GetReturns(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Returns));

        internal static string[] GetValue(this IMethodSymbol value) => GetValue(value.GetDocumentationCommentXml());

        internal static string[] GetValue(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Value));

        internal static string[] GetExamples(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Example));

        internal static XElement GetCommentElement(this ISymbol value) => GetCommentElement(value.GetDocumentationCommentXml());

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

        internal static IEnumerable<XElement> GetCommentElements(this string value, string xmlTag)
        {
            var element = GetCommentElement(value);

            return GetCommentElements(element, xmlTag);
        }

        internal static IEnumerable<XElement> GetCommentElements(this XElement value, string xmlTag) => value is null
                                                                                                        ? Array.Empty<XElement>() // happens in case of an invalid character
                                                                                                        : value.Descendants(xmlTag);

        internal static IEnumerable<XElement> GetExceptionCommentElements(string commentXml)
        {
            var comment = commentXml.AsCachedBuilder().Without(Constants.Markers.Symbols).ToStringAndRelease();
            var commentElements = GetCommentElements(comment, Constants.XmlTag.Exception);

            return commentElements;
        }

        internal static string GetExceptionComment(string exceptionTypeFullName, string commentXml)
        {
            return FlattenComment(GetExceptionCommentElements(commentXml).Where(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value == exceptionTypeFullName));
        }

        internal static IEnumerable<string> GetExceptionComments(string commentXml) => Cleaned(GetExceptionCommentElements(commentXml)).WhereNotNull();

        internal static IEnumerable<string> GetExceptionsOfExceptionComments(this string commentXml)
        {
            return GetExceptionCommentElements(commentXml).Select(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value).WhereNotNull();
        }

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

        private static IEnumerable<string> Cleaned(IEnumerable<XElement> elements)
        {
            foreach (var e in elements)
            {
                yield return Cleaned(e);
            }
        }

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