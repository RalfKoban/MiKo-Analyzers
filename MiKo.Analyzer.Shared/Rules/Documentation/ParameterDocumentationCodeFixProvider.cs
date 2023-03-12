using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
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
                    case XmlElementStartTagSyntax start: return start.Parent;
                    case XmlElementSyntax element when element.GetName() == Constants.XmlTag.Param: return element;
                    default: return null;
                }
            }

            return null;
        }

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
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

        protected abstract XmlElementSyntax Comment(CodeFixContext context, XmlElementSyntax comment, ParameterSyntax parameter, int index);
    }
}