using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
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

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is DocumentationCommentTriviaSyntax d)
            {
                return Comment(context, d, issue);
            }

            var parameterCommentSyntax = (XmlElementSyntax)syntax;
            var parameterName = GetParameterName(parameterCommentSyntax);

            var parameters = parameterCommentSyntax.GetParameters();

            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];

                if (parameter.GetName() == parameterName)
                {
                    return Comment(context, parameterCommentSyntax, parameter, index);
                }
            }

            return parameterCommentSyntax;
        }

        protected virtual IEnumerable<SyntaxNode> FittingSyntaxNodes(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MethodDeclarationSyntax>();

        protected abstract DocumentationCommentTriviaSyntax Comment(CodeFixContext context, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic);

        protected abstract XmlElementSyntax Comment(CodeFixContext context, XmlElementSyntax comment, ParameterSyntax parameter, int index);
    }
}