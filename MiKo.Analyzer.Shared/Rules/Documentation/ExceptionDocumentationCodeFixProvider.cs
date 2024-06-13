using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationCodeFixProvider : OverallDocumentationCodeFixProvider
    {
//// ncrunch: rdi off
        private static readonly string[] Phrases = CreatePhrases().Distinct().ToArray();
//// ncrunch: rdi default

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

        protected DocumentationCommentTriviaSyntax FixComment(SyntaxNode syntax, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic)
        {
            return comment.Content.OfType<XmlElementSyntax>()
                          .Where(_ => _.IsException())
                          .Select(_ => FixExceptionComment(syntax, _, comment, diagnostic))
                          .FirstOrDefault(_ => _ != null);
        }

        protected virtual DocumentationCommentTriviaSyntax FixExceptionComment(SyntaxNode syntax, XmlElementSyntax exception, DocumentationCommentTriviaSyntax comment, Diagnostic diagnostic) => null;

        private static IEnumerable<string> GetParameterReferences(string parameterName)
        {
            yield return GetParameterAsReference(parameterName);

            foreach (var textRef in GetParameterAsTextReference(parameterName))
            {
                yield return textRef;
            }
        }

        private static IEnumerable<string> GetParameterAsTextReference(string parameterName) => Constants.TrailingSentenceMarkers.Select(_ => string.Concat(" ", parameterName, _.ToString()));

        private static string GetParameterAsReference(string parameterName) => parameterName.SurroundedWithDoubleQuote();

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
                            // that's text before the parameter, so add the missing character afterward
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

//// ncrunch: rdi off

        // TODO RKN: see Constants.Comments.ExceptionForbiddenStartingPhrase
        private static IEnumerable<string> CreatePhrases()
        {
            var starts = new[] { "This exception", "The exception", "An exception", "A exception", "Exception" };
            var verbs = new[] { "gets thrown", "is thrown", "will be thrown", "should be thrown" };
            var rawConditions = new[] { "if", "in case", "when" };
            var conditions = new[] { "in case that", "in case which" }.Concat(rawConditions);
            var parts = new[] { "that", "which" };
            var specialParts = new[] { "thrown", "throws", "throw" };

            foreach (var s in starts)
            {
                foreach (var v in verbs)
                {
                    foreach (var c in conditions)
                    {
                        var phrase0 = $"{s} {v} {c} ";

                        foreach (var p in parts)
                        {
                            var phrase1 = $"{s} {p} {v} {c} ";
                            var phrase2 = phrase1 + p + " ";
                            var phrase3 = phrase0 + p + " ";

                            yield return phrase3 + "the ";
                            yield return phrase3;
                            yield return phrase2 + "the ";
                            yield return phrase2;
                            yield return phrase1 + "the ";
                            yield return phrase1;
                        }

                        yield return phrase0 + "the ";
                        yield return phrase0;
                    }
                }
            }

            foreach (var v in verbs)
            {
                foreach (var c in conditions)
                {
                    var phrase = $"{v} {c} ";

                    yield return phrase + "the ";
                    yield return phrase;

                    var upperCasePhrase = phrase.ToUpperCaseAt(0);
                    yield return upperCasePhrase + "the ";
                    yield return upperCasePhrase;
                }
            }

            foreach (var sp in specialParts)
            {
                yield return sp + " in case that ";
                yield return sp + " in case which ";

                var up = sp.ToUpperCaseAt(0);

                yield return up + " in case that ";
                yield return up + " in case which ";

                foreach (var c in rawConditions)
                {
                    var phrase = $"{sp} {c} ";
                    var upperCasePhrase = $"{up} {c} ";

                    yield return phrase + "the ";
                    yield return phrase;

                    yield return upperCasePhrase + "the ";
                    yield return upperCasePhrase;
                }
            }

            foreach (var c in conditions)
            {
                var phrase = c + " ";

                yield return phrase + "the ";
                yield return phrase;

                var upperCasePhrase = phrase.ToUpperCaseAt(0);

                yield return upperCasePhrase + "the ";
                yield return upperCasePhrase;
            }
        }

        //// ncrunch: rdi default
    }
}