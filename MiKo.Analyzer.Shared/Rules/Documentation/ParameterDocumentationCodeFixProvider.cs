using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParameterDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            foreach (var node in syntaxNodes)
            {
                switch (node)
                {
                    case XmlElementStartTagSyntax start:
                        return start.Parent;

                    case XmlElementSyntax element when element.GetName() == Constants.XmlTag.Param:
                        return element;

                    case XmlTextSyntax _:
                        continue; // we are part of an XML element, so we have to loop over it

                    default:
                        return null;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var parameterCommentSyntax = (XmlElementSyntax)syntax;
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

            return parameterCommentSyntax;
        }

        protected abstract XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index, Diagnostic issue);
    }
}