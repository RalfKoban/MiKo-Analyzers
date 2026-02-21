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
        /// <summary>
        /// The XML comment exterior trivia with a trailing space, used for formatting XML documentation comments.
        /// </summary>
        internal static readonly SyntaxTrivia XmlCommentExterior = SyntaxFactory.DocumentationCommentExterior(Constants.Comments.XmlCommentExterior + " ");

        /// <summary>
        /// Contains the standard XML comment start sequence consisting of an elastic carriage return line feed followed by the XML comment exterior trivia.
        /// </summary>
        internal static readonly SyntaxTrivia[] XmlCommentStart =
                                                                  {
                                                                      SyntaxFactory.ElasticCarriageReturnLineFeed, // use elastic one to allow formatting to be done automatically
                                                                      XmlCommentExterior,
                                                                  };

        /// <summary>
        /// Contains the boolean <see cref="string"/> representations in various casings used for detecting boolean values in XML documentation.
        /// </summary>
        private static readonly string[] Booleans = { "true", "false", "True", "False", "TRUE", "FALSE" };

        /// <summary>
        /// Contains the <see cref="string"/> representations for <see langword="null"/> in various casings used for detecting <see langword="null"/> values in XML documentation.
        /// </summary>
        private static readonly string[] Nulls = { "null", "Null", "NULL" };

        /// <summary>
        /// Finds the first syntax node in the hierarchy that has leading comment trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax node to start searching from.
        /// </param>
        /// <returns>
        /// The first <see cref="SyntaxNode"/> that has leading comment trivia, or <see langword="null"/> if no such node is found.
        /// </returns>
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
                var count = list.Count;

                if (count > 0)
                {
                    for (var index = 0; index < count; index++)
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

        /// <summary>
        /// Gets all comment trivia from both leading and trailing trivia of a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the comment trivia from.
        /// </param>
        /// <returns>
        /// An array of <see cref="SyntaxTrivia"/> that represents all comment trivia found in the leading and trailing trivia of the syntax node.
        /// </returns>
        internal static SyntaxTrivia[] GetComment(this SyntaxNode value) => value.GetLeadingTrivia().Concat(value.GetTrailingTrivia()).Where(_ => _.IsComment()).ToArray();

        /// <summary>
        /// Gets the location of the contents within an XML element, excluding whitespace-only text at the beginning and end.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to get the contents location from.
        /// </param>
        /// <returns>
        /// A <see cref="Location"/> that represents the span of the meaningful contents within the XML element, or the span between the start and end tags if no content is found.
        /// </returns>
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
                for (int index = 0, count = list.Count; index < count; index++)
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

        /// <summary>
        /// Gets the cref attribute from an XML element or empty element syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the cref attribute from.
        /// </param>
        /// <returns>
        /// The <see cref="XmlCrefAttributeSyntax"/> representing the cref attribute, or <see langword="null"/> if no cref attribute is found or the syntax node is not an XML element.
        /// </returns>
        internal static XmlCrefAttributeSyntax GetCref(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax e: return e.Attributes.GetCref();
                case XmlElementSyntax e: return e.StartTag.Attributes.GetCref();
                default: return null;
            }
        }

        /// <summary>
        /// Gets the cref attribute from an XML element or empty element syntax node with the specified name.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the cref attribute from.
        /// </param>
        /// <param name="name">
        /// The name of the XML element to match.
        /// </param>
        /// <returns>
        /// The <see cref="XmlCrefAttributeSyntax"/> representing the cref attribute, or <see langword="null"/> if no cref attribute is found or the syntax node does not match the specified name.
        /// </returns>
        internal static XmlCrefAttributeSyntax GetCref(this SyntaxNode value, string name)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax e when e.GetName() == name: return e.Attributes.GetCref();
                case XmlElementSyntax e when e.GetName() == name: return e.StartTag.Attributes.GetCref();
                default: return null;
            }
        }

        /// <summary>
        /// Gets the type syntax from an XML cref attribute.
        /// </summary>
        /// <param name="value">
        /// The XML cref attribute to get the type from.
        /// </param>
        /// <returns>
        /// The <see cref="TypeSyntax"/> representing the type referenced by the cref attribute, or <see langword="null"/> if no type is found.
        /// </returns>
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

        /// <summary>
        /// Gets the documentation comment trivia syntax from a syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the documentation comment trivia syntax from.
        /// </param>
        /// <param name="kind">
        /// One of the enumeration members that specifies the kind of documentation comment trivia syntax to get.
        /// The default is <see cref="SyntaxKind.SingleLineDocumentationCommentTrivia"/>.
        /// </param>
        /// <returns>
        /// An array of <see cref="DocumentationCommentTriviaSyntax"/> that represents the documentation comment trivia syntax, or an empty array if none is found.
        /// </returns>
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

        /// <summary>
        /// Gets only those XML elements that are empty (have NO content) and match the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> representing empty XML elements (with no content) that match the given tag.
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
        /// Gets only those XML elements that are empty (have NO content) and match the given tag out of the list of syntax nodes.
        /// </summary>
        /// <param name="value">
        /// The starting point of the XML elements to consider.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> representing empty XML elements (with no content) that match the given tag.
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

        /// <summary>
        /// Gets all example XML elements from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the example XML elements from.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that represents the example XML elements.
        /// </returns>
        internal static IReadOnlyList<XmlElementSyntax> GetExampleXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Example);

        /// <summary>
        /// Gets all exception XML elements from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the exception XML elements from.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that represents the exception XML elements.
        /// </returns>
        internal static IReadOnlyList<XmlElementSyntax> GetExceptionXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Exception);

        /// <summary>
        /// Gets all leading comment trivia from a syntax node as text strings.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the leading comments from.
        /// </param>
        /// <returns>
        /// An array of strings that represents the text content of all leading comment trivia, excluding empty comments.
        /// </returns>
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

        /// <summary>
        /// Gets the type attribute from a list XML element.
        /// </summary>
        /// <param name="value">
        /// The XML element to get the type attribute from.
        /// </param>
        /// <returns>
        /// The <see cref="XmlTextAttributeSyntax"/> representing the type attribute, or <see langword="null"/> if no type attribute is found.
        /// </returns>
        internal static XmlTextAttributeSyntax GetListType(this XmlElementSyntax value) => value.GetAttributes<XmlTextAttributeSyntax>()
                                                                                                .FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Type);

        /// <summary>
        /// Gets the type value from an XML text attribute without trivia.
        /// </summary>
        /// <param name="value">
        /// The XML text attribute to get the type value from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the type value without trivia.
        /// </returns>
        internal static string GetListType(this XmlTextAttributeSyntax value) => value.GetTextWithoutTrivia();

        /// <summary>
        /// Gets the name attribute from an XML element or empty element syntax node.
        /// </summary>
        /// <param name="value">
        /// The syntax node to get the name attribute from.
        /// </param>
        /// <returns>
        /// The <see cref="XmlTextAttributeSyntax"/> that represents the name attribute, or <see langword="null"/> if the syntax node is not an XML element or has no name attribute.
        /// </returns>
        internal static XmlTextAttributeSyntax GetNameAttribute(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlElementSyntax e: return e.GetAttributes<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Name);
                case XmlEmptyElementSyntax ee: return ee.Attributes.OfType<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Name);
                default: return null;
            }
        }

        /// <summary>
        /// Gets the parameters from the containing member declaration of an XML element.
        /// </summary>
        /// <param name="value">
        /// The XML element to get the parameters for.
        /// </param>
        /// <returns>
        /// An array of <see cref="ParameterSyntax"/> that represents the parameters of the containing member declaration, or an empty array if no parameters are found.
        /// </returns>
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
#if VS2022
                    case ClassDeclarationSyntax c when c.ParameterList is ParameterListSyntax parameters:
                        return parameters.Parameters.ToArray();

                    case StructDeclarationSyntax s when s.ParameterList is ParameterListSyntax parameters:
                        return parameters.Parameters.ToArray();
