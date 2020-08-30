using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExampleDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(Constants.XmlTag.Example, syntaxNodes).First();
    }
}