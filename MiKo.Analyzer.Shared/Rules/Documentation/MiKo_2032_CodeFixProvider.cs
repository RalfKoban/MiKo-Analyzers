﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2032_CodeFixProvider)), Shared]
    public sealed class MiKo_2032_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly string[] NonGenericStartParts = Constants.Comments.BooleanReturnTypeStartingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string[] NonGenericEndParts = Constants.Comments.BooleanReturnTypeEndingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string[] GenericStartParts = Constants.Comments.BooleanTaskReturnTypeStartingPhraseTemplate.FormatWith("|").Split('|');
        private static readonly string[] GenericEndParts = Constants.Comments.BooleanTaskReturnTypeEndingPhraseTemplate.FormatWith("|").Split('|');

        private static readonly string[] Phrases = AlmostCorrectTaskReturnTypeStartingPhrases.Concat(new[]
                                                                                                         {
                                                                                                             " if ",
                                                                                                             "A task that has the result ",
                                                                                                             "A task that completes with a result of ",
                                                                                                             "a task that completes with a result of ",
                                                                                                             "A task that will complete with a result of ",
                                                                                                             "a task that will complete with a result of ",
                                                                                                             "A task that represents the operation. The Result indicates whether ",
                                                                                                             "a task that represents the operation. The Result indicates whether ",
                                                                                                             "A task that represents the asynchronous operation. The Result indicates whether ",
                                                                                                             "a task that represents the asynchronous operation. The Result indicates whether ",
                                                                                                             "A task representing the operation. The Result indicates whether ",
                                                                                                             "a task representing the operation. The Result indicates whether ",
                                                                                                             "A task representing the asynchronous operation. The Result indicates whether ",
                                                                                                             "a task representing the asynchronous operation. The Result indicates whether ",
                                                                                                             "TRUE:",
                                                                                                             "True:",
                                                                                                             "true:",
                                                                                                             "TRUE,",
                                                                                                             "True,",
                                                                                                             "true,",
                                                                                                             "TRUE means that",
                                                                                                             "True means that",
                                                                                                             "true means that",
                                                                                                             "TRUE means",
                                                                                                             "True means",
                                                                                                             "true means",
                                                                                                             "TRUE when",
                                                                                                             "True when",
                                                                                                             "true when",
                                                                                                             "TRUE of",
                                                                                                             "True of",
                                                                                                             "true of",
                                                                                                             "TRUE",
                                                                                                             "True",
                                                                                                             "true",
                                                                                                             "FALSE:",
                                                                                                             "False:",
                                                                                                             "false:",
                                                                                                             "FALSE,",
                                                                                                             "False,",
                                                                                                             "false,",
                                                                                                             "FALSE",
                                                                                                             "False",
                                                                                                             "false",
                                                                                                             "Returns",
                                                                                                             "returns",
                                                                                                             "will return",
                                                                                                         }).OrderByDescending(_ => _.Length).ThenBy(_ => _).ToArray();

        private static readonly string[] SimpleStartingPhrases = CreateSimpleStartingPhrases().OrderByDescending(_ => _.Length).ThenBy(_ => _).ToArray();

        private static readonly string[] DelimiterPhrases =
                                                            {
                                                                ",",
                                                                ";",
                                                                ":",
                                                                "-",
                                                            };

        private static readonly string[] OtherStartingPhrases =
                                                                {
                                                                    "If ",
                                                                    "When ",
                                                                    "Whether ",
                                                                    "In case that ",
                                                                    "In case ",
                                                                    "Means that ",
                                                                    "Means ",
                                                                    "if ",
                                                                    "when ",
                                                                    "whether ",
                                                                    "in case that ",
                                                                    "in case ",
                                                                    "means that ",
                                                                    "means ",
                                                                };

        private static readonly string[] SimpleTrailingPhrases =
                                                                 {
                                                                     " otherwise",
                                                                     " otherwise not",
                                                                     " otherwise with a result of",
                                                                     " otherwise the task has the result",
                                                                     " else it",
                                                                     " else with",
                                                                     " in all of the other cases",
                                                                     " in all other cases",
                                                                     " in all other case",
                                                                     " in any of the other cases",
                                                                     " in any other cases",
                                                                     " in any other case",
                                                                     " in each of the other cases",
                                                                     " in each other cases",
                                                                     " in each other case",
                                                                     " in the other cases",
                                                                     " in the other case",
                                                                     " in other cases",
                                                                     " in other case",
                                                                     ", ; else",
                                                                     ", else",
                                                                     " Otherwise ",
                                                                     " Otherwise",
                                                                 };
