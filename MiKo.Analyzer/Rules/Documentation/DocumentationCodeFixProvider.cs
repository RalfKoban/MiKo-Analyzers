using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        protected DocumentationCodeFixProvider() : base(false)
        {
        }

        protected static IEnumerable<XmlElementSyntax> GetXmlSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNodes.SelectMany(_ => _.DescendantNodes(__ => true, true)).OfType<XmlElementSyntax>();
        }
    }
}