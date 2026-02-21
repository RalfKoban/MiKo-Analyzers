using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        protected override Task<DocumentationCommentTriviaSyntax> GetUpdatedSyntaxAsync(DocumentationCommentTriviaSyntax syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntax(DocumentationCommentTriviaSyntax syntax)
        {
            var list = syntax.DescendantNodes<XmlTextSyntax>(_ => _.TextTokens.Any(__ => __.ValueText.AsSpan().TrimStart().StartsWithAny(Markers, StringComparison.OrdinalIgnoreCase)));

            return syntax.ReplaceNodes(list, GetReplacements);
        }

        private static IEnumerable<SyntaxNode> GetReplacements(XmlTextSyntax text)
        {
            var result = new List<SyntaxNode>();

            var listItemText = new List<SyntaxToken>();
            var normalText = new List<SyntaxToken>();

            var tokens = text.GetXmlTextTokens();

            if (tokens.Count > 0)
            {
                foreach (var token in tokens)
                {
                    var valueText = token.ValueText.AsSpan().Trim();

                    if (valueText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    var adjustedToken = token.WithText(valueText).WithLeadingXmlComment();

                    if (valueText.StartsWithAny(Markers, StringComparison.OrdinalIgnoreCase))
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
            }

            AddList(result, listItemText);
            AddXmlText(result, normalText);

            if (result.Count is 0)
            {
                // nothing to replace, so use original code
                result.Add(text);
            }

            result[result.Count - 1] = result[result.Count - 1].WithTrailingXmlComment();

            return result;
        }

        private static void AddXmlText(List<SyntaxNode> result, List<SyntaxToken> text)
        {
            if (text.Any(_ => _.ValueText.IsNullOrWhiteSpace() is false))
            {
                result.Add(XmlText(text));
            }
        }

        private static void AddList(List<SyntaxNode> result, List<SyntaxToken> listItems)
        {
            if (listItems.Count > 0)
            {
                result.Add(GetAsList(listItems));
            }
        }

        private static XmlElementSyntax GetAsList(List<SyntaxToken> listItems)
        {
            var items = new List<XmlElementSyntax>();

            foreach (var listItem in listItems)
            {
                var comment = XmlText(listItem.ValueText.AsSpan().WithoutFirstWord().Trim().ToString());
                var description = XmlElement(Constants.XmlTag.Description, comment);
                var item = XmlElement(Constants.XmlTag.Item, description);

                items.Add(item);
            }

            return XmlList(Constants.XmlTag.ListType.Bullet, items).WithLeadingXmlComment();
        }
    }
}