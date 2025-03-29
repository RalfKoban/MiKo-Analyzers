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
                                                                };

        public override string FixableDiagnosticId => "MiKo_2019";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is XmlElementSyntax summary)
            {
                var content = summary.Content;

                if (content.Count > 0 && content[0] is XmlTextSyntax textSyntax)
                {
                    var startText = textSyntax.GetTextWithoutTrivia();

                    if (startText.StartsWith("Given ", StringComparison.Ordinal))
                    {
                        // we cannot fix that for the moment
                        return syntax;
                    }

                    if (startText.StartsWithAny(RepresentsCandidates, StringComparison.Ordinal))
                    {
                        return CommentStartingWith(summary, "Represents a ");
                    }

                    var updatedSyntax = MiKo_2012_CodeFixProvider.GetUpdatedSyntax(summary, textSyntax);

                    if (ReferenceEquals(summary, updatedSyntax) is false)
                    {
                        return updatedSyntax;
                    }

                    var text = startText.AsSpan();
                    var firstWord = text.FirstWord();

                    // only adjust in case there is no single letter
                    if (firstWord.Length > 1)
                    {
                        if (firstWord.EndsWith("alled", StringComparison.Ordinal))
                        {
                            // currently we cannot adjust "Called" text properly
                        }
                        else
                        {
                            var index = text.IndexOf(firstWord);
                            var remainingText = text.Slice(index + firstWord.Length);

                            var replacementForFirstWord = Verbalizer.MakeThirdPersonSingularVerb(firstWord.ToUpperCaseAt(0));

                            var replacedText = replacementForFirstWord.ConcatenatedWith(remainingText);

                            return Comment(summary, replacedText, content.RemoveAt(0));
                        }
                    }
                }
            }

            return syntax;
        }
    }
}