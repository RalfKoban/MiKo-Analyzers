﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ParameterDocumentationCodeFixProvider : DocumentationCodeFixProvider
    {
        protected static XmlElementSyntax Comment(XmlElementSyntax comment, string[] text) => comment.WithContent(new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText(text[0])));

        protected static XmlElementSyntax CommentWithSeeCRef(XmlElementSyntax comment, string commentStart, TypeSyntax type, string commentEnd)
        {
            var content = SyntaxFactory.List<XmlNodeSyntax>()
                                       .Add(SyntaxFactory.XmlText(commentStart))
                                       .Add(Cref(Constants.XmlTag.See, type))
                                       .Add(SyntaxFactory.XmlText(commentEnd));

            return comment.WithContent(content);
        }

        protected sealed override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var parameterName = syntaxNodes.OfType<ParameterSyntax>().First().Identifier.Text;

            // we are called for each parameter, so we have to find out the correct XML element
            return GetXmlSyntax(Constants.XmlTag.Param, syntaxNodes.OfType<MethodDeclarationSyntax>()).FirstOrDefault(_ => GetParameterName(_) == parameterName);
        }

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var parameterCommentSyntax = (XmlElementSyntax)syntax;
            var parameterName = GetParameterName(parameterCommentSyntax);

            var method = parameterCommentSyntax.Ancestors().OfType<MethodDeclarationSyntax>().First();
            var parameters = method.ParameterList.Parameters;

            for (var index = 0; index < parameters.Count; index++)
            {
                var parameter = parameters[index];
                if (parameter.GetName() == parameterName)
                {
                    return Comment(parameterCommentSyntax, parameter, index);
                }
            }

            return parameterCommentSyntax;
        }

        protected abstract XmlElementSyntax Comment(XmlElementSyntax comment, ParameterSyntax parameter, int index);

        private static string GetParameterName(XmlElementSyntax syntax) => syntax.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First().Identifier.GetName();

        private static XmlEmptyElementSyntax Cref(string name, TypeSyntax type)
        {
            var attribute = SyntaxFactory.XmlCrefAttribute(SyntaxFactory.TypeCref(type.WithoutTrailingTrivia()));

            return SyntaxFactory.XmlEmptyElement(name)
                                .WithAttributes(new SyntaxList<XmlAttributeSyntax>(attribute));
        }
    }
}