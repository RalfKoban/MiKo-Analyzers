using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2051_CodeFixProvider)), Shared]
    public sealed class MiKo_2051_CodeFixProvider : ExceptionDocumentationCodeFixProvider
    {
        // TODO RKN: see Constants.Comments.ExceptionForbiddenStartingPhrase
        private static readonly string[] Phrases =
            {
                "Thrown if the ",
                "Thrown if ",
                "Thrown when the ",
                "Thrown when ",
                "Throws if the ",
                "Throws if ",
                "Throws when the ",
                "Throws when ",
                "Is thrown when the ",
                "Is thrown when ",
                "Gets thrown when the ",
                "Gets thrown when ",
                "If the ",
                "In case the ",
                "In case ",
                "if the ",
                "in case the ",
                "in case ",
            };

        public override string FixableDiagnosticId => MiKo_2051_ExceptionTagDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2051_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var exceptionComments = GetExceptionXmls(syntax);

            foreach (var exceptionComment in exceptionComments)
            {
                if (exceptionComment.IsExceptionCommentFor<ArgumentNullException>())
                {
                    var parameters = exceptionComment.GetParameters();
                    switch (parameters.Count)
                    {
                        case 0:
                            break; // TODO RKN: cannot fix as there seems to be no parameter

                        case 1:
                        {
                            // seems like we have only a single parameter, so place it on a single line
                            var newComment = exceptionComment.WithContent(ParameterIsNull(parameters[0]));

                            return syntax.ReplaceNode(exceptionComment, newComment);
                        }

                        default:
                        {
                            // more than 1 parameter, so pick the referenced ones
                            var comment = exceptionComment.ToString();
                            var ps = parameters.Where(_ => comment.ContainsAny(GetParameterReferences(_))).ToArray();

                            var newComment = exceptionComment.WithContent(ParameterIsNull(ps));

                            return syntax.ReplaceNode(exceptionComment, newComment);
                        }
                    }
                }

                if (exceptionComment.Content.First() is XmlTextSyntax text)
                {
                    // TODO: RKN Put this into a lookup
                    return ReplaceText(syntax, text, Phrases, string.Empty);
                }
            }

            return syntax;
        }

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
                    yield return Para("-or-");
                }
            }
        }
    }
}