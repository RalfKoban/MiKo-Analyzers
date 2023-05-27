using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationCodeFixProvider : OverallDocumentationCodeFixProvider
    {
        // TODO RKN: see Constants.Comments.ExceptionForbiddenStartingPhrase
        private static readonly string[] Phrases =
            {
                "gets thrown when the ",
                "Gets thrown when the ",
                "gets thrown when ",
                "Gets thrown when ",
                "is thrown when the ",
                "Is thrown when the ",
                "is thrown when ",
                "Is thrown when ",
                "thrown if the ",
                "Thrown if the ",
                "thrown in case that ",
                "Thrown in case that ",
                "thrown in case the ",
                "Thrown in case the ",
                "thrown in case ",
                "Thrown in case ",
                "thrown when the ",
                "Thrown when the ",
                "throws if the ",
                "Throws if the ",
                "throws in case that ",
                "Throws in case that ",
                "throws in case the ",
                "Throws in case the ",
                "throws in case ",
                "Throws in case ",
                "throws when the ",
                "Throws when the ",
                "thrown if ",
                "Thrown if ",
                "thrown when ",
                "Thrown when ",
                "throws if ",
                "Throws if ",
                "throws when ",
                "Throws when ",
                "in case the ",
                "In case the ",
                "in case ",
                "In case ",
                "if the ",
                "If the ",
                "If ",
            };

        protected static XmlElementSyntax GetFixedExceptionCommentForArgumentNullException(XmlElementSyntax exceptionComment)
        {
            var parameters = exceptionComment.GetParameterNames();

            switch (parameters.Length)
            {
                case 0:
                    return exceptionComment;

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

        protected static XmlElementSyntax GetFixedExceptionCommentForArgumentException(XmlElementSyntax exceptionComment)
        {
            var parameters = exceptionComment.GetParameterNames();

            if (parameters.Length == 0)
            {
                return exceptionComment;
            }

            var parametersAsTextReferences = parameters.SelectMany(GetParameterAsTextReference).ToArray();

            // seems we found the reference in text, so we have to split the text into 2 separate ones and place a <paramref/> between
            var textNodes = exceptionComment.DescendantNodes<XmlTextSyntax>(_ => _.GetTextWithoutTriviaLazy().Any(__ => __.ContainsAny(parametersAsTextReferences))).ToList();

            if (textNodes.Any())
            {
                // seems we found the reference in text, so we have to split the text into 2 separate ones and place a <paramref/> between
                exceptionComment = exceptionComment.ReplaceNodes(textNodes, text => ReplaceTextWithParamRefs(text, parametersAsTextReferences));
            }

            // TODO RKN: maybe we should now try to separate all <paramref/> with <para>-or-</para>
            return GetFixedStartingPhrase(exceptionComment);
        }

        protected static XmlElementSyntax GetFixedExceptionCommentForArgumentOutOfRangeException(XmlElementSyntax exceptionComment)
        {
            return GetFixedExceptionCommentForArgumentException(exceptionComment);
        }

        protected static XmlElementSyntax GetFixedStartingPhrase(XmlElementSyntax comment)
        {
            // TODO RKN: check for XML elements inside the content, such as a <paramref/> (but be aware of <para> tags)
            if (comment.Content.First() is XmlTextSyntax text)
            {
                var replaced = ReplaceText(comment, text, Phrases, string.Empty);

                if (replaced.Content.First() is XmlTextSyntax newText)
                {
                    var firstWord = newText.GetTextWithoutTriviaLazy().FirstOrDefault(_ => _.IsNullOrWhiteSpace() is false).FirstWord();

                    if (firstWord.IsNullOrWhiteSpace() is false)
                    {
                        var fixedText = newText.ReplaceFirstText(firstWord, firstWord.ToUpperCaseAt(0));

                        return replaced.ReplaceNode(newText, fixedText);
                    }
                }

                return replaced;
            }

            return comment;
        }

        protected DocumentationCommentTriviaSyntax FixComment(Document document, SyntaxNode syntax, DocumentationCommentTriviaSyntax comment)
        {
            return comment.Content.OfType<XmlElementSyntax>()
                          .Where(_ => _.IsException())
                          .Select(_ => FixExceptionComment(document, syntax, _, comment))
                          .FirstOrDefault(_ => _ != null);
        }

        protected virtual DocumentationCommentTriviaSyntax FixExceptionComment(Document document, SyntaxNode syntax, XmlElementSyntax exception, DocumentationCommentTriviaSyntax comment) => null;

        private static IEnumerable<string> GetParameterReferences(string parameterName)
        {
            yield return GetParameterAsReference(parameterName);

            foreach (var textRef in GetParameterAsTextReference(parameterName))
            {
                yield return textRef;
            }
        }

        private static IEnumerable<string> GetParameterAsTextReference(string parameterName) => Constants.TrailingSentenceMarkers.Select(_ => " " + parameterName + _);

        private static string GetParameterAsReference(string parameterName) => "\"" + parameterName + "\"";

        private static IEnumerable<XmlNodeSyntax> ParameterIsNull(params string[] parameters)
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

        private static IEnumerable<SyntaxNode> ReplaceTextWithParamRefs(XmlTextSyntax text, string[] parametersAsTextReferences)
        {
            var parts = SplitCommentsOnParametersInText(text.ToString(), parametersAsTextReferences);

            foreach (var part in parts)
            {
                if (part.ContainsAny(parametersAsTextReferences))
                {
                    var parameterName = part.Substring(1, part.Length - 2);

                    yield return ParamRef(parameterName);
                }
                else
                {
                    yield return XmlText(part);
                }
            }
        }

        private static IEnumerable<string> SplitCommentsOnParametersInText(string comment, string[] parametersAsTextReferences)
        {
            // split into parts, so that we can easily detect which part is a parameter and which is some normal text
            var parts = comment.SplitBy(parametersAsTextReferences).ToArray();

            // now correct the parameters back
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                foreach (var textRef in parametersAsTextReferences)
                {
                    if (part == textRef)
                    {
                        if (i > 0)
                        {
                            // that's text before the parameter, so add the missing character afterwards
                            parts[i - 1] = parts[i - 1] + part.First();
                        }

                        if (i < parts.Length - 1)
                        {
                            // that's text after the parameter, so add the missing character before
                            parts[i + 1] = part.Last() + parts[i + 1];
                        }
                    }
                }
            }

            return parts;
        }
    }
}