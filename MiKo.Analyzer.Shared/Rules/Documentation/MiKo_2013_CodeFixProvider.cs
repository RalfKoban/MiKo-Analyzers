﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2013_CodeFixProvider)), Shared]
    public sealed class MiKo_2013_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private const string Phrase = Constants.Comments.EnumStartingPhrase;

        private static readonly string[] StartingPhrases =
                                                           {
                                                               "Defines",
                                                               "Indicates",
                                                               "Specifies",
                                                           };

        public override string FixableDiagnosticId => MiKo_2013_EnumSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2013_CodeFixTitle.FormatWith(Phrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => CommentWithStartingPhrase((XmlElementSyntax)syntax, Phrase);

        private static SyntaxNode CommentWithStartingPhrase(XmlElementSyntax comment, string startingPhrase)
        {
            if (comment.Content[0] is XmlTextSyntax text)
            {
                var contents = new List<XmlNodeSyntax>(comment.Content.Count);
                contents.Add(NewXmlComment(text, startingPhrase));
                contents.AddRange(comment.Content.Skip(1));

                // fix last item's CRLF
                if (contents[contents.Count - 1] is XmlTextSyntax last)
                {
                    contents[contents.Count - 1] = WithoutEmptyTextAtEnd(last, last.TextTokens.ToList());
                }

                return SyntaxFactory.XmlElement(
                                                comment.StartTag,
                                                SyntaxFactory.List(contents),
                                                comment.EndTag.WithLeadingXmlComment());
            }

            // happens if we start e.g. with a <see link
            return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            comment.Content.Insert(0, XmlText(startingPhrase).WithLeadingXmlComment()),
                                            comment.EndTag.WithLeadingXmlComment());
        }

        private static XmlNodeSyntax NewXmlComment(XmlTextSyntax comment, string text)
        {
            var textTokens = comment.TextTokens.ToList();

            // get rid of all empty tokens at the beginning
            while (textTokens.Any() && textTokens[0].ValueText.IsNullOrWhiteSpace())
            {
                textTokens.RemoveAt(0);
            }

            if (textTokens.Any())
            {
                // fix starting text
                var existingText = textTokens[0].WithoutTrivia().ValueText.AsSpan();
                var firstWord = existingText.FirstWord();

                if (firstWord.EqualsAny(StartingPhrases))
                {
                    existingText = existingText.WithoutFirstWord();
                }

                textTokens.RemoveAt(0);
                textTokens.Insert(0, XmlTextToken(text + existingText.Trim().ToString()));
            }
            else
            {
                // we are on same line
                textTokens.Add(XmlTextToken(text));
            }

            return WithoutEmptyTextAtEnd(comment, textTokens);
        }

        private static XmlNodeSyntax WithoutEmptyTextAtEnd(XmlTextSyntax comment, IList<SyntaxToken> textTokens)
        {
            // get rid of all empty tokens at the end as we re-add some
            while (textTokens.Count > 0 && textTokens[textTokens.Count - 1].ValueText.IsNullOrWhiteSpace())
            {
                textTokens.RemoveAt(textTokens.Count - 1);
            }

            return comment.WithTextTokens(SyntaxFactory.TokenList(textTokens));
        }

        private static SyntaxToken XmlTextToken(string text) => text.ToSyntaxToken(SyntaxKind.XmlTextLiteralToken).WithLeadingXmlComment();
    }
}