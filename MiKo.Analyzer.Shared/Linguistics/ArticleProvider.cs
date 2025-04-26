using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class ArticleProvider
    {
        internal static string GetArticleFor(string text, in FirstWordHandling firstWordHandling = FirstWordHandling.None) => GetArticleFor(text.AsSpan(), firstWordHandling);

        internal static string GetArticleFor(in ReadOnlySpan<char> text, in FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            if (text.Length == 0)
            {
                return string.Empty;
            }

            if (text.StartsWithAny("AaEeIiOo"))
            {
                return An(firstWordHandling);
            }

            if (text.StartsWithAny("Uu"))
            {
                if (text.StartsWith("uni", StringComparison.OrdinalIgnoreCase))
                {
                    if (text.Length > 3 && text[3].ToUpperCase() == 'N')
                    {
                        // something like 'uninformed', so is a vowel sound
                        return An(firstWordHandling);
                    }

                    // uni is pronounced like 'yuni' where 'y' is a consonant and no vowel sound, hence we have to use 'A'
                    return A(firstWordHandling);
                }

                return An(firstWordHandling);
            }

            if (text.StartsWithAny("Hh"))
            {
                if (text.StartsWith("hon", StringComparison.OrdinalIgnoreCase))
                {
                    // it is a vowel sound if 'h' is followed by 'on' (as in such case 'h' it is silent, like in 'honest')
                    return An(firstWordHandling);
                }
            }

            return A(firstWordHandling);

            string A(in FirstWordHandling handling) => StringExtensions.HasFlag(handling, FirstWordHandling.MakeLowerCase) ? "a " : "A ";

            string An(in FirstWordHandling handling) => StringExtensions.HasFlag(handling, FirstWordHandling.MakeLowerCase) ? "an " : "An ";
        }
    }
}