using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.CSharp;

using MiKoSolutions.Analyzers;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

        public static IEnumerable<int> AllIndexesOf(this string value, string finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            if (finding.Length > value.Length)
            {
                return Array.Empty<int>();
            }

            var indexes = new List<int>();

            for (var index = 0; ; index += finding.Length)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index == -1)
                {
                    // nothing more to find
                    return indexes;
                }

                indexes.Add(index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values) where T : class => string.Concat(values.Where(_ => _ != null));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values, string separator) => string.Join(separator, values);

        public static bool Contains(this string value, char c) => value?.IndexOf(c) >= 0;

        public static bool Contains(this string value, string finding, StringComparison comparison)
        {
            if (finding.Length > value.Length)
            {
                switch (comparison)
                {
                    case StringComparison.Ordinal:
                    case StringComparison.OrdinalIgnoreCase:
                        // cannot be contained as the item is longer than the string to search in
                        return false;
                }
            }

            return value.IndexOf(finding, comparison) >= 0;
        }

        public static bool Contains(this string value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison)
        {
            var index = 0;
            var valueLength = value.Length;
            var findingLength = finding.Length;

            if (findingLength > valueLength)
            {
                switch (comparison)
                {
                    case StringComparison.Ordinal:
                    case StringComparison.OrdinalIgnoreCase:
                        // cannot be contained as the item is longer than the string to search in
                        return false;
                }
            }

            while (true)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index <= -1)
                {
                    return false;
                }

                var positionAfterCharacter = index + findingLength;
                if (positionAfterCharacter >= valueLength)
                {
                    return true;
                }

                var nextChar = value[positionAfterCharacter];
                if (nextCharValidationCallback(nextChar))
                {
                    return true;
                }

                index = positionAfterCharacter;
            }
        }

        public static bool ContainsAny(this string value, char[] characters) => value?.IndexOfAny(characters) >= 0;

        public static bool ContainsAny(this string value, string[] phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this string value, IEnumerable<string> phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this string value, string[] phrases, StringComparison comparison) => value.ContainsAny((IEnumerable<string>)phrases, comparison);

        public static bool ContainsAny(this string value, IEnumerable<string> phrases, StringComparison comparison) => string.IsNullOrEmpty(value) is false && phrases.Any(_ => value.Contains(_, comparison));

        public static bool EndsWith(this string value, char character) => string.IsNullOrEmpty(value) is false && value[value.Length - 1] == character;

        public static bool EndsWithAny(this string value, string suffixCharacters) => suffixCharacters.Any(value.EndsWith);

        public static bool EndsWithAny(this string value, string[] suffixes) => EndsWithAny(value, suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this string value, string[] suffixes, StringComparison comparison) => string.IsNullOrEmpty(value) is false && suffixes.Any(_ => value.EndsWith(_, comparison));

        public static bool EndsWithCommonNumber(this string value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.OSBitNumbers) is false;

        public static bool EndsWithNumber(this string value) => string.IsNullOrEmpty(value) is false && value[value.Length - 1].IsNumber();

        public static bool EqualsAny(this string value, string[] phrases) => string.IsNullOrEmpty(value) is false && EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this string value, string[] phrases, StringComparison comparison) => string.IsNullOrEmpty(value) is false && phrases.Any(_ => value.Equals(_, comparison));

        public static string FirstWord(this string value)
        {
            var text = value.TrimStart();

            var firstSpace = text.IndexOfAny(Constants.WhiteSpaceCharacters);
            if (firstSpace != -1)
            {
                // we found a whitespace
                return text.Substring(0, firstSpace);
            }

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            for (var index = 1; index < text.Length; index++)
            {
                var c = text[index];

                if (c.IsUpperCase())
                {
                    var firstWord = text.Substring(0, index);

                    return firstWord;
                }
            }

            return text;
        }

        public static IEnumerable<string> Words(this string text)
        {
            var startIndex = 0;

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            var index = 1;

            for (; index < text.Length; index++)
            {
                var c = text[index];

                if (c.IsUpperCase())
                {
                    var word = text.Substring(startIndex, index - startIndex);
                    startIndex = index;

                    yield return word;
                }
            }

            // return the remaining word
            if (index == text.Length)
            {
                yield return text.Substring(startIndex);
            }
        }

        public static string GetNameOnlyPart(this string fullName)
        {
            var genericIndexStart = fullName.IndexOf('<');
            var genericIndexEnd = fullName.LastIndexOf('>');
            if (genericIndexStart > 0 && genericIndexEnd > 0)
            {
                var namePart = fullName.Substring(0, genericIndexStart).GetPartAfterLastDot();

                var indexAfterGenericStart = genericIndexStart + 1;
                var genericArguments = fullName.Substring(indexAfterGenericStart, genericIndexEnd - indexAfterGenericStart);
                var genericNameParts = genericArguments.Split(GenericTypeArgumentSeparator, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.GetPartAfterLastDot());
                var genericPart = string.Concat(genericNameParts);

                return string.Concat(namePart, "<", genericPart, ">");
            }

            return fullName.GetPartAfterLastDot();
        }

        public static string GetPartAfterLastDot(this string value) => value?.Substring(value.LastIndexOf('.') + 1);

        public static bool HasCollectionMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Collections);

        public static bool HasEntityMarker(this string symbolName)
        {
            var hasMarker = symbolName.ContainsAny(Constants.Markers.Entities);
            if (hasMarker)
            {
                if (symbolName.ContainsAny(Constants.Markers.ViewModels))
                {
                    return false;
                }

                if (symbolName.ContainsAny(Constants.Markers.SpecialModels))
                {
                    return false;
                }
            }

            return hasMarker;
        }

        public static bool HasUpperCaseLettersAbove(this string value, ushort limit)
        {
            var count = 0;
            for (var index = 0; index < value.Length; index++)
            {
                if (value[index].IsUpperCase())
                {
                    if (count == limit)
                    {
                        return true;
                    }

                    count++;
                }
            }

            return count > limit;
        }

        public static string HumanizedConcatenated(this IEnumerable<string> values, string lastSeparator = "or")
        {
            var items = values.Select(_ => _.SurroundedWithApostrophe()).ToList();

            const string Separator = ", ";

            var separatorForLast = string.Intern(" " + lastSeparator + " ");

            var count = items.Count;
            switch (count)
            {
                case 0: return string.Empty;
                case 1: return items[0];
                case 2: return string.Concat(items[0], separatorForLast, items[1]);
                case 3: return string.Concat(string.Concat(items[0], Separator, items[1]), separatorForLast, items[2]);
                case 4: return string.Concat(string.Concat(items[0], Separator, items[1]), string.Concat(Separator, items[2], separatorForLast, items[3]));
                default: return string.Concat(items.Take(count - 1).ConcatenatedWith(Separator), separatorForLast, items[count - 1]);
            }
        }

        public static string HumanizedTakeFirst(this string value, int max)
        {
            var index = Math.Min(max, value.Length);

            return index <= 0 || index == value.Length
                       ? value
                       : value.Substring(0, index) + "...";
        }

        public static bool IsAcronym(this string value) => string.IsNullOrEmpty(value) is false && value.All(_ => _.IsLowerCaseLetter() is false);

        public static bool IsCSharpKeyword(this string value) => SyntaxFactory.ParseToken(value).IsKeyword();

        public static bool IsEntityMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Entities) && symbolName.EndsWithAny(Constants.Markers.ViewModels) is false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(this char value) => char.IsLetter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this char value) => char.IsLower(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this char value) => value.IsLetter() && value.IsLowerCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this char value) => char.IsNumber(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSentenceEnding(this char value)
        {
            switch (value)
            {
                case '.':
                case '?':
                case '!':
                    return true;

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCase(this char value) => char.IsUpper(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value) => char.IsWhiteSpace(value);

        public static string SecondWord(this string text) => text.WithoutFirstWord().FirstWord();

        public static IEnumerable<string> SplitBy(this string value, string[] findings, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<string>();
            }

            var tuples = findings.SelectMany(finding => value.AllIndexesOf(finding, comparison).Select(index => new Tuple<int, string>(index, finding)))
                                 .ToList();

            var results = new List<string>((tuples.Count * 2) + 1);

            var remainingString = value;

            // get substrings by tuple indices and remember all parts (in reverse order)
            foreach (var (index, finding) in tuples.OrderByDescending(_ => _.Item1))
            {
                var lastPart = remainingString.Substring(index + finding.Length);

                results.Add(lastPart);
                results.Add(finding);

                remainingString = remainingString.Substring(0, index);
            }

            // add first part of string as it would miss otherwise
            results.Add(remainingString);

            // ensure the correct order as the substrings were added in reverse order
            results.Reverse();

            return results;
        }

        public static bool StartsWithAny(this string value, string[] prefixes) => StartsWithAny(value, prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this string value, string[] prefixes, StringComparison comparison) => string.IsNullOrEmpty(value) is false && prefixes.Any(_ => value.StartsWith(_, comparison));

        public static bool StartsWithAnyChar(this string value, string characters)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            var character = value[0];

            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < characters.Length; index++)
            {
                if (character == characters[index])
                {
                    return true;
                }
            }

            return false;
        }

        public static bool StartsWithNumber(this string value) => string.IsNullOrEmpty(value) is false && value[0].IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, string surrounding) => string.Concat(surrounding, value, surrounding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => value?.SurroundedWith("\'");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithDoubleQuote(this string value) => value?.SurroundedWith("\"");

        public static string ToLowerCase(this string value) => value?.ToLower(CultureInfo.InvariantCulture);

        public static char ToLowerCase(this char value) => char.ToLowerInvariant(value);

        /// <summary>
        /// Gets an interned copy of the <see cref="string"/> where the specified character is lower-case.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="value"/> that shall be changed into lower-case.
        /// </param>
        /// <returns>
        /// An interned copy of the <see cref="string"/> where the specified character is lower-case.
        /// </returns>
        public static string ToLowerCaseAt(this string value, int index)
        {
            if (value is null)
            {
                return null;
            }

            if (index >= value.Length)
            {
                return value;
            }

            var characters = value.ToCharArray();
            characters[index] = characters[index].ToLowerCase();

            return string.Intern(new string(characters));
        }

        public static char ToUpperCase(this char value) => char.ToUpperInvariant(value);

        /// <summary>
        /// Gets an interned copy of the <see cref="string"/> where the specified character is upper-case.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="value"/> that shall be changed into upper-case.
        /// </param>
        /// <returns>
        /// An interned copy of the <see cref="string"/> where the specified character is upper-case.
        /// </returns>
        public static string ToUpperCaseAt(this string value, int index)
        {
            if (value is null)
            {
                return null;
            }

            if (index >= value.Length)
            {
                return value;
            }

            var characters = value.ToCharArray();
            characters[index] = characters[index].ToUpperCase();

            return string.Intern(new string(characters));
        }

        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        public static string Without(this string value, string[] values) => values.Aggregate(value, (current, s) => current.Without(s));

        public static string WithoutFirstWord(this string value)
        {
            var text = value.TrimStart();

            var firstSpace = text.IndexOfAny(Constants.WhiteSpaceCharacters);
            if (firstSpace < 0)
            {
                // might happen if the text contains a <see> or some other XML element as second word; therefore we only return a space
                return " ";
            }

            return text.Substring(firstSpace);
        }

        public static string WithoutFirstWords(this string value, params string[] words)
        {
            var text = value.TrimStart();

            foreach (var word in words)
            {
                if (word.Equals(text.FirstWord(), StringComparison.OrdinalIgnoreCase))
                {
                    text = text.WithoutFirstWord().TrimStart();
                }
            }

            return text.TrimStart();
        }

        public static string WithoutNumberSuffix(this string value)
        {
            if (value is null)
            {
                return null;
            }

            var end = value.Length - 1;
            while (end >= 0)
            {
                if (value[end].IsNumber())
                {
                    end--;
                }
                else
                {
                    end++; // fix last character
                    break;
                }
            }

            return end >= 0 && end <= value.Length - 1
                       ? value.Substring(0, end)
                       : value;
        }

        public static string WithoutQuotes(this string value) => value.Without(@"""");

        public static string WithoutParaTags(this string value) => value.Without(Constants.ParaTags);

        public static IEnumerable<string> WithoutParaTags(this IEnumerable<string> values) => values.Select(WithoutParaTags);

        public static string WithoutSuffix(this string value, string suffix)
        {
            if (value is null)
            {
                return null;
            }

            var length = value.Length - suffix.Length;

            return length > 0 ? value.Substring(0, length) : string.Empty;
        }
    }
}