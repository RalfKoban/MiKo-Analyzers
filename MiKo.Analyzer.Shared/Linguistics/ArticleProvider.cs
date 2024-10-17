using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class ArticleProvider
    {
        internal static string GetArticleFor(string text, FirstWordHandling firstWordHandling = FirstWordHandling.None) => GetArticleFor(text.AsSpan(), firstWordHandling);

        internal static string GetArticleFor(ReadOnlySpan<char> text, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            if (text.Length == 0)
            {
                return string.Empty;
            }

            if (text.StartsWithAny("AaEeIiOo"))
            {
                return An();
            }

            if (text.StartsWithAny("Uu"))
            {
                if (text.StartsWith("uni", StringComparison.OrdinalIgnoreCase))
                {
                    if (text.Length > 3 && text[3].ToUpperCase() == 'N')
                    {
                        // something like 'uninformed', so is a vowel sound
                        return An();
                    }

                    // uni is pronounced like 'yuni' where 'y' is a consonant and no vowel sound, hence we have to use 'A'
                    return A();
                }

                return An();
            }

            if (text.StartsWithAny("Hh"))
            {
                if (text.StartsWith("hon", StringComparison.OrdinalIgnoreCase))
                {
                    // it is a vowel sound if 'h' is followed by 'on' (as in such case 'h' it is silent, like in 'honest')
                    return An();
                }
            }

            return A();

            string A() => StringExtensions.HasFlag(firstWordHandling, FirstWordHandling.MakeLowerCase) ? "a " : "A ";

            string An() => StringExtensions.HasFlag(firstWordHandling, FirstWordHandling.MakeLowerCase) ? "an " : "An ";
        }
    }
}