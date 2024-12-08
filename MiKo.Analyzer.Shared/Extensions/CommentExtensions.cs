﻿using System;
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
                return GetComment(p, p.ContainingSymbol?.GetDocumentationCommentXml());
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

        internal static IReadOnlyCollection<string> GetSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Summary));

        internal static IReadOnlyCollection<string> GetOverloadSummaries(this IMethodSymbol value) => GetOverloadSummaries(value.GetDocumentationCommentXml());

        internal static IReadOnlyCollection<string> GetOverloadSummaries(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Overloads, Constants.XmlTag.Summary));

        internal static IReadOnlyCollection<string> GetRemarks(this ISymbol value) => GetRemarks(value.GetDocumentationCommentXml());

        internal static IReadOnlyCollection<string> GetRemarks(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Remarks));

        internal static IReadOnlyCollection<string> GetReturns(this IMethodSymbol value) => GetReturns(value.GetDocumentationCommentXml());

        internal static IReadOnlyCollection<string> GetReturns(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Returns));

        internal static IReadOnlyCollection<string> GetValue(this IMethodSymbol value) => GetValue(value.GetDocumentationCommentXml());

        internal static IReadOnlyCollection<string> GetValue(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Value));

        internal static IReadOnlyCollection<string> GetExamples(string commentXml) => Cleaned(GetComments(commentXml, Constants.XmlTag.Example));

        internal static XElement GetCommentElement(this ISymbol value) => GetCommentElement(value.GetDocumentationCommentXml());

        internal static XElement GetCommentElement(this string value)
        {
            // just to be sure that we always have a root element (malformed XMLs are reported as comment but without a root element)
            var xml = "<root>".ConcatenatedWith(value.AsSpan().Trim(), "</root>");

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

        internal static IEnumerable<XElement> GetCommentElements(this XElement value, string xmlTag) => value is null
                                                                                                        ? Enumerable.Empty<XElement>() // happens in case of an invalid character
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

        internal static IEnumerable<string> GetExceptionsOfExceptionComments(string commentXml)
        {
            return GetExceptionCommentElements(commentXml).Select(_ => _.Attribute(Constants.XmlTag.Attribute.Cref)?.Value).WhereNotNull();
        }

        internal static IReadOnlyCollection<string> Cleaned(IEnumerable<string> comments)
        {
            var cleanedComments = comments.WhereNotNull().ToList();

            switch (cleanedComments.Count)
            {
                case 0: return Array.Empty<string>();
                case 1: return new[] { TrimComment(cleanedComments[0]) };
                default:
                    return cleanedComments.ToHashSet(TrimComment);
            }

            string TrimComment(string comment) => comment.AsCachedBuilder().WithoutParaTags().Trimmed().ToStringAndRelease();
        }

        internal static string Cleaned(XElement element)
        {
            if (element is null)
            {
                return null;
            }

            // remove all code elements
            var codeElements = element.Descendants(Constants.XmlTag.Code).ToList();

            if (codeElements.Count > 0)
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
                                 .ReplaceAllWithCheck(Constants.Comments.MultiWhitespaceStrings, Constants.Comments.SingleWhitespaceString)
                                 .Trim();
            }

            StringBuilderCache.Release(builder);

            return cleaned;
        }

        private static string FlattenComment(IEnumerable<XElement> comments)
        {
            if (comments.Any())
            {
                var comment = Cleaned(comments.Nodes());

                return comment;
            }

            return null;
        }
    }
}