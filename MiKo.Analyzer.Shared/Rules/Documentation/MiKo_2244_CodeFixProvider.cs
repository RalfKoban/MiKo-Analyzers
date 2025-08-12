using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2244_CodeFixProvider)), Shared]
    public sealed class MiKo_2244_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_2244";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<XmlElementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax element)
            {
                switch (element.GetName())
                {
                    case "ul":
                    case "UL":
                        return GetUpdatedSyntaxForUnorderedList(element);

                    case "ol":
                    case "OL":
                        return GetUpdatedSyntaxForOrderedList(element);
                }
            }

            return syntax;
        }

        private static XmlElementSyntax GetUpdatedSyntaxForUnorderedList(XmlElementSyntax syntax) => GetAsList(Constants.XmlTag.ListType.Bullet, syntax);

        private static XmlElementSyntax GetUpdatedSyntaxForOrderedList(XmlElementSyntax syntax) => GetAsList(Constants.XmlTag.ListType.Number, syntax);

        private static XmlElementSyntax GetAsList(string listType, XmlElementSyntax syntax)
        {
            var items = new List<XmlElementSyntax>();

            foreach (var element in syntax.Content.OfType<XmlElementSyntax>())
            {
                // should be a <li> or a <LI>
                var description = XmlElement(Constants.XmlTag.Description, element.Content);
                var item = XmlElement(Constants.XmlTag.Item, description);

                items.Add(item.WithTriviaFrom(element));
            }

            return XmlList(listType, items).WithTriviaFrom(syntax);
        }
    }
}