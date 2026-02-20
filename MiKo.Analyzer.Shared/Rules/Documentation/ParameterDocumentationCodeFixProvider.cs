using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
                        if (node.IsPara())
                        {
                            continue; // ignore para tags
                        }

                        return node;

                    case SyntaxKind.XmlText:
                        continue; // we are part of an XML element, so we have to loop over it

                    default:
                        return null;
                }
            }

            return null;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(document, syntax, issue);

            return Task.FromResult(updatedSyntax);
        }

        protected abstract XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue);

        private SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
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
                            var content = parameterCommentSyntax.Content;

                            if (content.Count > 1 && content[0].IsWhiteSpaceOnlyText() && content[1] is XmlElementSyntax para && para.IsPara())
                            {
                                var updatedPara = Comment(document, para, parameter, index, issue);

                                return parameterCommentSyntax.ReplaceNode(para, updatedPara);
                            }

                            return Comment(document, parameterCommentSyntax, parameter, index, issue);
                        }
                    }
                }
            }

            return syntax;
        }
    }
}