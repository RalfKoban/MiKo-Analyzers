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
        public override string FixableDiagnosticId => "MiKo_2224";

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
            switch (syntax)
            {
                case XmlEmptyElementSyntax _:
                    return syntax;

                case XmlElementSyntax element:
                {
                    var startTag = element.StartTag;

                    if (issue.Location == startTag.GetLocation())
                    {
                        return GetUpdatedSyntax(startTag);
                    }

                    var endTag = element.EndTag;

                    if (issue.Location == endTag.GetLocation())
                    {
                        return GetUpdatedSyntax(endTag);
                    }

                    break;
                }
            }

            return base.GetUpdatedSyntax(document, syntax, issue);
        }

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is XmlEmptyElementSyntax element)
            {
                var properties = issue.Properties;

                var nodes = new List<SyntaxNode> { syntax };

                if (properties.ContainsKey(Constants.AnalyzerCodeFixSharedData.AddSpaceBefore))
                {
                    nodes.Insert(0, NewLineXmlText());
                }

                if (properties.ContainsKey(Constants.AnalyzerCodeFixSharedData.AddSpaceAfter))
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