using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2224_CodeFixProvider)), Shared]
    public sealed class MiKo_2224_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer.Id;

        protected override string Title => Resources.MiKo_2224_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case XmlElementSyntax _:
                    case XmlEmptyElementSyntax _:
                        return node;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax element)
            {
                if (issue.Location == element.StartTag.GetLocation())
                {
                    return GetUpdatedSyntax(element.StartTag);
                }

                if (issue.Location == element.EndTag.GetLocation())
                {
                    return GetUpdatedSyntax(element.EndTag);
                }
            }

            if (syntax is XmlEmptyElementSyntax)
            {
                return syntax;
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is XmlEmptyElementSyntax element)
            {
                var nodes = new List<SyntaxNode> { syntax };

                if (issue.Properties.ContainsKey(MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer.AddSpaceBefore))
                {
                    nodes.Insert(0, NewLineXmlText());
                }

                if (issue.Properties.ContainsKey(MiKo_2224_DocumentationPlacesContentsOnSeparateLineAnalyzer.AddSpaceAfter))
                {
                    nodes.Add(NewLineXmlText());
                }

                return root.ReplaceNode(element, nodes);
            }

            return base.GetUpdatedSyntaxRoot(document, root, syntax, annotationOfSyntax, issue);
        }

        private static SyntaxNode GetUpdatedSyntax(XmlElementStartTagSyntax syntax)
        {
            if (syntax.Parent is XmlElementSyntax element)
            {
                return element.WithContent(element.Content.Insert(0, NewLineXmlText()));
            }

            return syntax;
        }

        private static SyntaxNode GetUpdatedSyntax(XmlElementEndTagSyntax syntax)
        {
            if (syntax.Parent is XmlElementSyntax element)
            {
                return element.WithContent(element.Content.Add(NewLineXmlText()));
            }

            return syntax;
        }
    }
}