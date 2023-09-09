using System.Collections.Generic;
using System.Composition;
using System.Linq;

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

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().First();

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

            return base.GetUpdatedSyntax(document, syntax, issue);
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