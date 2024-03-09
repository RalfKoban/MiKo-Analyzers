using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

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
                                                               "Contains",
                                                               "Define",
                                                               "Defined",
                                                               "Defines",
                                                               "Describe",
                                                               "Described",
                                                               "Describes",
                                                               "Identified",
                                                               "Identifies",
                                                               "Identify",
                                                               "Indicate",
                                                               "Indicated",
                                                               "Indicates",
                                                               "Present",
                                                               "Presents",
                                                               "Provide",
                                                               "Provides",
                                                               "Represent",
                                                               "Represents",
                                                               "Specified",
                                                               "Specifies",
                                                               "Specify",
                                                           };

        private static readonly string[] EnumStartingPhrases = CreateEnumStartingPhrases().ToArray();

        public override string FixableDiagnosticId => MiKo_2013_EnumSummaryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2013_CodeFixTitle.FormatWith(Phrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => CommentWithStartingPhrase((XmlElementSyntax)syntax, Phrase);

        private static SyntaxNode CommentWithStartingPhrase(XmlElementSyntax comment, string startingPhrase)
        {
            var originalContent = comment.Content;

            if (originalContent[0] is XmlTextSyntax text)
            {
                var contents = new List<XmlNodeSyntax>(originalContent.Count);
                contents.Add(NewXmlComment(text, startingPhrase));
                contents.AddRange(originalContent.Skip(1));

                // fix last item's CRLF
                if (contents[contents.Count - 1] is XmlTextSyntax last)
                {
                    contents[contents.Count - 1] = WithoutEmptyTextAtEnd(last, last.TextTokens.ToList());
                }

                return SyntaxFactory.XmlElement(
                                            comment.StartTag,
                                            contents.ToSyntaxList(),
                                            comment.EndTag.WithLeadingXmlComment());
            }

            // happens if we start e.g. with a <see link
            return SyntaxFactory.XmlElement(
                                        comment.StartTag,
                                        originalContent.Insert(0, XmlText(startingPhrase).WithLeadingXmlComment()),
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

                var continuation = new StringBuilder(existingText.Trim().ToString()).ReplaceAllWithCheck(EnumStartingPhrases, string.Empty).ToString();

                textTokens.RemoveAt(0);
                textTokens.Insert(0, XmlTextToken(text + continuation));
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

            return comment.WithTextTokens(textTokens.ToTokenList());
        }

        private static SyntaxToken XmlTextToken(string text) => text.AsToken(SyntaxKind.XmlTextLiteralToken).WithLeadingXmlComment();

//// ncrunch: rdi off
        private static IEnumerable<string> CreateEnumStartingPhrases()
        {
            foreach (var start in new[] { "Declaration", "Enum", "Enumeration", "Flagged enum", "Flagged enumeration", "Flags enum", "Flags enumeration", "State" })
            {
                yield return start + " of ";
                yield return start + " for ";

                yield return start + " contains ";
                yield return start + " describes ";
                yield return start + " represents ";

                yield return start + " containing ";
                yield return start + " describing ";
                yield return start + " representing ";

                yield return start + " that contains ";
                yield return start + " that describes ";
                yield return start + " that represents ";

                yield return start + " which contains ";
                yield return start + " which describes ";
                yield return start + " which represents ";

                yield return start + " ";
            }
        }
//// ncrunch: rdi default
    }
}