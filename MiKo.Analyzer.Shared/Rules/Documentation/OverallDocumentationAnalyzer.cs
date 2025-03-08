using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class OverallDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected OverallDocumentationAnalyzer(string id) : base(id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol.IsPrimaryConstructor() is false;

        protected virtual Diagnostic Issue(Location location, string replacement) => base.Issue(location, replacement);

        protected IReadOnlyList<Diagnostic> AnalyzeForSpecialPhrase(SyntaxToken token, string startingPhrase, Func<string, string> replacementCallback)
        {
            var locations = GetAllLocations(token, startingPhrase);

            if (locations.Count > 0)
            {
                return AnalyzeForSpecialPhraseLocal();
            }

            return Array.Empty<Diagnostic>();

            IReadOnlyList<Diagnostic> AnalyzeForSpecialPhraseLocal()
            {
                var issues = new List<Diagnostic>(locations.Count);

                foreach (var location in locations)
                {
                    var start = location.SourceSpan.Start;
                    var index = start - token.SpanStart + startingPhrase.Length;

                    var textAfterStartingPhrase = token.ValueText.AsSpan(index);
                    var nextWord = textAfterStartingPhrase.FirstWord();

                    var adjective = ReadOnlySpan<char>.Empty;

                    if (Verbalizer.IsAdjectiveOrAdverb(nextWord))
                    {
                        // jump over an adjective or adverb as next word
                        adjective = nextWord;

                        nextWord = textAfterStartingPhrase.SecondWord();

                        if (Verbalizer.IsAdjectiveOrAdverb(nextWord))
                        {
                            // jump over an adjective or adverb as next word
                            nextWord = textAfterStartingPhrase.ThirdWord();

                            adjective = textAfterStartingPhrase.Slice(0, textAfterStartingPhrase.IndexOf(nextWord)).Trim(Constants.WhiteSpaceCharacters);
                        }
                    }

                    // let's find the end of the next word in the source code (but keep in mind the offset of the starting phrase)
                    var offset = start + startingPhrase.Length;
                    var end = textAfterStartingPhrase.IndexOf(nextWord, StringComparison.Ordinal) + nextWord.Length + offset;

                    var replacement = replacementCallback(nextWord.ToString());

                    var finalReplacement = adjective.Length > 0
                                           ? adjective.ConcatenatedWith(' ', replacement.ToLowerCaseAt(0))
                                           : replacement;

                    var finalLocation = CreateLocation(token, start, end);

                    issues.Add(Issue(finalLocation, finalReplacement));
                }

                return issues;
            }
        }
    }
}