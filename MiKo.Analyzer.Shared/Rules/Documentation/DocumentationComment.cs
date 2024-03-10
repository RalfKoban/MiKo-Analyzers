using System;
using System.Collections.Generic;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    internal static class DocumentationComment
    {
        private static readonly HashSet<char> AllowedChars = new HashSet<char>
                                                             {
                                                                 '.',
                                                                 '/',
                                                                 '\\',
                                                             };

        internal static bool EndsWithPeriod(ReadOnlySpan<char> comment) => comment.EndsWith('.')
                                                                        && comment.EndsWith("...", StringComparison.OrdinalIgnoreCase) is false
                                                                        && comment.EndsWith("etc.", StringComparison.OrdinalIgnoreCase) is false;

        internal static bool ContainsDoublePeriod(ReadOnlySpan<char> comment) => comment.Contains("..", _ => AllowedChars.Contains(_) is false, StringComparison.OrdinalIgnoreCase)
                                                                              && comment.EndsWith("...", StringComparison.OrdinalIgnoreCase) is false;

        internal static bool ContainsPhrase(string phrase, ReadOnlySpan<char> comment)
        {
            var index = comment.IndexOf(phrase.AsSpan(), StringComparison.OrdinalIgnoreCase);

            if (index < 0)
            {
                return false;
            }

            var indexAfterPhrase = index + phrase.Length;

            if (indexAfterPhrase == comment.Length)
            {
                // that's the last phrase
                return true;
            }

            return comment.Slice(indexAfterPhrase).StartsWithAny(Constants.Comments.Delimiters);
        }

        internal static bool ContainsPhrases(string[] phrases, ReadOnlySpan<char> comment) => comment.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);
    }
}