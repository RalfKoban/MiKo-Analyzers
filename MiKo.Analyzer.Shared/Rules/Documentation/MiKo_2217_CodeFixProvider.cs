using System;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2217_CodeFixProvider)), Shared]
    public sealed class MiKo_2217_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2217";

        protected override string Title => Resources.MiKo_2217_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            if (syntax.FindNode(diagnostic.Location.SourceSpan) is XmlElementSyntax node)
            {
                var attribute = GetListElement(node).GetListType();
                var listType = attribute.GetListType();

                switch (listType.ToLowerCase())
                {
                    case null:
                    case Constants.XmlTag.ListType.Bullet:
                    case Constants.XmlTag.ListType.Number:
                    {
                        return GetUpdatedSyntaxForBulletOrNumber(syntax, node);
                    }

                    case Constants.XmlTag.ListType.Table:
                    {
                        return GetUpdatedSyntaxForTable(syntax, node);
                    }
                }
            }

            return syntax;
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntaxForBulletOrNumber(DocumentationCommentTriviaSyntax syntax, XmlElementSyntax node)
        {
            var name = node.GetName();

            switch (name)
            {
                case Constants.XmlTag.Description:
                case Constants.XmlTag.ListHeader:
                case Constants.XmlTag.Term:
                {
                    return RemoveNode(syntax, node);
                }

                case Constants.XmlTag.Item:
                {
                    // seems like the node has either some child nodes (and we are interested in the first one only) or just contains some text (we are interested in)
                    var firstNode = node.FirstChild<XmlElementSyntax>();
                    var updatedNode = node.WithContent(SyntaxFactory.XmlElement(Constants.XmlTag.Description, firstNode?.Content ?? node.Content));

                    return ReplaceNode(syntax, node, updatedNode);
                }

                default:
                {
                    return syntax;
                }
            }
        }

        private static DocumentationCommentTriviaSyntax GetUpdatedSyntaxForTable(DocumentationCommentTriviaSyntax syntax, XmlElementSyntax node)
        {
            var name = node.GetName();

            switch (name)
            {
                case Constants.XmlTag.Description:
                {
                    // problem on parent, so get ancestor list and replace all description with term nodes
                    var list = GetListElement(node);
                    var itemsToReplace = list.DescendantNodes<XmlElementSyntax>().Where(_ => _.GetName() == Constants.XmlTag.Description);

                    return syntax.ReplaceNodes(itemsToReplace, (_, rewritten) => SyntaxFactory.XmlElement(Constants.XmlTag.Term, rewritten.Content));
                }

                case Constants.XmlTag.ListHeader:
                case Constants.XmlTag.Item:
                {
                    // it's either a <term> or a <description>, so first sub item should be a term, second a description
                    var nodes = node.ChildNodes<XmlElementSyntax>().ToList();

                    if (nodes.Count == 2)
                    {
                        return syntax.ReplaceNodes(nodes, (original, rewritten) =>
                                                                                  {
                                                                                      var newName = nodes.IndexOf(original) == 0
                                                                                                    ? Constants.XmlTag.Term
                                                                                                    : Constants.XmlTag.Description;

                                                                                      return SyntaxFactory.XmlElement(newName, rewritten.Content);
                                                                                  });
                    }

                    break;
                }
            }

            return syntax;
        }

        private static DocumentationCommentTriviaSyntax RemoveNode(DocumentationCommentTriviaSyntax syntax, SyntaxNode node) => ReplaceNode(syntax, node.Parent, ((XmlElementSyntax)node.Parent).Without(node));

        private static DocumentationCommentTriviaSyntax ReplaceNode(DocumentationCommentTriviaSyntax syntax, SyntaxNode node, XmlElementSyntax replacement) => syntax.ReplaceNode(node, replacement.WithoutWhitespaceOnlyComment());

        private static XmlElementSyntax GetListElement(SyntaxNode node) => node.FirstAncestorOrSelf<XmlElementSyntax>(_ => _.GetName() == Constants.XmlTag.List);
    }
}