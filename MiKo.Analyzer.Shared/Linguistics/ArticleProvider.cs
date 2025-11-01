using System;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Provides functionality to determine the correct English article ('a' or 'an') for a given word based on its pronunciation.
    /// </summary>
    internal static class ArticleProvider
    {
        /// <summary>
        /// Gets the appropriate English article ('a' or 'an') for the specified text.
        /// </summary>
        /// <param name="text">
        /// The text for which to determine the article.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the appropriate article ('a ' or 'an ', 'A ' or 'An ') with a trailing space.
        /// </returns>
        internal static string GetArticleFor(string text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None) => GetArticleFor(text.AsSpan(), firstWordAdjustment);

        /// <summary>
        /// Gets the appropriate English article ('a' or 'an') for the specified text.
        /// </summary>
        /// <param name="text">
        /// The text for which to determine the article.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the appropriate article ('a ' or 'an ', 'A ' or 'An ') with a trailing space, or <see cref="string.Empty"/> if the text is empty.
        /// </returns>
        /// <remarks>
        /// The article selection is based on the phonetic pronunciation of the word, not just the first letter.
        /// Special cases are handled for words like 'hour', 'honest', 'university', 'one', etc.
        /// </remarks>
        internal static string GetArticleFor(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None)
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
                return An(firstWordAdjustment);
            }

            if (text.StartsWithAny("Hh"))
            {
                return ArticleForH(text, firstWordAdjustment);
            }

            if (text.StartsWithAny("Oo"))
            {
                return ArticleForO(text, firstWordAdjustment);
            }

            if (text.StartsWithAny("Ee"))
            {
                return ArticleForE(text, firstWordAdjustment);
            }

            if (text.StartsWithAny("Uu"))
            {
                return ArticleForU(text, firstWordAdjustment);
            }

            return A(firstWordAdjustment);
        }

        /// <summary>
        /// Determines the appropriate article for words starting with 'U'.
        /// </summary>
        /// <param name="text">
        /// The text to analyze.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains 'a ' or 'A ' for words with consonant sounds (for example, 'university', 'use'), otherwise 'an ' or 'An '.
        /// </returns>
        private static string ArticleForU(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment)
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
                            return An(firstWordAdjustment);
                        }
                    }
                }

                // uni is pronounced like 'yuni' where 'y' is a consonant and no vowel sound, hence we have to use 'A'
                return A(firstWordAdjustment);
            }

            if (text.StartsWith("use", StringComparison.OrdinalIgnoreCase))
            {
                // something like 'use case' or 'user'
                return A(firstWordAdjustment);
            }

            return An(firstWordAdjustment);
        }

        /// <summary>
        /// Determines the appropriate article for words starting with 'E'.
        /// </summary>
        /// <param name="text">
        /// The text to analyze.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains 'a ' or 'A ' for words starting with 'eu' (for example, 'European'), otherwise 'an ' or 'An '.
        /// </returns>
        private static string ArticleForE(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment)
        {
            if (text.StartsWith("eu", StringComparison.OrdinalIgnoreCase))
            {
                // Words like 'european' start with a 'y' sound which is a consonant and no vowel sound, hence we have to use 'A'
                return A(firstWordAdjustment);
            }

            return An(firstWordAdjustment);
        }

        /// <summary>
        /// Determines the appropriate article for words starting with 'O'.
        /// </summary>
        /// <param name="text">
        /// The text to analyze.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains 'a ' or 'A ' for words starting with 'on' (for example, 'one', 'once'), otherwise 'an ' or 'An '.
        /// </returns>
        private static string ArticleForO(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment)
        {
            if (text.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            {
                // in English, the words 'one' or 'once' begin with the letter 'o', but they are pronounced with an initial 'w' sound which is a consonant sound, not a vowel sound
                return A(firstWordAdjustment);
            }

            return An(firstWordAdjustment);
        }

        /// <summary>
        /// Determines the appropriate article for words starting with 'H'.
        /// </summary>
        /// <param name="text">
        /// The text to analyze.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains 'an ' or 'An ' for words with silent 'h' (for example, 'hour', 'honest', 'heir', 'herb'), otherwise 'a ' or 'A '.
        /// </returns>
        private static string ArticleForH(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment)
        {
            if (text.StartsWith("herb", StringComparison.OrdinalIgnoreCase) // in American English, 'h' is silent in 'herb', so it starts with a vowel sound
             || text.StartsWith("heir", StringComparison.OrdinalIgnoreCase) // 'h' is silent in 'heir', so it starts with a vowel sound
             || text.StartsWith("hon", StringComparison.OrdinalIgnoreCase) // 'h' it is silent in 'honest', so it starts with a vowel sound
             || text.StartsWith("hou", StringComparison.OrdinalIgnoreCase)) // 'h' is silent in 'hour', so it starts with a vowel sound
            {
                return An(firstWordAdjustment);
            }

            return A(firstWordAdjustment);
        }

        /// <summary>
        /// Gets the article 'a' with the appropriate capitalization.
        /// </summary>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains 'a ' if <see cref="FirstWordAdjustment.StartLowerCase"/> is set, otherwise 'A '.
        /// </returns>
        private static string A(in FirstWordAdjustment adjustment) => adjustment.HasSet(FirstWordAdjustment.StartLowerCase) ? "a " : "A ";

        /// <summary>
        /// Gets the article 'an' with the appropriate capitalization.
        /// </summary>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies how to adjust the capitalization of the article.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains 'an ' if <see cref="FirstWordAdjustment.StartLowerCase"/> is set, otherwise 'An '.
        /// </returns>
        private static string An(in FirstWordAdjustment adjustment) => adjustment.HasSet(FirstWordAdjustment.StartLowerCase) ? "an " : "An ";
    }
}