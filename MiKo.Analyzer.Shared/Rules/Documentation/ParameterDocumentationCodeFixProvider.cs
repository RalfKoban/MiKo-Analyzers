using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParameterDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node.Kind())
                {
                    case SyntaxKind.XmlElementStartTag:
                        return ((XmlElementStartTagSyntax)node).Parent;

                    case SyntaxKind.XmlElement:
                        return node;

                    case SyntaxKind.XmlText:
                        continue; // we are part of an XML element, so we have to loop over it

                    default:
                        return null;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax parameterCommentSyntax)
            {
                var parameters = parameterCommentSyntax.GetParameters();
                var parametersLength = parameters.Length;

                if (parametersLength > 0)
                {
                    var parameterName = GetParameterName(parameterCommentSyntax);

                    for (var index = 0; index < parametersLength; index++)
                    {
                        var parameter = parameters[index];

                        if (parameter.GetName() == parameterName)
                        {
                            return Comment(document, parameterCommentSyntax, parameter, index, issue);
                        }
                    }
                }
            }

            return syntax;
        }

        protected abstract XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue);
    }
}