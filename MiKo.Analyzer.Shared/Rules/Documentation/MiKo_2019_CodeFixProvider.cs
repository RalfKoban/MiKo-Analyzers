using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2019_CodeFixProvider)), Shared]
    public sealed class MiKo_2019_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly string[] RepresentsCandidates =
                                                                {
                                                                    "Repository ",
                                                                    "Simple ",
                                                                    "Complex ",
                                                                    "Additional ",
                                                                    "Addtional ", // typo
                                                                    "Health ",
                                                                };

        private static readonly string[] CurrentlyUnfixable =
                                                              {
                                                                  "Given ",
                                                                  "When ",
                                                              };

        private static readonly string[] DeterminesCandidates =
                                                                {
                                                                    "Whether",
                                                                    "If",
                                                                };

        public override string FixableDiagnosticId => "MiKo_2019";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax summary)
            {
                var content = summary.Content;

                if (content.Count > 0 && content[0] is XmlTextSyntax textSyntax)
                {
                    var startText = textSyntax.GetTextTrimmed();

                    if (startText.StartsWithAny(CurrentlyUnfixable))
                    {
                        // currently we cannot fix that
                        return syntax;
                    }

                    var text = startText.AsSpan();
                    var firstWord = text.FirstWord();

                    if (startText.StartsWithAny(RepresentsCandidates))
                    {
                        var article = ArticleProvider.GetArticleFor(firstWord, FirstWordAdjustment.StartLowerCase);

                        return CommentStartingWith(summary, "Represents " + article);
                    }

                    var updatedSyntax = MiKo_2012_CodeFixProvider.GetUpdatedSyntax(summary);

                    if (ReferenceEquals(summary, updatedSyntax) is false)
                    {
                        return updatedSyntax;
                    }

                    // only adjust in case there is no single letter
                    if (firstWord.Length > 1)
                    {
                        if (firstWord.EndsWith("alled"))
                        {
                            // currently we cannot adjust "Called" text properly
                            return syntax;
                        }

                        var index = text.IndexOf(firstWord);
                        var remainingText = text.Slice(index + firstWord.Length);

                        var firstWordUpper = firstWord.ToUpperCaseAt(0);

                        var replacementForFirstWord = firstWordUpper.EqualsAny(DeterminesCandidates)
                                                      ? "Determines whether"
                                                      : Verbalizer.MakeThirdPersonSingularVerb(firstWordUpper);

                        var replacedText = replacementForFirstWord.ConcatenatedWith(remainingText);

                        return Comment(summary, replacedText, content.RemoveAt(0));
                    }
                }
            }

            return syntax;
        }
    }
}