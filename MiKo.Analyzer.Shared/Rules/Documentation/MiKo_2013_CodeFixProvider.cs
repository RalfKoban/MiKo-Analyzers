using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2013_CodeFixProvider)), Shared]
    public sealed class MiKo_2013_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private const string Phrase = Constants.Comments.EnumStartingPhrase;
        private const string KindPhrase = Phrase + "the different kinds of ";

        private static readonly string[] WrongStartingWords =
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

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2013";

        protected override string Title => Resources.MiKo_2013_CodeFixTitle.FormatWith(Phrase);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => CommentWithStartingPhrase((XmlElementSyntax)syntax, Phrase);

        private static XmlElementSyntax CommentWithStartingPhrase(XmlElementSyntax comment, string startingPhrase)
        {
            var originalContent = comment.Content;

            if (originalContent[0] is XmlTextSyntax text)
            {
                var contents = new List<XmlNodeSyntax>(originalContent.Count);
                contents.Add(NewXmlComment(text, startingPhrase));
                contents.AddRange(originalContent.Skip(1));

                // fix last item's CRLF
                var lastIndex = contents.Count - 1;

                if (contents[lastIndex] is XmlTextSyntax last)
                {
                    contents[lastIndex] = WithoutEmptyTextAtEnd(last, last.TextTokens.ToList());
                }

                return CommentWithContent(comment, contents.ToSyntaxList());
            }

            // happens if we start e.g. with a <see link
            return CommentWithContent(comment, originalContent.Insert(0, XmlText(startingPhrase).WithLeadingXmlComment()));
        }

        private static XmlTextSyntax NewXmlComment(XmlTextSyntax text, string startingPhrase)
        {
            var textTokens = text.TextTokens.ToList();

            // get rid of all empty tokens at the beginning
            while (textTokens.Count != 0 && textTokens[0].ValueText.IsNullOrWhiteSpace())
            {
                textTokens.RemoveAt(0);
            }

            if (textTokens.Count != 0)
            {
                // fix starting text
                var existingText = textTokens[0].WithoutTrivia().ValueText.AsSpan();
                var firstWord = existingText.FirstWord();

                if (firstWord.EqualsAny(WrongStartingWords))
                {
                    existingText = existingText.WithoutFirstWord();
                }

                // fix sentence ending
                var trimmedExistingTextEnd = string.Empty;
                var trimmedExistingText = existingText.Trim();

                if (trimmedExistingText.EndsWithAny(Constants.SentenceMarkers))
                {
                    var end = trimmedExistingText.Length - 1;

                    // send end here before slice gets re-assigned
                    trimmedExistingTextEnd = trimmedExistingText.Slice(end).ToString();

                    trimmedExistingText = trimmedExistingText.Slice(0, end);
                }

                var continuation = trimmedExistingText.AsBuilder().Without(EnumStartingPhrases).Trimmed();

                if (continuation.IsSingleWord())
                {
                    var word = continuation.ToString();

                    var enumName = text.FirstAncestor<EnumDeclarationSyntax>()?.GetName();

                    // seems like the continuation is a single word and ends with the name of the enum, so it can be lower-case plural
                    if (enumName == word || enumName == word + "Enum")
                    {
                        startingPhrase = KindPhrase;

                        // get rid of 'Type', 'Enum' and 'Kind' and ensure that we have a plural name here, but do not split yet into parts (as otherwise we would pluralize the wrong part)
                        continuation = continuation.Without("Type")
                                                   .Without("Kind")
                                                   .Without("Enum")
                                                   .WithoutAbbreviations()
                                                   .AdjustFirstWord(FirstWordHandling.MakePlural)
                                                   .SeparateWords(' ', FirstWordHandling.MakeLowerCase);
                    }
                    else
                    {
                        continuation = continuation.AdjustFirstWord(FirstWordHandling.MakeLowerCase);
                    }
                }
                else
                {
                    continuation = continuation.AdjustFirstWord(FirstWordHandling.MakeLowerCase);
                }

                var finalText = continuation.Insert(0, startingPhrase)
                                            .Append(trimmedExistingTextEnd)
                                            .ReplaceWithCheck(" kinds of state ", " states of ")
                                            .ReplaceWithCheck(" of of ", " of ")
                                            .ToString();

                textTokens.RemoveAt(0);
                textTokens.Insert(0, XmlTextToken(finalText));
            }
            else
            {
                // we are on same line
                textTokens.Add(XmlTextToken(startingPhrase));
            }

            return WithoutEmptyTextAtEnd(text, textTokens);
        }

        private static XmlTextSyntax WithoutEmptyTextAtEnd(XmlTextSyntax comment, List<SyntaxToken> textTokens)
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

                yield return "Gets or sets ";
                yield return "Gets or Sets ";
                yield return "Gets ";
                yield return "Sets ";
            }
        }
//// ncrunch: rdi default
    }
}