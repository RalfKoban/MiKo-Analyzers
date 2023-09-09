using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class CommentExtensions
    {
        private static readonly string[] MultiWhitespaceStrings = { "    ", "   ", "  " };

        internal static string GetComment(this ISymbol value)
        {
            if (value is IParameterSymbol p)
            {
                // parameter might be method or property (setter or indexer)
                return GetComment(p, p.ContainingSymbol?.GetDocumentationCommentXml());
            }

            return Cleaned(GetCommentElement(value)).ConcatenatedWith();
        }

        internal static string GetComment(this IParameterSymbol value, string commentXml)
        {
            var parameterName = value.Name;

            return FlattenComment(GetCommentElements(commentXml, Constants.XmlTag.Param).Where(_ => _.Attribute("name")?.Value == parameterName));
        }

        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag) => Cleaned(GetCommentElements(commentXml, xmlTag));

        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag, string xmlSubElement) => Cleaned(GetCommentElements(commentXml, xmlTag).Descendants(xmlSubElement));

        internal static IReadOnlyCollection<string> GetSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Summary));

        internal static IEnumerable<string> GetOverloadSummaries(this IMethodSymbol value) => GetOverloadSummaries(value.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetOverloadSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Overloads, Constants.XmlTag.Summary));

        internal static IEnumerable<string> GetRemarks(this ISymbol value) => GetRemarks(value.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetRemarks(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Remarks));

        internal static IEnumerable<string> GetReturns(this IMethodSymbol value) => GetReturns(value.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetReturns(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Returns));

        internal static IEnumerable<string> GetValue(this IMethodSymbol value) => GetValue(value.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetValue(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Value));

        internal static IEnumerable<string> GetExamples(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Example));

        internal static XElement GetCommentElement(this ISymbol value) => GetCommentElement(value.GetDocumentationCommentXml());

        internal static XElement GetCommentElement(this string value)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = string.Concat("<root>", value, "</root>");

            try
            {
                return XElement.Parse(xml);
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

        internal static IEnumerable<XElement> GetCommentElements(this XElement value, string xmlTag)
        {
            return value is null
                   ? Enumerable.Empty<XElement>() // happens in case of an invalid character
                   : value.Descendants(xmlTag);
        }

        internal static IEnumerable<XElement> GetExceptionCommentElements(string commentXml)
        {
            var comment = new StringBuilder(commentXml).Without(Constants.Markers.Symbols).ToString();
            var commentElements = GetCommentElements(comment, Constants.XmlTag.Exception);

            return commentElements;
        }

        internal static string GetExceptionComment(string exceptionTypeFullName, string commentXml)
        {
            return FlattenComment(GetExceptionCommentElements(commentXml).Where(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value == exceptionTypeFullName));
        }

        internal static IEnumerable<string> GetExceptionComments(string commentXml) => Cleaned(GetExceptionCommentElements(commentXml)).Where(_ => _ != null);

        internal static IEnumerable<string> GetExceptionsOfExceptionComments(string commentXml)
        {
            return GetExceptionCommentElements(commentXml).Select(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value).Where(_ => _ != null);
        }

        internal static IReadOnlyCollection<string> Cleaned(IEnumerable<string> comments) => comments.Where(_ => _ != null).WithoutParaTags().ToHashSet(_ => _.Trim());

        internal static IEnumerable<string> Cleaned(params XElement[] elements) => Cleaned((IEnumerable<XElement>)elements);

        private static string FlattenComment(IEnumerable<XElement> comments)
        {
            if (comments.Any())
            {
                var comment = Cleaned(comments.Nodes().ConcatenatedWith());

                return comment;
            }

            return null;
        }

        private static IEnumerable<string> Cleaned(IEnumerable<XElement> elements)
        {
            foreach (var e in elements)
            {
                if (e is null)
                {
                    continue;
                }

                // remove all code elements
                var codeElements = e.Descendants(Constants.XmlTag.Code).ToList();
                codeElements.ForEach(_ => _.Remove());

                yield return Cleaned(e.Nodes().ConcatenatedWith());
            }
        }

        private static string Cleaned(string value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return new StringBuilder(value).WithoutParaTags()
                                           .Without(Constants.Markers.SymbolsAndLineBreaks)
                                           .ReplaceAllWithCheck(MultiWhitespaceStrings, " ")
                                           .ToString()
                                           .Trim();
        }
    }
}