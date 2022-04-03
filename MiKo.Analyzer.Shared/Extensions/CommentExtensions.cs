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