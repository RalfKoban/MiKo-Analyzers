using System;
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
        private static readonly string[] NonGenericStartParts = string.Format(Constants.Comments.BooleanReturnTypeStartingPhraseTemplate, "|").Split('|');
        private static readonly string[] NonGenericEndParts = string.Format(Constants.Comments.BooleanReturnTypeEndingPhraseTemplate, "|").Split('|');
        private static readonly string[] GenericStartParts = string.Format(Constants.Comments.BooleanTaskReturnTypeStartingPhraseTemplate, "|").Split('|');
        private static readonly string[] GenericEndParts = string.Format(Constants.Comments.BooleanTaskReturnTypeEndingPhraseTemplate, "|").Split('|');

        private static readonly string[] Phrases =
            {
                " if ",
                "A task that will complete with a result of ",
                "a task that will complete with a result of ",
                "TRUE:",
                "True:",
                "true:",
                "TRUE",
                "True",
                "true",
                "FALSE:",
                "False:",
                "false:",
                "FALSE",
                "False",
                "false",
                "Returns",
                "returns",
                "will return",
            };

        private static readonly string[] SimpleStartingPhrases = CreateSimpleStartingPhrases().ToArray();

        public override string FixableDiagnosticId => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2032_CodeFixTitle;

        internal static IEnumerable<string> CreateSimpleStartingPhrases()
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
                               };
            var verbs = new[] { "indicating", "that indicates", "which indicates" };
            var conditions = new[] { "if", "whether" };

            foreach (var start in starts)
            {
                foreach (var boolean in booleans)
                {
                    foreach (var verb in verbs)
                    {
                        foreach (var condition in conditions)
                        {
                            yield return $"{start}{boolean} {verb} {condition} ";
                        }
                    }
                }
            }
        }

        protected override XmlElementSyntax GenericComment(Document document, XmlElementSyntax comment, GenericNameSyntax returnType) => CommentCanBeFixed(comment)
                                                                                                                                             ? Comment(comment, GenericStartParts, GenericEndParts)
                                                                                                                                             : comment;

        protected override XmlElementSyntax NonGenericComment(Document document, XmlElementSyntax comment, TypeSyntax returnType) => CommentCanBeFixed(comment)
                                                                                                                                         ? Comment(comment, NonGenericStartParts, NonGenericEndParts)
                                                                                                                                         : comment;

        // introduced as workaround for issue #399
        private static bool CommentCanBeFixed(SyntaxNode syntax)
        {
            var comment = syntax.ToString();

            var falseIndex = comment.IndexOf("false", StringComparison.OrdinalIgnoreCase);
            if (falseIndex == -1)
            {
                return true;
            }

            var trueIndex = comment.IndexOf("true", StringComparison.OrdinalIgnoreCase);
            if (trueIndex == -1)
            {
                // cannot fix currently (false case comes as only case)
                if (comment.IndexOf("otherwise", StringComparison.OrdinalIgnoreCase) == -1)
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
                // get rid of first node (and any empty comment if the 1st node was the only one)
                comment = comment.Without(firstNode).WithoutWhitespaceOnlyComment();

                if (firstNode is XmlElementSyntax element)
                {
                    comment = comment.WithContent(element.WithoutFirstXmlNewLine().Content);

                    // create comment based on remaining text (without <para> tag)
                    var replacedComment = Comment(comment, startParts, endParts);

                    // re-wrap with <para> tag
                    var paraTag = element.WithContent(replacedComment.Content.WithLeadingXmlComment().WithTrailingXmlComment());

                    return replacedComment.WithContent(new SyntaxList<XmlNodeSyntax>(paraTag));
                }
                else
                {
                    // its an empty element, so nothing to do
                }
            }

            var middlePart = CreateMiddlePart(comment, startParts[1], endParts[0]);

            return Comment(comment, startParts[0], SeeLangword_True(), middlePart, SeeLangword_False(), endParts[1]);
        }

        private static IEnumerable<XmlNodeSyntax> CreateMiddlePart(XmlElementSyntax comment, string startingPhrase, string endingPhrase)
        {
            if (comment.Content.Count == 0)
            {
                // we have no comment, hence we add a "TODO" into the resulting comment
                return new[] { XmlText(startingPhrase + "TODO" + endingPhrase) };
            }

            const string OrIfPhrase = " or if ";
            const string OrIfReplacementPhrase = " orif ";

            // remove boolean <see langword="..."/> and <c>...</c>
            var adjustedComment = comment.Without(comment.Content.Where(_ => _.IsSeeLangwordBool() || _.IsCBool()));

            var nodes = adjustedComment.WithoutText(SimpleStartingPhrases)
                                       .ReplaceText(OrIfPhrase, OrIfReplacementPhrase)
                                       .WithoutText(Phrases)
                                       .WithStartText(startingPhrase) // add starting text and ensure that first character of original text is now lower-case
                                       .ReplaceText(OrIfReplacementPhrase, OrIfPhrase);

            // remove last node if it is ending with a dot
            if (nodes.LastOrDefault() is XmlTextSyntax sentenceEnding)
            {
                var text = GetText(sentenceEnding.WithoutTrailingCharacters(Constants.TrailingSentenceMarkers)
                                                 .WithoutTrailing(" otherwise"));

                if (text.IsNullOrWhiteSpace())
                {
                    nodes = nodes.Remove(sentenceEnding);
                }
            }

            // remove middle parts before the <see langword=""false""/>
            if (nodes.LastOrDefault() is XmlTextSyntax last)
            {
                var replacement = last.WithoutTrailingCharacters(Constants.TrailingSentenceMarkers)
                                      .WithoutTrailing(" otherwise")
                                      .WithoutTrailing(" otherwise with a result of")
                                      .WithoutTrailing(" else it")
                                      .WithoutTrailing(" else with")
                                      .WithoutTrailingCharacters(Constants.TrailingSentenceMarkers);

                var text = GetText(replacement);

                if (text.IsNullOrWhiteSpace())
                {
                    nodes = nodes.Remove(last);

                    // remove left-over trailing sentence marker on the middle string
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
                        // seems like there was an '///' at the very end of the text, so move text on same line
                        var glue = untouchedText[0].IsWhiteSpace() ? " " : string.Empty;

                        return nodes.Replace(last, XmlText(glue + text + endingPhrase));
                    }

                    nodes = nodes.Replace(last, replacement);
                }
            }

            return nodes.Add(XmlText(endingPhrase));
        }
    }
}