using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class SummaryDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstXmlSyntax(Constants.XmlTag.Summary);
    }
}