using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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
            };

        public override string FixableDiagnosticId => MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2032_CodeFixTitle;

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType) => Comment(comment, GenericStartParts, GenericEndParts);

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType) => Comment(comment, NonGenericStartParts, NonGenericEndParts);

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
                // we have no comment, hence we fake a "..." into the resulting comment
                return new[] { XmlText(startingPhrase + "..." + endingPhrase) };
            }

            // remove boolean <see langword="..."/> and <c>...</c>
            var adjustedComment = comment.Without(comment.Content.Where(_ => _.IsSeeLangwordBool() || _.IsCBool()));

            var nodes = adjustedComment.WithoutText(Phrases).WithStartText(startingPhrase); // add starting text and ensure that first character of original text is now lower-case

            // remove last node if it is ending with a dot
            if (nodes.LastOrDefault() is XmlTextSyntax sentenceEnding)
            {
                var text = sentenceEnding.WithoutTrailingCharacters(Constants.TrailingSentenceMarkers)
                                         .WithoutTrailing(" otherwise")
                                         .WithoutXmlCommentExterior();
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
                                      .WithoutTrailing(" otherwise with a result of ")
                                      .WithoutTrailingCharacters(Constants.TrailingSentenceMarkers);

                var text = replacement.WithoutXmlCommentExterior();

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
                    nodes = nodes.Replace(last, replacement);
                }
            }

            return nodes.Add(XmlText(endingPhrase));
        }
    }
}