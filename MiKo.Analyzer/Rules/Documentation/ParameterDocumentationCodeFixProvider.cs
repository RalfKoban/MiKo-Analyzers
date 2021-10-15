using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParameterDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var parameterName = syntaxNodes.OfType<ParameterSyntax>().First().GetName();

            // we are called for each parameter, so we have to find out the correct XML element
            var fittingSyntaxNodes = FittingSyntaxNodes(syntaxNodes);

            var parameterSyntax = GetXmlSyntax(Constants.XmlTag.Param, fittingSyntaxNodes).FirstOrDefault(_ => GetParameterName(_) == parameterName);
            if (parameterSyntax != null)
            {
                return parameterSyntax;
            }

            // we did not find the parameter documentation, hence we have to return the parent documentation to be able to add a value
            return GetXmlSyntax(fittingSyntaxNodes);
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            if (syntax is DocumentationCommentTriviaSyntax d)
            {
                return Comment(document, d, diagnostic);
            }

            var parameterCommentSyntax = (XmlElementSyntax)syntax;
            var parameterName = GetParameterName(parameterCommentSyntax);

            var parameters = GetParameters(parameterCommentSyntax);

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                if (parameter.GetName() == parameterName)
                {
                    return Comment(document, parameterCommentSyntax, parameter, index);
                }
            }

            return parameterCommentSyntax;
        }

        protected virtual IEnumerable<SyntaxNode> FittingSyntaxNodes(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>();

        protected virtual SeparatedSyntaxList<ParameterSyntax> GetParameters(XmlElementSyntax syntax)
        {
            var method = syntax.Ancestors().OfType<MethodDeclarationSyntax>().First();

            return method.ParameterList.Parameters;
        }

        protected abstract DocumentationCommentTriviaSyntax Comment(Document document, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic);

        protected abstract XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, int index);

        private static string GetParameterName(XmlElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>().First().Identifier.GetName();
    }
}