#endif
                    case RecordDeclarationSyntax r when r.ParameterList is ParameterListSyntax parameters:
                        return parameters.Parameters.ToArray();

                    case BaseTypeDeclarationSyntax _:
                        return Array.Empty<ParameterSyntax>();
                }
            }

            return Array.Empty<ParameterSyntax>();
        }

        /// <summary>
        /// Gets all remarks XML elements from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the remarks XML elements from.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that represents the remarks XML elements.
        /// </returns>
        internal static IReadOnlyList<XmlElementSyntax> GetRemarksXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Remarks);

        /// <summary>
        /// Gets all returns XML elements from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the returns XML elements from.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that represents the returns XML elements.
        /// </returns>
        internal static IReadOnlyList<XmlElementSyntax> GetReturnsXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Returns);

        /// <summary>
        /// Gets all summary XML elements from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the summary XML elements from.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that represents the summary XML elements.
        /// </returns>
        internal static IReadOnlyList<XmlElementSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Summary);

        /// <summary>
        /// Gets all summary XML nodes matching the specified tags from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the summary XML nodes from.
        /// </param>
        /// <param name="tags">
        /// The set of tags to match within summary XML elements.
        /// </param>
        /// <returns>
        /// A sequence that contains <see cref="XmlNodeSyntax"/> that represents the summary XML nodes matching the specified tags.
        /// </returns>
        internal static IEnumerable<XmlNodeSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value, ISet<string> tags)
        {
            var summaryXmls = value.GetSummaryXmls();

            if (summaryXmls.Count is 0)
            {
                return Array.Empty<XmlNodeSyntax>();
            }

            return GetSummaryXmlsCore(summaryXmls, tags);
        }

        /// <summary>
        /// Gets the text content of an XML element with trimmed whitespace, normalized line breaks, collapsed multiple spaces, and without paragraph tags.
        /// </summary>
        /// <param name="value">
        /// The XML element to get the trimmed text from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text content of the XML element with trimmed whitespace, or the <see cref="string.Empty"/> string ("") if the element is <see langword="null"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the text content of an XML text syntax with trimmed whitespace, normalized line breaks, collapsed multiple spaces, and without paragraph tags.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to get the trimmed text from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text content of the XML text syntax with trimmed whitespace, or the <see cref="string.Empty"/> string ("") if the syntax is <see langword="null"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the text content of an XML text attribute without syntax trivia (whitespace, comments, and formatting tokens).
        /// </summary>
        /// <param name="value">
        /// The XML text attribute to get the text from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text content without trivia, or <see langword="null"/> if the attribute is <see langword="null"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the text content of an XML text syntax without syntax trivia (whitespace, comments, and formatting tokens).
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to get the text from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text content without trivia, or <see langword="null"/> if the syntax is <see langword="null"/>.
        /// </returns>
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

        /// <summary>
        /// Gets the text content of an XML text syntax without syntax trivia (whitespace, comments, and formatting tokens) and appends it to a <see cref="StringBuilder"/> .
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to get the text from.
        /// </param>
        /// <param name="builder">
        /// The <see cref="StringBuilder"/>  to append the text to.
        /// </param>
        /// <returns>
        /// The same <see cref="StringBuilder"/>  with the appended text content.
        /// </returns>
        internal static StringBuilder GetTextWithoutTrivia(this XmlTextSyntax value, StringBuilder builder)
        {
            if (value is null)
            {
                return builder;
            }

            return value.TextTokens.GetTextWithoutTrivia(builder);
        }

        /// <summary>
        /// Gets the text content of an XML element without syntax trivia (whitespace, comments, and formatting tokens) and appends it to a <see cref="StringBuilder"/> .
        /// </summary>
        /// <param name="value">
        /// The XML element to get the text from.
        /// </param>
        /// <param name="builder">
        /// The <see cref="StringBuilder"/>  to append the text to.
        /// </param>
        /// <returns>
        /// The same <see cref="StringBuilder"/>  with the appended text content.
        /// </returns>
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

        /// <summary>
        /// Gets the text content of an XML text syntax lazily without syntax trivia (whitespace, comments, and formatting tokens) using deferred execution.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to get the text from.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="string"/>s that represent the text content without trivia.
        /// </returns>
        /// <remarks>
        /// Use this method when you may not need to process all text tokens, as it yields results one at a time.
        /// </remarks>
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

        /// <summary>
        /// Gets all value XML elements from documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to get the value XML elements from.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that represents the value XML elements.
        /// </returns>
        internal static IReadOnlyList<XmlElementSyntax> GetValueXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Value);

        /// <summary>
        /// Gets only those XML elements that are NOT empty (have some content) and match the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> representing non-empty XML elements that match the given tag.
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
        /// Gets only those XML elements that are NOT empty (have some content) and match the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
        /// The documentation syntax.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> representing non-empty XML elements that match the given tag.
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
        /// Determines whether a syntax node has comment trivia in either leading or trailing trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the comment trivia.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has comment trivia in leading or trailing trivia; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasComment(this SyntaxNode value) => value.HasLeadingComment() || value.HasTrailingComment();

        /// <summary>
        /// Determines whether a syntax node has documentation comment trivia syntax.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the documentation comment trivia.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has documentation comment trivia syntax; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasDocumentationCommentTriviaSyntax(this SyntaxNode value)
        {
            var token = value.FindStructuredTriviaToken();

            return token.HasStructuredTrivia && token.HasDocumentationCommentTriviaSyntax();
        }

        /// <summary>
        /// Determines whether a syntax node has leading comment trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the leading comment trivia.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has leading comment trivia; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasLeadingComment(this SyntaxNode value) => value.GetLeadingTrivia().HasComment();

        /// <summary>
        /// Determines whether a syntax node has trailing comment trivia.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the trailing comment trivia.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node has trailing comment trivia; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool HasTrailingComment(this SyntaxNode value) => value != null && value.GetTrailingTrivia().HasComment();

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;b&gt;true&lt;/b&gt;</c> or <c>&lt;b&gt;false&lt;/b&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;b&gt;true&lt;/b&gt;</c> or <c>&lt;b&gt;false&lt;/b&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;b&gt;true&lt;/b&gt;</c> or <c>&lt;b&gt;false&lt;/b&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsBBool(this SyntaxNode value) => value.Is("b", Booleans);

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;b&gt;null&lt;/b&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;b&gt;null&lt;/b&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;b&gt;null&lt;/b&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsBNull(this SyntaxNode value) => value.Is("b", Nulls);

        /// <summary>
        /// Determines whether a syntax node represents a boolean tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the boolean tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a boolean tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsBooleanTag(this SyntaxNode value) => value.IsSeeLangwordBool() || value.IsWrongBooleanTag();

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;c&gt;…&lt;/c&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;c&gt;…&lt;/c&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;c&gt;…&lt;/c&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsC(this SyntaxNode value) => value is XmlElementSyntax xes && xes.IsC();

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;c&gt;…&lt;/c&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;c&gt;…&lt;/c&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;c&gt;…&lt;/c&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsC(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.C;

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;c&gt;true&lt;/c&gt;</c> or <c>&lt;c&gt;false&lt;/c&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;c&gt;true&lt;/c&gt;</c> or <c>&lt;c&gt;false&lt;/c&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;c&gt;true&lt;/c&gt;</c> or <c>&lt;c&gt;false&lt;/c&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCBool(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Booleans);

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;c&gt;null&lt;/c&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;c&gt;null&lt;/c&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;c&gt;null&lt;/c&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCNull(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Nulls);

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;code&gt;…&lt;/code&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;code&gt;…&lt;/code&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;code&gt;…&lt;/code&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCode(this SyntaxNode value) => value is XmlElementSyntax xes && xes.IsCode();

        /// <summary>
        /// Determines whether an XML element syntax represents a <c>&lt;code&gt;…&lt;/code&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to check for <c>&lt;code&gt;…&lt;/code&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML element syntax represents a <c>&lt;code&gt;…&lt;/code&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCode(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.Code;

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;code&gt;true&lt;/code&gt;</c> or <c>&lt;code&gt;false&lt;/code&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;code&gt;true&lt;/code&gt;</c> or <c>&lt;code&gt;false&lt;/code&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;code&gt;true&lt;/code&gt;</c> or <c>&lt;code&gt;false&lt;/code&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCodeBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Code, Booleans);

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;code&gt;null&lt;/code&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;code&gt;null&lt;/code&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;code&gt;null&lt;/code&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsCodeNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Code, Nulls);

        /// <summary>
        /// Determines whether an XML element syntax represents an <c>&lt;exception… /&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to check for <c>&lt;exception… /&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML element syntax represents an <c>&lt;exception… /&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsException(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.Exception;

        /// <summary>
        /// Determines whether an XML element syntax represents an exception comment for the specified exception type.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to check for exception comment.
        /// </param>
        /// <param name="exceptionType">
        /// The exception type to check for.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML element syntax represents an exception comment for the specified exception type; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether an XML element syntax represents an exception comment for the specified exception type.
        /// </summary>
        /// <typeparam name="T">
        /// The exception type to check for.
        /// </typeparam>
        /// <param name="value">
        /// The XML element syntax to check for exception comment.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML element syntax represents an exception comment for the specified exception type; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsExceptionCommentFor<T>(this XmlElementSyntax value) where T : Exception => IsExceptionComment(value, typeof(T));

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;para/&gt;</c> XML tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the paragraph tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;para/&gt;</c> XML tag ; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsPara(this SyntaxNode value) => value.IsXmlTag(Constants.XmlTag.Para);

        /// <summary>
        /// Determines whether an XML empty element syntax represents a <c>&lt;see… /&gt;</c> tag with the specified attribute names.
        /// </summary>
        /// <param name="value">
        /// The XML empty element syntax to check for <c>&lt;see… /&gt;</c> tag.
        /// </param>
        /// <param name="attributeNames">
        /// The set of attribute names to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML empty element syntax represents a <c>&lt;see… /&gt;</c> tag with the specified attribute names; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSee(this XmlEmptyElementSyntax value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.See, attributeNames);

        /// <summary>
        /// Determines whether an XML element syntax represents a <c>&lt;see… /&gt;</c> tag with the specified attribute names.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to check for <c>&lt;see… /&gt;</c> tag.
        /// </param>
        /// <param name="attributeNames">
        /// The set of attribute names to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML element syntax represents a <c>&lt;see… /&gt;</c> tag with the specified attribute names; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSee(this XmlElementSyntax value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.See, attributeNames);

        /// <summary>
        /// Determines whether an XML empty element syntax represents a <c>&lt;seealso… /&gt;</c> tag with the specified attribute names.
        /// </summary>
        /// <param name="value">
        /// The XML empty element syntax to check for <c>&lt;seealso… /&gt;</c> tag.
        /// </param>
        /// <param name="attributeNames">
        /// The set of attribute names to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML empty element syntax represents a <c>&lt;seealso… /&gt;</c> tag with the specified attribute names; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeAlso(this XmlEmptyElementSyntax value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        /// <summary>
        /// Determines whether an XML element syntax represents a <c>&lt;seealso… /&gt;</c> tag with the specified attribute names.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to check for <c>&lt;seealso… /&gt;</c> tag.
        /// </param>
        /// <param name="attributeNames">
        /// The set of attribute names to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML element syntax represents a <c>&lt;seealso… /&gt;</c> tag with the specified attribute names; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsSeeAlso(this XmlElementSyntax value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;see langword… /&gt;</c> or an (invalid) <c>&lt;see langref… /&gt;</c> XML tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;see langword… /&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;see langword… /&gt;</c> or an (invalid) <c>&lt;see langref… /&gt;</c> XML tag; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;see langword… /&gt;</c> XML tag with a boolean value.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;see langword… /&gt;</c> boolean tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;see langword… /&gt;</c> XML tag with a boolean value; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;value&gt;true&lt;/value&gt;</c> or <c>&lt;value&gt;false&lt;/value&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the value boolean tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;value&gt;true&lt;/value&gt;</c> or <c>&lt;value&gt;false&lt;/value&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsValueBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Booleans);

        /// <summary>
        /// Determines whether a syntax node represents a <c>&lt;value&gt;null&lt;/value&gt;</c> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the <c>&lt;value&gt;null&lt;/value&gt;</c> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a <c>&lt;value&gt;null&lt;/value&gt;</c> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsValueNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Nulls);

        /// <summary>
        /// Determines whether a syntax node represents XML text that contains only whitespace characters.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the whitespace-only XML text.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents XML text that contains only whitespace characters; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsWhiteSpaceOnlyText(this SyntaxNode value) => value is XmlTextSyntax text && text.IsWhiteSpaceOnlyText();

        /// <summary>
        /// Determines whether an XML text syntax contains only whitespace characters.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to check for whitespace-only content.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the XML text syntax contains only whitespace characters; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Determines whether a syntax node represents a wrong boolean tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the wrong boolean tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a wrong boolean tag; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Incorrect tags include <c>&lt;c&gt;</c>, <c>&lt;b&gt;</c>, <c>&lt;value&gt;</c> or <c>&lt;code&gt;</c> containing boolean values.
        /// The correct format is <c>&lt;see langword="true"/&gt;</c> or <c>&lt;see langword="false"/&gt;</c>.
        /// </remarks>
        internal static bool IsWrongBooleanTag(this SyntaxNode value) => value.IsCBool() || value.IsBBool() || value.IsValueBool() || value.IsCodeBool();

        /// <summary>
        /// Determines whether a syntax node represents a wrong <see langword="null"/> tag.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the wrong <see langword="null"/> tag.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents a wrong <see langword="null"/> tag; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsWrongNullTag(this SyntaxNode value) => value.IsCNull() || value.IsBNull() || value.IsValueNull() || value.IsCodeNull();

        /// <summary>
        /// Determines whether a syntax node represents an XML element or empty element.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for XML representation.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents an XML element or empty element; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsXml(this SyntaxNode value)
        {
            switch (value.Kind())
            {
                case SyntaxKind.XmlElement:
                case SyntaxKind.XmlEmptyElement:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether a syntax node represents an XML element or empty element with the specified tag name.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check for the XML tag.
        /// </param>
        /// <param name="tagName">
        /// The name of the XML tag to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node represents an XML element or empty element with the specified tag name; otherwise, <see langword="false"/>.
        /// </returns>
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

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax with the first occurrence of a specified phrase replaced with a replacement <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to replace text in.
        /// </param>
        /// <param name="phrase">
        /// The phrase to search for and replace.
        /// </param>
        /// <param name="replacement">
        /// The <see cref="string"/> to replace the first occurrence of the phrase with.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the first occurrence of the phrase replaced with the replacement <see cref="string"/>, or the original syntax if no replacement was made.
        /// </returns>
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

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax with all occurrences of a specified phrase replaced with a replacement <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to replace text in.
        /// </param>
        /// <param name="phrase">
        /// The phrase to search for and replace.
        /// </param>
        /// <param name="replacement">
        /// The <see cref="string"/> to replace all occurrences of the phrase with.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with all occurrences of the phrase replaced with the replacement <see cref="string"/>, or the original syntax if no replacements were made.
        /// </returns>
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

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax with all occurrences of multiple specified phrases replaced with a replacement <see cref="string"/>.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to replace text in.
        /// </param>
        /// <param name="phrases">
        /// The collection of phrases to search for and replace.
        /// </param>
        /// <param name="replacement">
        /// The <see cref="string"/> to replace all occurrences of any phrase with.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with all occurrences of any phrase replaced with the replacement <see cref="string"/>, or the original syntax if no replacements were made.
        /// </returns>
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

        /// <summary>
        /// Creates a new XML node from the specified XML node syntax with an additional XML attribute.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the XML node syntax, which must be derived from <see cref="XmlNodeSyntax"/>.
        /// </typeparam>
        /// <param name="value">
        /// The XML node syntax to add the attribute to.
        /// </param>
        /// <param name="attribute">
        /// The XML attribute to add to the node.
        /// </param>
        /// <returns>
        /// A new XML node syntax with the attribute added, or the original syntax if the node type is not supported.
        /// </returns>
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

        /// <summary>
        /// Creates a new documentation comment trivia syntax with the specified XML node contents.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to add contents to.
        /// </param>
        /// <param name="contents">
        /// The collection of XML nodes to set as contents.
        /// </param>
        /// <returns>
        /// A new <see cref="DocumentationCommentTriviaSyntax"/> with the specified contents.
        /// </returns>
        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(contents.ToSyntaxList());

        /// <summary>
        /// Creates a new XML element syntax with the specified XML node contents.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to add contents to.
        /// </param>
        /// <param name="contents">
        /// The collection of XML nodes to set as contents.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlElementSyntax"/> with the specified contents.
        /// </returns>
        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(contents.ToSyntaxList());

        /// <summary>
        /// Creates a new documentation comment trivia syntax with the specified XML node contents.
        /// </summary>
        /// <param name="value">
        /// The documentation comment trivia syntax to add contents to.
        /// </param>
        /// <param name="contents">
        /// The array of XML nodes to set as contents.
        /// </param>
        /// <returns>
        /// A new <see cref="DocumentationCommentTriviaSyntax"/> with the specified contents.
        /// </returns>
        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(contents.ToSyntaxList());

        /// <summary>
        /// Creates a new XML element syntax with the specified XML node contents.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to add contents to.
        /// </param>
        /// <param name="contents">
        /// The array of XML nodes to set as contents.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlElementSyntax"/> with the specified contents.
        /// </returns>
        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(contents.ToSyntaxList());

        /// <summary>
        /// Creates a new syntax node from the specified syntax node with leading XML comment trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node, which must be derived from <see cref="SyntaxNode"/>.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add leading XML comment trivia to.
        /// </param>
        /// <returns>
        /// A new syntax node with leading XML comment trivia added.
        /// </returns>
        internal static T WithLeadingXmlComment<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentStart);

        /// <summary>
        /// Creates a new syntax node from the specified syntax node with leading XML comment exterior trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node, which must be derived from <see cref="SyntaxNode"/>.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add leading XML comment exterior trivia to.
        /// </param>
        /// <returns>
        /// A new syntax node with leading XML comment exterior trivia added.
        /// </returns>
        internal static T WithLeadingXmlCommentExterior<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentExterior);

        /// <summary>
        /// Creates a new XML element from the specified XML element without the first XML newline token in its content.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to remove the first newline from.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlElementSyntax"/> with the first XML newline token removed from its content.
        /// </returns>
        internal static XmlElementSyntax WithoutFirstXmlNewLine(this XmlElementSyntax value)
        {
            return value.WithContent(value.Content.WithoutFirstXmlNewLine());
        }

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without the first XML newline token and leading trivia.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove the first newline from.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the first XML newline token and leading trivia removed.
        /// </returns>
        internal static XmlTextSyntax WithoutFirstXmlNewLine(this XmlTextSyntax value)
        {
            return value.WithTextTokens(value.TextTokens.WithoutFirstXmlNewLine()).WithoutLeadingTrivia();
        }

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without the last XML newline token.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove the last newline from.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the last XML newline token removed.
        /// </returns>
        internal static XmlTextSyntax WithoutLastXmlNewLine(this XmlTextSyntax value)
        {
            var textTokens = value.TextTokens.WithoutLastXmlNewLine();

            return value.WithTextTokens(textTokens);
        }

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without the first XML newline token and with leading whitespace trimmed from the first text token.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove the leading XML comment from.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the first XML newline token removed and leading whitespace trimmed from the first text token, or the original syntax if no modifications were made.
        /// </returns>
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

        /// <summary>
        /// Creates a new syntax list from the specified XML element's content without the specified starting texts.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to remove starting texts from.
        /// </param>
        /// <param name="startTexts">
        /// The collection of texts to check for and remove from the beginning of the XML element's content.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlNodeSyntax"/> with the first matching starting text removed, or the original syntax list if none of the texts were found at the beginning.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this XmlElementSyntax value, in ReadOnlySpan<string> startTexts) => value.Content.WithoutStartText(startTexts);

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without the specified starting texts.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove starting texts from.
        /// </param>
        /// <param name="startTexts">
        /// The collection of texts to check for and remove from the beginning of the XML text syntax.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the first matching starting text removed (after trimming leading whitespace), or the original syntax if none of the texts were found at the beginning.
        /// </returns>
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

            for (int i = 0, count = textTokens.Count; i < count; i++)
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

        /// <summary>
        /// Creates a new syntax list from the specified XML element's content without all occurrences of the specified text.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to remove text from.
        /// </param>
        /// <param name="text">
        /// The text to remove from the XML element's content.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlNodeSyntax"/> with all occurrences of the specified text removed.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, string text) => value.Content.WithoutText(text);

        /// <summary>
        /// Creates a new syntax list from the specified XML element's content without all occurrences of the specified texts.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to remove texts from.
        /// </param>
        /// <param name="texts">
        /// The collection of texts to remove from the XML element's content.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlNodeSyntax"/> with all occurrences of the specified texts removed.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, in ReadOnlySpan<string> texts) => value.Content.WithoutText(texts);

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without the specified trailing text.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove trailing text from.
        /// </param>
        /// <param name="text">
        /// The text to remove from the end of the XML text syntax.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the trailing text removed, or the original syntax if the text was not found at the end.
        /// </returns>
        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, string text) => value.WithoutTrailing(new[] { text });

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without any of the specified trailing texts.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove trailing text from.
        /// </param>
        /// <param name="texts">
        /// The collection of texts to check for and remove from the end of the XML text syntax.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the first matching trailing text removed (case-insensitive), or the original syntax if none of the texts were found at the end.
        /// </returns>
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

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without the specified trailing characters.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove trailing characters from.
        /// </param>
        /// <param name="characters">
        /// The collection of characters to trim from the end of the XML text syntax.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the trailing characters removed from the last non-whitespace text token, or the original syntax if no modifications were made.
        /// </returns>
        /// <remarks>
        /// This method processes tokens in reverse order and stops after modifying the first non-whitespace, non-newline token found.
        /// XML newline tokens are skipped during processing.
        /// </remarks>
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

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax without trailing XML comment trivia.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to remove trailing XML comment trivia from.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with trailing XML comment trivia removed.
        /// </returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>If the syntax has no tokens, the original syntax is returned.</description></item>
        /// <item><description>If the syntax has a single token with trailing trivia, returns a new syntax with the trailing trivia removed.</description></item>
        /// <item><description>If the syntax has multiple tokens, returns a new syntax with the last XML newline token removed.</description></item>
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// Creates a new XML element from the specified XML element without whitespace-only XML text nodes in its content.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to remove whitespace-only comments from.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlElementSyntax"/> with whitespace-only text content removed or replaced.
        /// </returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>
        /// If the element contains a single text node or uses the second-to-last text node (for multiple text nodes),
        /// the method removes leading XML comment markers and evaluates whether the remaining content is empty or whitespace-only.
        /// </description></item>
        /// <item><description>
        /// When the processed text has no tokens or only whitespace, the text node is removed from the element;
        /// otherwise, the text node is replaced with the processed version.
        /// </description></item>
        /// <item><description>
        /// If no text nodes exist, the original element is returned unchanged.
        /// </description></item>
        /// </list>
        /// </remarks>
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

        /// <summary>
        /// Removes all XML comment exterior markers from a <see cref="StringBuilder"/> 's content.
        /// </summary>
        /// <param name="value">
        /// The <see cref="StringBuilder"/>  to remove XML comment exterior markers from.
        /// </param>
        /// <returns>
        /// The <see cref="StringBuilder"/>  with all XML comment exterior markers removed.
        /// </returns>
        internal static StringBuilder WithoutXmlCommentExterior(this StringBuilder value) => value.Without(Constants.Comments.XmlCommentExterior);

        /// <summary>
        /// Removes all XML comment exterior markers from a syntax node's <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">
        /// The syntax node to remove XML comment exterior markers from.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the syntax node's content with all XML comment exterior markers and surrounding whitespace removed.
        /// </returns>
        internal static string WithoutXmlCommentExterior(this SyntaxNode value)
        {
            var builder = StringBuilderCache.Acquire();

            var trimmed = builder.Append(value).WithoutXmlCommentExterior().Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        /// <summary>
        /// Creates a new syntax list from the specified XML element's content with the starting text added or replaced and the first word adjusted according to the specified adjustment.
        /// </summary>
        /// <param name="value">
        /// The XML element syntax to add or replace starting text in.
        /// </param>
        /// <param name="startText">
        /// The text to add or replace at the beginning of the XML element's content.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the first word of the existing content.
        /// The default is <see cref="FirstWordAdjustment.StartLowerCase"/>.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlNodeSyntax"/> with the starting text added or replaced and the first word adjusted according to the specified adjustment.
        /// </returns>
        internal static SyntaxList<XmlNodeSyntax> WithStartText(this XmlElementSyntax value, string startText, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.StartLowerCase) => value.Content.WithStartText(startText, firstWordAdjustment);

        /// <summary>
        /// Creates a new XML text syntax from the specified XML text syntax with the starting text added or replaced and the first word adjusted according to the specified adjustment.
        /// </summary>
        /// <param name="value">
        /// The XML text syntax to add or replace starting text in.
        /// </param>
        /// <param name="startText">
        /// The text to add or replace at the beginning of the XML text syntax.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the first word of the existing content.
        /// The default is <see cref="FirstWordAdjustment.StartLowerCase"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="XmlTextSyntax"/> with the starting text added or replaced and the first word adjusted according to the specified adjustment, or a new syntax with only the starting text if no existing content is found.
        /// </returns>
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

        /// <summary>
        /// Creates a new syntax node from the specified syntax node with trailing XML comment trivia.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the syntax node, which must be derived from <see cref="SyntaxNode"/>.
        /// </typeparam>
        /// <param name="value">
        /// The syntax node to add trailing XML comment trivia to.
        /// </param>
        /// <returns>
        /// A new syntax node with trailing XML comment trivia added.
        /// </returns>
        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        /// <summary>
        /// Finds the first token with structured trivia in a syntax node's hierarchy.
        /// </summary>
        /// <param name="value">
        /// The syntax node to search for structured trivia token.
        /// </param>
        /// <returns>
        /// A <see cref="SyntaxToken"/> that has structured trivia, or the default token if no such token is found.
        /// </returns>
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

        /// <summary>
        /// Gets all summary XML nodes matching the specified tags from a collection of summary XML elements.
        /// </summary>
        /// <param name="summaryXmls">
        /// The read-only list of summary XML elements to search within.
        /// </param>
        /// <param name="tags">
        /// The set of tags to match within summary XML elements.
        /// </param>
        /// <returns>
        /// A sequence that contains <see cref="XmlNodeSyntax"/> that represents the XML nodes matching the specified tags found within the summary XML elements.
        /// </returns>
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

        /// <summary>
        /// Determines whether a syntax node represents an XML element with the specified tag name and content values.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="tagName">
        /// The name of the XML tag to match.
        /// </param>
        /// <param name="contents">
        /// The collection of content values to match against the XML element's content.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is an XML element with the specified tag name and its trimmed content matches any of the specified content values (case-insensitive); otherwise, <see langword="false"/>.
        /// </returns>
        private static bool Is(this SyntaxNode value, string tagName, in ReadOnlySpan<string> contents)
        {
            if (value is XmlElementSyntax syntax && string.Equals(tagName, syntax.GetName(), StringComparison.OrdinalIgnoreCase))
            {
                var content = syntax.Content.ToString().AsSpan().Trim();

                return content.EqualsAny(contents, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        /// <summary>
        /// Determines whether a syntax node represents an empty XML element with the specified tag name and attribute names.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="tagName">
        /// The name of the XML tag to match.
        /// </param>
        /// <param name="attributeNames">
        /// The collection of attribute names to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is an empty XML element with the specified tag name and its first attribute name is contained in the specified collection; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsEmpty(this SyntaxNode value, string tagName, IEnumerable<string> attributeNames)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

        /// <summary>
        /// Determines whether a syntax node represents a non-empty XML element with the specified tag name and attribute names.
        /// </summary>
        /// <param name="value">
        /// The syntax node to check.
        /// </param>
        /// <param name="tagName">
        /// The name of the XML tag to match.
        /// </param>
        /// <param name="attributeNames">
        /// The collection of attribute names to match.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the syntax node is a non-empty XML element with the specified tag name and its first attribute name is contained in the specified collection; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsNonEmpty(this SyntaxNode value, string tagName, IEnumerable<string> attributeNames)
        {
            if (value is XmlElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.StartTag.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }
    }
}