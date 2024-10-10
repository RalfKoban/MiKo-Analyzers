using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2204_CodeFixProvider)), Shared]
    public sealed class MiKo_2204_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly string[] Markers =
                                                   {
                                                       "- ",
                                                       "* ",
                                                   };

        public override string FixableDiagnosticId => "MiKo_2204";

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var list = syntax.DescendantNodes<XmlTextSyntax>(_ => _.TextTokens.Any(__ => __.ValueText.AsSpan().TrimStart().StartsWithAny(Markers)));

            return syntax.ReplaceNodes(list, GetReplacements);
        }

        private static IEnumerable<SyntaxNode> GetReplacements(XmlTextSyntax text)
        {
            var result = new List<SyntaxNode>();

            var listItemText = new List<SyntaxToken>();
            var normalText = new List<SyntaxToken>();

            foreach (var token in text.GetXmlTextTokens())
            {
                var valueText = token.ValueText.AsSpan().Trim();

                if (valueText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var adjustedToken = token.WithText(valueText).WithLeadingXmlComment();

                if (valueText.StartsWithAny(Markers))
                {
                    // we already have some text
                    AddXmlText(result, normalText);

                    listItemText.Add(adjustedToken);

                    normalText.Clear();
                }
                else
                {
                    // we found some text
                    AddList(result, listItemText);

                    normalText.Add(adjustedToken);

                    listItemText.Clear();
                }
            }

            AddList(result, listItemText);
            AddXmlText(result, normalText);

            if (result.Count == 0)
            {
                // nothing to replace, so use original code
                result.Add(text);
            }

            result[result.Count - 1] = result[result.Count - 1].WithTrailingXmlComment();

            return result;
        }

        private static void AddXmlText(ICollection<SyntaxNode> result, ICollection<SyntaxToken> text)
        {
            if (text.Any(_ => _.ValueText.IsNullOrWhiteSpace() is false))
            {
                result.Add(XmlText(text));
            }
        }

        private static void AddList(ICollection<SyntaxNode> result, ICollection<SyntaxToken> listItems)
        {
            if (listItems.Count > 0)
            {
                result.Add(GetAsList(listItems));
            }
        }

        private static XmlElementSyntax GetAsList(IEnumerable<SyntaxToken> listItems)
        {
            var items = new List<XmlElementSyntax>();

            foreach (var listItem in listItems)
            {
                var comment = XmlText(listItem.ValueText.AsSpan().WithoutFirstWord().Trim().ToString());
                var description = XmlElement(Constants.XmlTag.Description, comment);
                var item = XmlElement(Constants.XmlTag.Item, description);

                items.Add(item.WithLeadingXmlComment());
            }

            var itemsCount = items.Count;

            if (itemsCount > 0)
            {
                items[itemsCount - 1] = items[itemsCount - 1].WithTrailingXmlComment();
            }

            var list = XmlElement(Constants.XmlTag.List, items);
            var listType = XmlAttribute(Constants.XmlTag.Attribute.Type, Constants.XmlTag.ListType.Bullet);

            return list.AddStartTagAttributes(listType).WithLeadingXmlComment();
        }
    }
}