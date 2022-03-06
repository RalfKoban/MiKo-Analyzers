using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class CommentExtensions
    {
        internal static string GetComment(this ISymbol symbol)
        {
            if (symbol is IParameterSymbol p)
            {
                // parameter might be method or property (setter or indexer)
                return GetComment(p, p.ContainingSymbol?.GetDocumentationCommentXml());
            }

            return Cleaned(GetCommentElement(symbol)).ConcatenatedWith();
        }

        internal static string GetComment(this IParameterSymbol parameter, string commentXml)
        {
            var parameterName = parameter.Name;

            return FlattenComment(GetCommentElements(commentXml, Constants.XmlTag.Param).Where(_ => _.Attribute("name")?.Value == parameterName));
        }

        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag) => Cleaned(GetCommentElements(commentXml, xmlTag));

        internal static IEnumerable<string> GetComments(string commentXml, string xmlTag, string xmlSubElement) => Cleaned(GetCommentElements(commentXml, xmlTag).Descendants(xmlSubElement));

        internal static IEnumerable<string> GetSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Summary));

        internal static IEnumerable<string> GetOverloadSummaries(this IMethodSymbol symbol) => GetOverloadSummaries(symbol.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetOverloadSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Overloads, Constants.XmlTag.Summary));

        internal static IEnumerable<string> GetRemarks(this ISymbol symbol) => GetRemarks(symbol.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetRemarks(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Remarks));

        internal static IEnumerable<string> GetReturns(this IMethodSymbol symbol) => GetReturns(symbol.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetReturns(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Returns));

        internal static IEnumerable<string> GetValue(this IMethodSymbol symbol) => GetValue(symbol.GetDocumentationCommentXml());

        internal static IEnumerable<string> GetValue(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Value));

        internal static IEnumerable<string> GetExamples(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Example));

        internal static XElement GetCommentElement(this ISymbol symbol) => GetCommentElement(symbol.GetDocumentationCommentXml());

        internal static XElement GetCommentElement(this string commentXml)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = string.Concat("<root>", commentXml, "</root>");

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

        internal static IEnumerable<XElement> GetCommentElements(this string commentXml, string xmlTag)
        {
            var element = GetCommentElement(commentXml);

            return GetCommentElements(element, xmlTag);
        }

        internal static IEnumerable<XElement> GetCommentElements(this XElement commentXml, string xmlTag)
        {
            return commentXml is null
                       ? Enumerable.Empty<XElement>() // happens in case of an invalid character
                       : commentXml.Descendants(xmlTag);
        }

        internal static IEnumerable<XElement> GetExceptionCommentElements(string commentXml)
        {
            var comment = commentXml.Without(Constants.Markers.Symbols);
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

        internal static IEnumerable<string> Cleaned(IEnumerable<string> comments) => comments.Where(_ => _ != null).WithoutParaTags().ToHashSet(_ => _.Trim());

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

            return value
                   .WithoutParaTags()
                   .Without(Constants.Markers.SymbolsAndLineBreaks)
                   .Replace("    ", " ")
                   .Replace("   ", " ")
                   .Replace("  ", " ")
                   .Trim();
        }
    }
}