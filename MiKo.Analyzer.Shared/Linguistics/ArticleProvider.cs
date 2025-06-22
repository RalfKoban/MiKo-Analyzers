using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class ArticleProvider
    {
        internal static string GetArticleFor(string text, in FirstWordHandling firstWordHandling = FirstWordHandling.None) => GetArticleFor(text.AsSpan(), firstWordHandling);

        internal static string GetArticleFor(in ReadOnlySpan<char> text, in FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            if (text.Length is 0)
            {
                return string.Empty;
            }

            // the order of following if blocks is based on the following information
            //
            // | Letter | Probability (%) |
            // |--------|-----------------|
            // | A / a  | ~ 11.7          |
            // | E / e  | ~ 2.8           |
            // | I / i  | ~ 7.3           |
            // | O / o  | ~ 3.3           |
            // | U / u  | ~ 1.2           |
            // | H / h  | ~ 3.5           |
            if (text.StartsWithAny("AaIi"))
            {
                return An(firstWordHandling);
            }

            if (text.StartsWithAny("Hh"))
            {
                return ArticleForH(text, firstWordHandling);
            }

            if (text.StartsWithAny("Oo"))
            {
                return ArticleForO(text, firstWordHandling);
            }

            if (text.StartsWithAny("Ee"))
            {
                return ArticleForE(text, firstWordHandling);
            }

            if (text.StartsWithAny("Uu"))
            {
                return ArticleForU(text, firstWordHandling);
            }

            return A(firstWordHandling);
        }

        private static string ArticleForU(in ReadOnlySpan<char> text, in FirstWordHandling firstWordHandling)
        {
            if (text.StartsWith("uni", StringComparison.OrdinalIgnoreCase))
            {
                if (text.Length > 3)
                {
                    switch (text[3])
                    {
                        case 'n':
                        case 'N':
                        {
                            // something like 'uninformed', so is a vowel sound
                            return An(firstWordHandling);
                        }
                    }
                }

                // uni is pronounced like 'yuni' where 'y' is a consonant and no vowel sound, hence we have to use 'A'
                return A(firstWordHandling);
            }

            if (text.StartsWith("use", StringComparison.OrdinalIgnoreCase))
            {
                // something like 'use case' or 'user'
                return A(firstWordHandling);
            }

            return An(firstWordHandling);
        }

        private static string ArticleForE(in ReadOnlySpan<char> text, in FirstWordHandling firstWordHandling)
        {
            if (text.StartsWith("eu", StringComparison.OrdinalIgnoreCase))
            {
                // Words like 'european' start with a 'y' sound which is a consonant and no vowel sound, hence we have to use 'A'
                return A(firstWordHandling);
            }

            return An(firstWordHandling);
        }

        private static string ArticleForO(in ReadOnlySpan<char> text, in FirstWordHandling firstWordHandling)
        {
            if (text.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            {
                // in English, the words 'one' or 'once' begin with the letter 'o', but they are pronounced with an initial 'w' sound which is a consonant sound, not a vowel sound
                return A(firstWordHandling);
            }

            return An(firstWordHandling);
        }

        private static string ArticleForH(in ReadOnlySpan<char> text, in FirstWordHandling firstWordHandling)
        {
            if (text.StartsWith("herb", StringComparison.OrdinalIgnoreCase) // in American English, 'h' is silent in 'herb', so it starts with a vowel sound
             || text.StartsWith("heir", StringComparison.OrdinalIgnoreCase) // 'h' is silent in 'heir', so it starts with a vowel sound
             || text.StartsWith("hon", StringComparison.OrdinalIgnoreCase) // 'h' it is silent in 'honest', so it starts with a vowel sound
             || text.StartsWith("hou", StringComparison.OrdinalIgnoreCase)) // 'h' is silent in 'hour', so it starts with a vowel sound
            {
                return An(firstWordHandling);
            }

            return A(firstWordHandling);
        }

        private static string A(in FirstWordHandling handling) => handling.HasSet(FirstWordHandling.StartLowerCase) ? "a " : "A ";

        private static string An(in FirstWordHandling handling) => handling.HasSet(FirstWordHandling.StartLowerCase) ? "an " : "An ";
    }
}