using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        protected IReadOnlyList<Diagnostic> AnalyzeForSpecialPhrase(in SyntaxToken syntaxToken, string startingPhrase, Func<string, string> replacementCallback)
        {
            var locations = GetAllLocations(syntaxToken, startingPhrase);

            if (locations.Count > 0)
            {
                return AnalyzeForSpecialPhraseLocal(syntaxToken);
            }

            return Array.Empty<Diagnostic>();

            IReadOnlyList<Diagnostic> AnalyzeForSpecialPhraseLocal(in SyntaxToken token)
            {
                var issues = new List<Diagnostic>(locations.Count);

                foreach (var location in locations)
                {
                    var start = location.SourceSpan.Start;
                    var index = start - token.SpanStart + startingPhrase.Length;

                    var textAfterStartingPhrase = token.ValueText.AsSpan(index);
                    var nextWord = textAfterStartingPhrase.FirstWord();

                    var adjective = ReadOnlySpan<char>.Empty;

                    if (AdjectiveFinder.IsAdjectiveOrAdverb(nextWord))
                    {
                        // jump over an adjective or adverb as next word
                        adjective = nextWord;

                        nextWord = textAfterStartingPhrase.SecondWord();

                        if (AdjectiveFinder.IsAdjectiveOrAdverb(nextWord))
                        {
                            // jump over an adjective or adverb as next word
                            nextWord = textAfterStartingPhrase.ThirdWord();

                            adjective = textAfterStartingPhrase.Slice(0, textAfterStartingPhrase.IndexOf(nextWord)).Trim(Constants.WhiteSpaceCharacters);
                        }
                    }

                    // let's find the end of the next word in the source code (but keep in mind the offset of the starting phrase)
                    var offset = start + startingPhrase.Length;
                    var end = textAfterStartingPhrase.IndexOf(nextWord) + nextWord.Length + offset;

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

        protected IReadOnlyList<Diagnostic> AnalyzeComment(
                                                       DocumentationCommentTriviaSyntax comment,
                                                       in ReadOnlySpan<string> termsForText,
                                                       in ReadOnlySpan<string> termsForAllLocations,
                                                       string replacement = "",
                                                       in StringComparison comparison = StringComparison.OrdinalIgnoreCase,
                                                       string ignoreText = "")
        {
            var textTokens = comment.GetXmlTextTokens(_ => CodeTags.Contains(_.GetName()) is false);
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.ContainsAny(termsForText, comparison) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                var syntaxToken = textTokens[i];
                var tokenText = syntaxToken.ValueText.AsSpan();

                if (ignoreText.Length > 0 && tokenText.Contains(ignoreText))
                {
                    // ignore that specific text
                    continue;
                }

                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(syntaxToken, termsForAllLocations, comparison, Offset, Offset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(locationsCount);
                    }

                    for (var index = 0; index < locationsCount; index++)
                    {
                        var issue = IssueLocal(locations[index]);

                        results.Add(issue);
                    }
                }

                foreach (var term in termsForAllLocations)
                {
                    var trimmedTerm = term.AsSpan().TrimEnd();

                    if (trimmedTerm.Length == term.Length)
                    {
                        // we could not trim, so we probably will not find it (as we get the terms with a ' ' character at the end
                        continue;
                    }

                    if (tokenText.EndsWith(trimmedTerm, comparison))
                    {
                        if (results is null)
                        {
                            results = new List<Diagnostic>(1);
                        }

                        var offset = syntaxToken.SpanStart;
                        var end = offset + tokenText.Length;
                        var start = end - trimmedTerm.Length;

                        if (trimmedTerm.StartsWith(' '))
                        {
                            start += Offset; // we do not want to underline the first char
                        }

                        var location = CreateLocation(syntaxToken, start, end);
                        var issue = IssueLocal(location);

                        results.Add(issue);

                        // we found one, so we do not need to look further
                        break;
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();

            Diagnostic IssueLocal(Location location) => replacement?.Length > 0 ? Issue(location, replacement) : Issue(location);
        }
    }
}