﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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

        protected override string Title => Resources.MiKo_2204_CodeFixTitle;

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

            var textTokens = text.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];
                var valueText = token.ValueText.AsSpan().Trim();

                if (valueText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                if (valueText.StartsWithAny(Markers))
                {
                    // we already have some text
                    AddXmlText(result, normalText);

                    normalText.Clear();

                    listItemText.Add(token.WithText(valueText).WithLeadingXmlComment());
                }
                else
                {
                    AddList(result, listItemText);

                    // we found some text
                    normalText.Add(token);

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
                if (text.Count > 0)
                {
                    result.Add(XmlText(text).WithLeadingEndOfLine());
                }
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
            var listType = SyntaxFactory.XmlTextAttribute(Constants.XmlTag.Attribute.Type, Constants.XmlTag.ListType.Bullet.AsToken());

            return list.AddStartTagAttributes(listType).WithLeadingXmlComment();
        }
    }
}