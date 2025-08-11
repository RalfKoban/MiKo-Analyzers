using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2230_CodeFixProvider)), Shared]
    public sealed class MiKo_2230_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2230";

        protected override string Title => Resources.MiKo_2230_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic issue)
        {
            var token = syntax.FindToken(issue);

            if (token.Parent is XmlTextSyntax node && node.Parent is XmlElementSyntax element)
            {
                var text = element.GetTextTrimmed();

                return syntax.ReplaceNode(element, element.WithContent(UpdateText(text.AsSpan())));
            }

            return syntax;
        }

        private static SyntaxList<XmlNodeSyntax> UpdateText(in ReadOnlySpan<char> text)
        {
            const string ValueMeaning = Constants.Comments.ValueMeaningPhrase;

            var index = text.IndexOf(ValueMeaning.AsSpan(), StringComparison.Ordinal);

            var xmlNodes = XmlNodes(text.Slice(0, index).Trim().ToString());
            var xmlTable = XmlTable(text.Slice(index + ValueMeaning.Length));

            return xmlNodes.Add(xmlTable.WithTrailingXmlComment());
        }

        private static SyntaxList<XmlNodeSyntax> XmlNodes(string text)
        {
            if (text.ContainsXml())
            {
                // the tag we use here is not relevant, as we are interested in the contents
                var element = ParseAsXmlElement(Constants.XmlTag.Returns, text);

                if (element != null)
                {
                    var content = element.Content;
                    var node = content[0];

                    return content.Replace(node, node.WithLeadingXmlComment());
                }
            }

            return SyntaxFactory.List<XmlNodeSyntax>().Add(XmlText(text).WithLeadingXmlComment());
        }

        private static XmlElementSyntax XmlTable(in ReadOnlySpan<char> text)
        {
            // try to find all sentences by splitting the texts at the dots (that should indicate a sentence ending)
            var nodes = new List<XmlElementSyntax>
                            {
                                XmlListHeader(Constants.Comments.ValuePhrase, Constants.Comments.MeaningPhrase),
                            };

            if (TryCreateItems(text, nodes))
            {
                return XmlList(Constants.XmlTag.ListType.Table, nodes).WithLeadingXmlComment();
            }

            return XmlElement(Constants.XmlTag.ListType.Table, XmlText(Constants.TODO));
        }

        private static bool TryCreateItems(in ReadOnlySpan<char> text, List<XmlElementSyntax> nodes)
        {
            const string LessThanZero = Constants.Comments.LessThanZero;
            const string Zero = Constants.Comments.Zero;
            const string GreaterThanZero = Constants.Comments.GreaterThanZero;

            var lessThanZeroStart = text.IndexOf(LessThanZero.AsSpan());
            var zeroStart = text.IndexOf(Zero.AsSpan());
            var greaterThanZeroStart = text.IndexOf(GreaterThanZero.AsSpan());

            if (lessThanZeroStart < 0 || zeroStart <= lessThanZeroStart || greaterThanZeroStart <= zeroStart)
            {
                return false;
            }

            var lessThanZeroEnd = lessThanZeroStart + LessThanZero.Length;
            var zeroEnd = zeroStart + Zero.Length;
            var greaterThanZeroEnd = greaterThanZeroStart + GreaterThanZero.Length;

            var lessThanZeroText = text.Slice(lessThanZeroEnd, zeroStart - lessThanZeroEnd).Trim().ToString();
            var zeroText = text.Slice(zeroEnd, greaterThanZeroStart - zeroEnd).Trim().ToString();
            var greaterThanZeroText = text.Slice(greaterThanZeroEnd).Trim().ToString();

            nodes.Add(XmlItem(LessThanZero, lessThanZeroText));
            nodes.Add(XmlItem(Zero, zeroText));
            nodes.Add(XmlItem(GreaterThanZero, greaterThanZeroText));

            return true;
        }

        private static XmlElementSyntax XmlListHeader(string term, string description) => XmlElement(Constants.XmlTag.ListHeader, new[] { XmlTerm(term), XmlDescription(description) });

        private static XmlElementSyntax XmlItem(string term, string description) => XmlElement(Constants.XmlTag.Item, new[] { XmlTerm(term), XmlDescription(description) });

        private static XmlElementSyntax XmlTerm(string term) => XmlElement(Constants.XmlTag.Term, XmlText(term));

        private static XmlElementSyntax XmlDescription(string description)
        {
            if (description.ContainsXml())
            {
                var element = ParseAsXmlElement(Constants.XmlTag.Description, description);

                if (element != null)
                {
                    return element;
                }
            }

            return XmlElement(Constants.XmlTag.Description, XmlText(description));
        }

        private static XmlElementSyntax ParseAsXmlElement(string tag, string description)
        {
            var text = StringBuilderCache.Acquire(Constants.Comments.XmlCommentExterior.Length + (2 * tag.Length) + 6 + description.Length)
                                         .Append(Constants.Comments.XmlCommentExterior)
                                         .Append(' ')
                                         .Append('<').Append(tag).Append('>')
                                         .Append(description)
                                         .Append('<').Append('/').Append(tag).Append('>')
                                         .ToStringAndRelease();

            var tree = CSharpSyntaxTree.ParseText(text);
            var documentation = tree.GetRoot().GetDocumentationCommentTriviaSyntax();

            if (documentation.Length is 1)
            {
                var elements = documentation[0].Content.OfType<XmlElementSyntax>();

                if (elements.Count > 0)
                {
                    return elements[0];
                }
            }

            return null;
        }
    }
}