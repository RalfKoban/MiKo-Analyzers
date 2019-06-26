﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class CommentExtensions
    {
        internal static string GetComment(this ISymbol symbol) => symbol is IParameterSymbol p
                                                                      ? GetComment(p, (p.ContainingSymbol as IMethodSymbol)?.GetDocumentationCommentXml())
                                                                      : Cleaned(GetCommentElement(symbol)).ConcatenatedWith();

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

        internal static IEnumerable<XElement> GetCommentElements(string commentXml, string xmlTag)
        {
            var element = GetCommentElement(commentXml);
            return element is null
                       ? Enumerable.Empty<XElement>() // happens in case of an invalid character
                       : element.Descendants(xmlTag);
        }

        internal static IEnumerable<XElement> GetExceptionCommentElements(string commentXml)
        {
            var commentElements = GetCommentElements(commentXml.RemoveAll(Constants.Markers.Symbols), Constants.XmlTag.Exception);
            return commentElements;
        }

        internal static string GetExceptionComment(string exceptionTypeFullName, string commentXml)
        {
            return FlattenComment(GetExceptionCommentElements(commentXml).Where(_ => _.Attribute("cref")?.Value == exceptionTypeFullName));
        }

        internal static IEnumerable<string> GetExceptionComments(string commentXml) => Cleaned(GetExceptionCommentElements(commentXml)).Where(_ => _ != null);

        internal static IEnumerable<string> Cleaned(IEnumerable<string> comments) => comments.Where(_ => _ != null).WithoutParaTags().Select(_ => _.Trim()).ToHashSet();

        internal static IEnumerable<string> Cleaned(params XElement[] elements) => Cleaned((IEnumerable<XElement>)elements);

        internal static string FirstWord(this string text)
        {
            var firstSpace = text.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
            var firstWord = firstSpace == -1 ? text : text.Substring(0, firstSpace);
            return firstWord;
        }

        private static XElement GetCommentElement(string commentXml)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = "<root>" + commentXml + "</root>";

            try
            {
                return XElement.Parse(xml);
            }
            catch (System.Xml.XmlException)
            {
                // happens in case of an invalid character
                return null;
            }
        }

        private static string FlattenComment(IEnumerable<XElement> comments)
        {
            var comment = comments.Any()
                              ? Cleaned(comments.Nodes().ConcatenatedWith())
                              : null;
            return comment;
        }

        private static IEnumerable<string> Cleaned(IEnumerable<XElement> elements)
        {
            foreach (var e in elements)
            {
                if (e is null)
                    continue;

                e.Descendants(Constants.XmlTag.Code).ToList().ForEach(_ => _.Remove());

                yield return Cleaned(e.Nodes().ConcatenatedWith());
            }
        }

        private static string Cleaned(string value)
        {
            if (value is null)
                return string.Empty;

            return value
                   .WithoutParaTags()
                   .RemoveAll(Constants.Markers.SymbolsAndLineBreaks)
                   .Replace("    ", " ")
                   .Replace("   ", " ")
                   .Replace("  ", " ")
                   .Trim();
        }
    }
}