//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2032";

//// ncrunch: rdi off

        public static ISet<string> CreateSimpleStartingPhrases()
        {
            var starts = new[] { "A ", "An ", string.Empty };
            var booleans = new[]
                               {
                                   "bool value",
                                   "Bool value",
                                   "bool",
                                   "Bool",
                                   "boolean value",
                                   "Boolean value",
                                   "boolean",
                                   "Boolean",
                                   "value",
                                   "Value",
                               };
            var verbs = new[] { "indicating", "that indicates", "which indicates", "indicates", "indicate" };
            var conditions = new[] { "if", "whether", "when" };

            var results = new HashSet<string>();

            foreach (var start in starts)
            {
                foreach (var boolean in booleans)
                {
                    var begin = start + boolean + " ";

                    foreach (var verb in verbs)
                    {
                        var beginWithVerb = begin + verb + " ";

                        foreach (var condition in conditions)
                        {
                            results.Add(beginWithVerb + condition + " ");
                        }
                    }
                }
            }

            foreach (var condition in conditions)
            {
                results.Add("Indicates " + condition + " ");
            }

            results.AddRange(booleans);

            return results;
        }

//// ncrunch: rdi default

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, string memberName, GenericNameSyntax returnType) => CommentCanBeFixed(comment)
                                                                                                                                                            ? Comment(comment, GenericStartParts, GenericEndParts)
                                                                                                                                                            : comment;

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, string memberName, TypeSyntax returnType) => CommentCanBeFixed(comment)
                                                                                                                                                        ? Comment(comment, NonGenericStartParts, NonGenericEndParts)
                                                                                                                                                        : comment;

        // introduced as workaround for issue #399
        private static bool CommentCanBeFixed(XmlElementSyntax syntax)
        {
            var comment = syntax.ToString();

            var falseIndex = comment.IndexOf("false", StringComparison.OrdinalIgnoreCase);

            if (falseIndex is -1)
            {
                return true;
            }

            var trueIndex = comment.IndexOf("true", StringComparison.OrdinalIgnoreCase);

            if (trueIndex is -1)
            {
                // cannot fix currently (false case comes as only case)
                if (comment.Contains("otherwise", StringComparison.OrdinalIgnoreCase) is false)
                {
                    return false;
                }
            }
            else
            {
                if (falseIndex < trueIndex)
                {
                    // cannot fix currently (false case comes before true case)
                    return false;
                }
            }

            return true;
        }

        private static XmlElementSyntax Comment(XmlElementSyntax comment, IReadOnlyList<string> startParts, IReadOnlyList<string> endParts)
        {
            comment = comment.WithoutFirstXmlNewLine();

            var contents = comment.Content;

            // handle situation that comment starts with <para>
            var firstNode = contents.FirstOrDefault();

            if (firstNode.IsPara())
            {
                // clean up comment and remove first node (and any empty comment if the 1st node was the only one)
                comment = comment.Without(firstNode).WithoutWhitespaceOnlyComment();

                if (firstNode is XmlElementSyntax element)
                {
                    comment = comment.WithContent(element.WithoutFirstXmlNewLine().Content);

                    // create comment based on remaining text (without <para> tag)
                    var replacedComment = Comment(comment, startParts, endParts);

                    // re-wrap with <para> tag
                    var paraTag = element.WithContent(replacedComment.Content.WithLeadingXmlComment().WithTrailingXmlComment());

                    return replacedComment.WithContent(paraTag);
                }
                else
                {
                    // it's an empty element, so nothing to do
                }
            }

            var middlePart = CreateMiddlePart(comment, startParts[1], endParts[0]);

            return Comment(comment, startParts[0], SeeLangword_True(), middlePart, SeeLangword_False(), endParts[1]);
        }

        private static IEnumerable<XmlNodeSyntax> CreateMiddlePart(XmlElementSyntax comment, string startingPhrase, string endingPhrase)
        {
            if (comment.Content.Count is 0)
            {
                // we have no comment, hence we add a "TODO" into the resulting comment
                return new[] { XmlText(startingPhrase + Constants.TODO + endingPhrase) };
            }

            const string OrIfPhrase = " or if ";
            const string OrIfReplacementPhrase = " orif ";

            // clean up comment and remove boolean <see langword="..."/> and <c>...</c>
            var adjustedComment = RemoveBooleansTags(comment);

            var nodes = adjustedComment.WithoutStartText(DelimiterPhrases)
                                       .WithoutStartText(SimpleStartingPhrases)
                                       .WithoutStartText(OtherStartingPhrases)
                                       .ReplaceText(OrIfPhrase, OrIfReplacementPhrase)
                                       .WithoutText(Phrases)
                                       .WithoutFirstXmlNewLine();

            if (nodes.Count is 0)
            {
                // we have no comment, hence we add a "TODO" into the resulting comment
                return new[] { XmlText(startingPhrase + Constants.TODO + endingPhrase) };
            }

            nodes = nodes.WithStartText(startingPhrase) // keep starting text and ensure that first character of original text is now lower-case
                         .ReplaceText(OrIfReplacementPhrase, OrIfPhrase);

            // clean up comment and remove last node if it is ending with a dot
            if (nodes.LastOrDefault() is XmlTextSyntax sentenceEnding)
            {
                var ending = sentenceEnding.WithoutTrailingCharacters(Constants.TrailingSentenceMarkers)
                                           .WithoutTrailing(" otherwise");

                if (ending.IsWhiteSpaceOnlyText())
                {
                    nodes = nodes.Remove(sentenceEnding);
                }
            }

            // clean up comment and remove middle parts before the <see langword=""false""/>
            if (nodes.LastOrDefault() is XmlTextSyntax last)
            {
                var replacement = last.WithoutTrailingXmlComment()
                                      .WithoutTrailingCharacters(Constants.TrailingSentenceMarkers)
                                      .WithoutTrailingXmlComment()
                                      .WithoutTrailing(SimpleTrailingPhrases)
                                      .WithoutTrailingCharacters(Constants.TrailingSentenceMarkers);

                if (replacement.IsWhiteSpaceOnlyText())
                {
                    nodes = nodes.Remove(last);

                    // clean up comment and remove left-over trailing sentence marker on the middle string
                    if (nodes.LastOrDefault() is XmlTextSyntax newLast)
                    {
                        nodes = nodes.Replace(newLast, newLast.WithoutTrailingCharacters(Constants.TrailingSentenceMarkers));
                    }
                }
                else
                {
                    var textWithoutTrailingXml = replacement.WithoutTrailingXmlComment().ToString();
                    var untouchedText = replacement.ToString();

                    if (textWithoutTrailingXml.Length != untouchedText.Length)
                    {
                        // seems like there was an '///' at the very end of the text, so move trailing text on same line
                        return nodes.Replace(last, XmlText(textWithoutTrailingXml + endingPhrase));
                    }

                    nodes = nodes.Replace(last, replacement);
                }
            }

            return nodes.Add(XmlText(endingPhrase));
        }
    }
}