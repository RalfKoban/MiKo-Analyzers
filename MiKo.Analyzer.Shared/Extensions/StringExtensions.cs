﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using MiKoSolutions.Analyzers;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

        public static IEnumerable<int> AllIndicesOf(this string value, string finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                yield break;
            }

            if (finding.Length > value.Length)
            {
                yield break;
            }

            for (var index = 0; ; index += finding.Length)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index == -1)
                {
                    // nothing more to find
                    yield break;
                }

                yield return index;
            }
        }

        public static IReadOnlyList<int> AllIndicesOf(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            if (finding.Length > value.Length)
            {
                return Array.Empty<int>();
            }

            var indices = new List<int>();

            for (var index = 0; ; index += finding.Length)
            {
                var newIndex = value.Slice(index).IndexOf(finding, comparison);

                if (newIndex == -1)
                {
                    // nothing more to find
                    break;
                }

                index += newIndex;

                indices.Add(index);
            }

            return indices;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values) where T : class => string.Concat(values.Where(_ => _ != null));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values, string separator) => string.Join(separator, values);

        public static bool Contains(this string value, char c) => value?.IndexOf(c) >= 0;

        public static bool Contains(this ReadOnlySpan<char> value, char c) => value.Length > 0 && value.IndexOf(c) >= 0;

        public static bool Contains(this ReadOnlySpan<char> value, string finding)
        {
            if (finding.Length > value.Length)
            {
                return false;
            }

            return value.IndexOf(finding.AsSpan()) >= 0;
        }

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

        public static bool Contains(this ReadOnlySpan<char> value, string finding, StringComparison comparison)
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

            return value.IndexOf(finding.AsSpan(), comparison) >= 0;
        }

        public static bool Contains(this ReadOnlySpan<char> value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison) => value.Contains(finding.AsSpan(), nextCharValidationCallback, comparison);

        public static bool Contains(this ReadOnlySpan<char> value, ReadOnlySpan<char> finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison)
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
                var newIndex = value.Slice(index).IndexOf(finding, comparison);

                if (newIndex <= -1)
                {
                    return false;
                }

                index += newIndex;

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

        public static bool ContainsAny(this ReadOnlySpan<char> value, char[] characters) => value.Length > 0 && value.IndexOfAny(characters) >= 0;

        public static bool ContainsAny(this string value, string[] phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this ReadOnlySpan<char> value, string[] phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this string value, IEnumerable<string> phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this ReadOnlySpan<char> value, IEnumerable<string> phrases) => value.ContainsAny(phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this string value, string[] phrases, StringComparison comparison) => value.ContainsAny((IEnumerable<string>)phrases, comparison);

        public static bool ContainsAny(this ReadOnlySpan<char> value, string[] phrases, StringComparison comparison) => value.ContainsAny((IEnumerable<string>)phrases, comparison);

        public static bool ContainsAny(this string value, IEnumerable<string> phrases, StringComparison comparison) => string.IsNullOrEmpty(value) is false && phrases.Any(_ => value.Contains(_, comparison));

        public static bool ContainsAny(this ReadOnlySpan<char> value, IEnumerable<string> phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                foreach (var phrase in phrases)
                {
                    if (value.Contains(phrase, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWith(this string value, char character) => string.IsNullOrEmpty(value) is false && value[value.Length - 1] == character;

        public static bool EndsWith(this ReadOnlySpan<char> value, char character) => value.Length > 0 && value[value.Length - 1] == character;

        public static bool EndsWith(this ReadOnlySpan<char> value, string characters) => string.IsNullOrEmpty(characters) is false && value.EndsWith(characters.AsSpan());

        public static bool EndsWith(this ReadOnlySpan<char> value, string characters, StringComparison comparison) => string.IsNullOrEmpty(characters) is false && value.EndsWith(characters.AsSpan(), comparison);

        public static bool EndsWithAny(this string value, string suffixCharacters) => string.IsNullOrEmpty(value) is false && suffixCharacters.Contains(value[value.Length - 1]);

        public static bool EndsWithAny(this string value, char[] suffixCharacters) => string.IsNullOrEmpty(value) is false && suffixCharacters.Contains(value[value.Length - 1]);

        public static bool EndsWithAny(this string value, IEnumerable<string> suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string suffixCharacters)
        {
            if (value.Length > 0)
            {
                var lastChar = value[value.Length - 1];

                for (var index = 0; index < suffixCharacters.Length; index++)
                {
                    if (lastChar == suffixCharacters[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithAny(this ReadOnlySpan<char> value, char[] suffixCharacters)
        {
            if (value.Length > 0)
            {
                var lastChar = value[value.Length - 1];

                for (var index = 0; index < suffixCharacters.Length; index++)
                {
                    if (lastChar == suffixCharacters[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string[] suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this string value, string[] suffixes, StringComparison comparison) => value.EndsWithAny((IEnumerable<string>)suffixes, comparison);

        public static bool EndsWithAny(this string value, IEnumerable<string> suffixes, StringComparison comparison) => string.IsNullOrEmpty(value) is false && suffixes.Any(_ => value.EndsWith(_, comparison));

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string[] suffixes, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                for (var index = 0; index < suffixes.Length; index++)
                {
                    if (value.EndsWith(suffixes[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithCommonNumber(this string value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.OSBitNumbers) is false;

        public static bool EndsWithCommonNumber(this ReadOnlySpan<char> value) => value.EndsWithNumber() && value.EndsWithAny(Constants.Markers.OSBitNumbers) is false;

        public static bool EndsWithNumber(this string value) => string.IsNullOrEmpty(value) is false && value[value.Length - 1].IsNumber();

        public static bool EndsWithNumber(this ReadOnlySpan<char> value) => value.Length > 0 && value[value.Length - 1].IsNumber();

        public static bool Equals(this ReadOnlySpan<char> value, string other, StringComparison comparison) => other != null && value.Equals(other.AsSpan(), comparison);

        public static bool EqualsAny(this string value, IEnumerable<string> phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this ReadOnlySpan<char> value, string[] phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this string value, IEnumerable<string> phrases, StringComparison comparison) => string.IsNullOrEmpty(value) is false && phrases.Any(_ => value.Equals(_, comparison));

        public static bool EqualsAny(this ReadOnlySpan<char> value, string[] phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                for (var index = 0; index < phrases.Length; index++)
                {
                    if (value.Equals(phrases[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string FirstWord(this string value)
        {
            if (value is null)
            {
                return null;
            }

            var span = value.AsSpan();
            var word = FirstWord(span);

            return word != span
                       ? word.ToString()
                       : value;
        }

        public static ReadOnlySpan<char> FirstWord(this ReadOnlySpan<char> value)
        {
            var text = value.TrimStart();

            var firstSpace = text.IndexOfAny(Constants.WhiteSpaceCharacters);

            if (firstSpace != -1)
            {
                // we found a whitespace
                return text.Slice(0, firstSpace);
            }

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            for (var index = 1; index < text.Length; index++)
            {
                var c = text[index];

                if (c.IsUpperCase())
                {
                    var firstWord = text.Slice(0, index);

                    return firstWord;
                }
            }

            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0) => string.Format(format, arg0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1) => string.Format(format, arg0, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1, object arg2) => string.Format(format, arg0, arg1, arg2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1, object arg2, object arg3) => string.Format(format, arg0, arg1, arg2, arg3);

        public static IEnumerable<string> Words(this string text) => Words(text.AsSpan());

        public static IEnumerable<string> Words(this ReadOnlySpan<char> text)
        {
            var words = new List<string>();

            var startIndex = 0;

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            var index = 1;

            for (; index < text.Length; index++)
            {
                var c = text[index];

                if (c.IsUpperCase())
                {
                    var word = text.Slice(startIndex, index - startIndex);

                    startIndex = index;

                    words.Add(word.ToString());
                }
            }

            // return the remaining word
            if (index == text.Length)
            {
                words.Add(text.Slice(startIndex).ToString());
            }

            return words;
        }

        public static string GetNameOnlyPart(this string fullName) => GetNameOnlyPart(fullName.AsSpan());

        public static string GetNameOnlyPart(this ReadOnlySpan<char> fullName)
        {
            var genericIndexStart = fullName.IndexOf('<');
            var genericIndexEnd = fullName.LastIndexOf('>');

            if (genericIndexStart > 0 && genericIndexEnd > 0)
            {
                var namePart = fullName.Slice(0, genericIndexStart).GetPartAfterLastDot().ToString();

                var indexAfterGenericStart = genericIndexStart + 1;
                var genericArguments = fullName.Slice(indexAfterGenericStart, genericIndexEnd - indexAfterGenericStart).ToString();
                var genericNameParts = genericArguments.Split(GenericTypeArgumentSeparator, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.GetPartAfterLastDot());
                var genericPart = string.Join(",", genericNameParts);

                return string.Concat(namePart, "<", genericPart, ">");
            }

            return fullName.GetPartAfterLastDot().ToString();
        }

        public static string GetNameOnlyPartWithoutGeneric(this ReadOnlySpan<char> fullName)
        {
            var genericIndexStart = fullName.IndexOf('<');

            var name = genericIndexStart > 0
                           ? fullName.Slice(0, genericIndexStart)
                           : fullName;

            return name.GetPartAfterLastDot().ToString();
        }

        public static string GetPartAfterLastDot(this string value)
        {
            if (value is null)
            {
                return null;
            }

            return GetPartAfterLastDot(value.AsSpan()).ToString();
        }

        public static ReadOnlySpan<char> GetPartAfterLastDot(this ReadOnlySpan<char> value) => value.Slice(value.LastIndexOf('.') + 1);

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
            var items = values.Select(_ => _.SurroundedWithApostrophe()).ToArray();

            var count = items.Length;

            switch (count)
            {
                case 0: return string.Empty;
                case 1: return items[0];
            }

            const string Separator = ", ";

            var separatorForLast = string.Intern(" " + lastSeparator + " ");

            switch (count)
            {
                case 2: return string.Concat(items[0], separatorForLast, items[1]);
                case 3: return new StringBuilder(items[0]).Append(Separator).Append(items[1]).Append(separatorForLast).Append(items[2]).ToString();
                case 4: return new StringBuilder(items[0]).Append(Separator).Append(items[1]).Append(Separator).Append(items[2]).Append(separatorForLast).Append(items[3]).ToString();
                default: return string.Concat(items.Take(count - 1).ConcatenatedWith(Separator), separatorForLast, items[count - 1]);
            }
        }

        public static string HumanizedTakeFirst(this ReadOnlySpan<char> value, int max)
        {
            var length = Math.Min(max, value.Length);

            if (length <= 0 || length == value.Length)
            {
                return value.TrimEnd().ToString();
            }

            var span = value.Slice(0, length).TrimEnd();
            length = span.Length;

            var chars = new char[length + 3];
            chars[length] = '.';
            chars[length + 1] = '.';
            chars[length + 2] = '.';
            span.CopyTo(chars);

            return new string(chars);
        }

        public static bool IsAcronym(this string value) => string.IsNullOrEmpty(value) is false && value.None(_ => _.IsLowerCaseLetter());

        public static bool IsEntityMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Entities) && symbolName.EndsWithAny(Constants.Markers.ViewModels) is false;

        public static bool IsHyperlink(this string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(text, @"(www|ftp:|ftps:|http:|https:)+[^\s]+[\w]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(this char value) => char.IsLetter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this char value) => char.IsLower(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this char value) => value.IsLetter() && value.IsLowerCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        public static bool IsNullOrWhiteSpace(this ReadOnlySpan<char> value)
        {
            if (value.Length > 0)
            {
                for (var index = 0; index < value.Length; index++)
                {
                    if (value[index].IsWhiteSpace() is false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

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
        public static bool IsUpperCaseLetter(this char value) => value.IsLetter() && value.IsUpperCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value) => char.IsWhiteSpace(value);

        public static string SecondWord(this string text) => string.Intern(text.AsSpan().WithoutFirstWord().FirstWord().ToString());

        public static bool StartsWith(this string value, char character) => string.IsNullOrEmpty(value) is false && value[0] == character;

        public static bool StartsWith(this ReadOnlySpan<char> value, string characters) => string.IsNullOrEmpty(characters) is false && value.StartsWith(characters.AsSpan());

        public static bool StartsWith(this ReadOnlySpan<char> value, string characters, StringComparison comparison) => string.IsNullOrEmpty(characters) is false && value.StartsWith(characters.AsSpan(), comparison);

        public static bool StartsWithAny(this string value, IEnumerable<char> characters) => string.IsNullOrEmpty(value) is false && characters.Contains(value[0]);

        public static bool StartsWithAny(this ReadOnlySpan<char> value, IEnumerable<char> characters) => value.Length > 0 && characters.Contains(value[0]);

        public static bool StartsWithAny(this string value, string[] prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this ReadOnlySpan<char> value, string[] prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this string value, string[] prefixes, StringComparison comparison) => value.StartsWithAny((IEnumerable<string>)prefixes, comparison);

        public static bool StartsWithAny(this string value, IEnumerable<string> prefixes, StringComparison comparison) => string.IsNullOrEmpty(value) is false && prefixes.Any(_ => value.StartsWith(_, comparison));

        public static bool StartsWithAny(this ReadOnlySpan<char> value, string[] prefixes, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                for (var index = 0; index < prefixes.Length; index++)
                {
                    if (value.StartsWith(prefixes[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithNumber(this string value) => string.IsNullOrEmpty(value) is false && value[0].IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, string surrounding) => string.Concat(surrounding, value, surrounding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => value?.SurroundedWith("\'");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithDoubleQuote(this string value) => value?.SurroundedWith("\"");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLowerCase(this string value) => value?.ToLower(CultureInfo.InvariantCulture);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLowerCase(this char value) => char.ToLowerInvariant(value);

        /// <summary>
        /// Gets an interned copy of the <see cref="string"/> where the characters are lower-case.
        /// </summary>
        /// <param name="value">
        /// The original text.
        /// </param>
        /// <returns>
        /// An interned copy of the <see cref="string"/> where the character are lower-case.
        /// </returns>
        public static string ToLowerCase(this ReadOnlySpan<char> value)
        {
            var characters = value.ToArray();

            for (var index = 0; index < characters.Length; index++)
            {
                characters[index] = value[index].ToLowerCase();
            }

            return string.Intern(new string(characters));
        }

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
        public static string ToLowerCaseAt(this ReadOnlySpan<char> value, int index)
        {
            if (index >= value.Length)
            {
                return string.Intern(value.ToString());
            }

            var characters = value.ToArray();
            characters[index] = value[index].ToLowerCase();

            return string.Intern(new string(characters));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static string ToUpperCaseAt(this ReadOnlySpan<char> value, int index)
        {
            if (index >= value.Length)
            {
                return string.Intern(value.ToString());
            }

            var characters = value.ToArray();
            characters[index] = characters[index].ToUpperCase();

            return string.Intern(new string(characters));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        public static string Without(this string value, string[] phrases) => new StringBuilder(value).Without(phrases).ToString();

        public static StringBuilder Without(this StringBuilder value, string phrase) => value.ReplaceWithCheck(phrase, string.Empty);

        public static StringBuilder Without(this StringBuilder value, string[] phrases)
        {
            for (var index = 0; index < phrases.Length; index++)
            {
                value.Without(phrases[index]);
            }

            return value;
        }

        public static string WithoutFirstWord(this string value) => WithoutFirstWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> WithoutFirstWord(this ReadOnlySpan<char> value)
        {
            var text = value.TrimStart();

            var firstSpace = text.IndexOfAny(Constants.WhiteSpaceCharacters);

            if (firstSpace < 0)
            {
                // might happen if the text contains a <see> or some other XML element as second word; therefore we only return a space
                return " ".AsSpan();
            }

            return text.Slice(firstSpace);
        }

        public static string WithoutFirstWords(this string value, params string[] words) => WithoutFirstWords(value.AsSpan(), words).ToString();

        public static ReadOnlySpan<char> WithoutFirstWords(this ReadOnlySpan<char> value, params string[] words)
        {
            var text = value.TrimStart();

            for (var index = 0; index < words.Length; index++)
            {
                var word = words[index];

                if (text.FirstWord().Equals(word, StringComparison.OrdinalIgnoreCase))
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

            var totalLength = value.Length - 1;
            var end = totalLength;

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

            return end >= 0 && end <= totalLength
                       ? value.Substring(0, end)
                       : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WithoutQuotes(this string value) => value.Without(@"""");

        public static ReadOnlySpan<char> WithoutParaTagsAsSpan(this StringBuilder value) => value.WithoutParaTags().ToString().AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder WithoutParaTags(this StringBuilder value) => value.Without(Constants.ParaTags);

        public static IEnumerable<string> WithoutParaTags(this IEnumerable<string> values) => values.Select(_ => new StringBuilder(_).WithoutParaTags().ToString());

        public static string WithoutSuffix(this ReadOnlySpan<char> value, char suffix)
        {
            if (value.EndsWith(suffix))
            {
                var length = value.Length - 1;

                return length <= 0
                           ? string.Empty
                           : value.Slice(0, length).ToString();
            }

            return value.ToString();
        }

        public static string WithoutSuffix(this string value, string suffix)
        {
            if (value is null)
            {
                return null;
            }

            var length = value.Length - suffix.Length;

            return length <= 0
                       ? string.Empty
                       : value.Substring(0, length);
        }
    }
}