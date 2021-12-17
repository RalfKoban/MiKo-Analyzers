using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationCodeFixProvider : OverallDocumentationCodeFixProvider
    {
        protected static XmlElementSyntax GetFixedExceptionCommentForArgumentNullException(XmlElementSyntax exceptionComment)
        {
            var parameters = exceptionComment.GetParameters();
            switch (parameters.Count)
            {
                case 0:
                    return exceptionComment; // TODO RKN: cannot fix as there seems to be no parameter

                case 1:
                {
                    // seems like we have only a single parameter, so place it on a single line
                    return exceptionComment.WithContent(ParameterIsNull(parameters[0]));
                }

                default:
                {
                    // more than 1 parameter, so pick the referenced ones
                    var comment = exceptionComment.ToString();
                    var ps = parameters.Where(_ => comment.ContainsAny(GetParameterReferences(_))).ToArray();

                    return exceptionComment.WithContent(ParameterIsNull(ps));
                }
            }
        }

        protected DocumentationCommentTriviaSyntax FixComment(Document document, SyntaxNode syntax, DocumentationCommentTriviaSyntax comment)
        {
            return comment.Content.OfType<XmlElementSyntax>()
                          .Where(_ => _.IsException())
                          .Select(e => FixExceptionComment(document, syntax, e, comment))
                          .FirstOrDefault(fix => fix != null);
        }

        protected virtual DocumentationCommentTriviaSyntax FixExceptionComment(Document document, SyntaxNode syntax, XmlElementSyntax exception, DocumentationCommentTriviaSyntax comment) => null;

        private static IEnumerable<string> GetParameterReferences(ParameterSyntax p)
        {
            var name = p.GetName();

            yield return " " + name + " ";
            yield return "\"" + name + "\"";
        }

        private static IEnumerable<XmlNodeSyntax> ParameterIsNull(params ParameterSyntax[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                yield return ParamRef(parameter).WithLeadingXmlComment();
                yield return XmlText(" is ");
                yield return SeeLangword_Null();
                yield return XmlText(".").WithTrailingXmlComment();

                if (i < parameters.Length - 1)
                {
                    yield return ParaOr();
                }
            }
        }
    }
}