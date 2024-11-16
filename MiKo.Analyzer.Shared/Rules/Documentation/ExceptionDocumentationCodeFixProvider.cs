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
                    var ps = parameters.Where(_ => comment.ContainsAny(GetParameterReferences(_).ToArray())).ToArray();

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

            if (textNodes.Count != 0)
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
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var exception in comment.Content.OfType<XmlElementSyntax>().Where(_ => _.IsException()))
            {
                var fixedException = FixExceptionComment(syntax, exception, comment, diagnostic);

                if (fixedException != null)
                {
                    return fixedException;
                }
            }

            return null;
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

        private static IEnumerable<string> GetParameterAsTextReference(string parameterName) => Constants.TrailingSentenceMarkers.Select(_ => ' '.ConcatenatedWith(parameterName, _));

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

        private static string[] SplitCommentsOnParametersInText(string comment, string[] parametersAsTextReferences)
        {
            var parametersAsTextReferencesLength = parametersAsTextReferences.Length;

            // split into parts, so that we can easily detect which part is a parameter and which is some normal text
            var parts = comment.SplitBy(parametersAsTextReferences).ToArray();
            var partsLength = parts.Length;

            // now correct the parameters back
            for (var i = 0; i < partsLength; i++)
            {
                var part = parts[i];

                for (var index = 0; index < parametersAsTextReferencesLength; index++)
                {
                    if (part == parametersAsTextReferences[index])
                    {
                        if (i > 0)
                        {
                            // that's text before the parameter, so add the missing character afterward
                            parts[i - 1] = parts[i - 1].ConcatenatedWith(part.First());
                        }

                        if (i < partsLength - 1)
                        {
                            // that's text after the parameter, so add the missing character before
                            parts[i + 1] = part.Last().ConcatenatedWith(parts[i + 1]);
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
            var conditions = new[] { "in case that", "in case which" }.Concat(rawConditions).ToArray();
            var parts = new[] { "that", "which" };
            var specialParts = new[] { "thrown", "throws", "throw" };

            foreach (var v in verbs)
            {
                foreach (var c in conditions)
                {
                    var phrase = string.Concat(v, " ", c, " ");

                    foreach (var s in starts)
                    {
                        var phrase0 = string.Concat(s, " ", phrase);

                        foreach (var p in parts)
                        {
                            var part = p + " ";

                            var phrase1 = string.Concat(s, " ", part, phrase);
                            var phrase2 = phrase1 + part;
                            var phrase3 = phrase0 + part;

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
                    var phrase = string.Concat(sp, " ", c, " ");
                    var upperCasePhrase = string.Concat(up, " ", c, " ");

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