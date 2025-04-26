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

        internal static bool EndsWithPeriod(in ReadOnlySpan<char> comment) => comment.EndsWith('.')
                                                                        && comment.EndsWith("...", StringComparison.Ordinal) is false
                                                                        && comment.EndsWith("etc.", StringComparison.OrdinalIgnoreCase) is false;

        internal static bool ContainsDoublePeriod(in ReadOnlySpan<char> comment) => comment.Contains("..", _ => AllowedChars.Contains(_) is false, StringComparison.Ordinal)
                                                                              && comment.EndsWith("...", StringComparison.Ordinal) is false;

        internal static bool ContainsPhrase(string phrase, in ReadOnlySpan<char> comment)
        {
            // use string here to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see 'IndexOf' method inside 'MemoryExtensions')
            var index = comment.ToString().IndexOf(phrase, StringComparison.OrdinalIgnoreCase);

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

        internal static bool ContainsPhrases(string[] phrases, in ReadOnlySpan<char> comment) => comment.ToString().ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);
    }
}