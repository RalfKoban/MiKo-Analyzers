using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers;
using MiKoSolutions.Analyzers.Linguistics;

//// ncrunch: rdi off
// ReSharper disable once CheckNamespace
namespace System
{
    internal static class StringExtensions
    {
        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

        public static string AdjustFirstWord(this string value, FirstWordHandling handling)
        {
            bool HasFlag(FirstWordHandling flag) => (handling & flag) == flag;

            if (value.StartsWith('<'))
            {
                return value;
            }

            var word = value.FirstWord();

            if (HasFlag(FirstWordHandling.MakeLowerCase))
            {
                word = word.ToLowerCaseAt(0);
            }

            if (HasFlag(FirstWordHandling.MakeUpperCase))
            {
                word = word.ToUpperCaseAt(0);
            }

            // build continuation here because the word length may change based on the infinite term
            var continuation = value.AsSpan().TrimStart().Slice(word.Length).ToString();

            if (HasFlag(FirstWordHandling.MakeInfinite))
            {
                word = Verbalizer.MakeInfiniteVerb(word);
            }

            var text = word + continuation;

            if (HasFlag(FirstWordHandling.KeepLeadingSpace))
            {
                // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space and we would fix it in a wrong way)
                if (value.StartsWith(' '))
                {
                    return " " + text;
                }
            }

            return text;
        }

        public static IReadOnlyList<int> AllIndicesOf(this string value, string finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            var valueLength = value.Length;
            var findingLength = finding.Length;

            if (findingLength > valueLength)
            {
                return Array.Empty<int>();
            }

            var indices = new List<int>();

            for (var index = 0; ; index += findingLength)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index == -1)
                {
                    // nothing more to find
                    break;
                }

                indices.Add(index);
            }

            return indices;
        }

        public static IReadOnlyList<int> AllIndicesOf(this ReadOnlySpan<char> value, string finding, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return Array.Empty<int>();
            }

            if (finding.Length > value.Length)
            {
                return Array.Empty<int>();
            }

            if (comparison == StringComparison.Ordinal)
            {
                // performance optimization for 'StringComparison.Ordinal' to avoid multiple strings from being created (see 'IndexOf' method inside 'MemoryExtensions')
                return AllIndicesOrdinal(value, finding.AsSpan());
            }

            return AllIndicesOf(value.ToString(), finding, comparison);

            IReadOnlyList<int> AllIndicesOrdinal(ReadOnlySpan<char> span, ReadOnlySpan<char> other)
            {
                var indices = new List<int>();

                for (var index = 0; ; index += other.Length)
                {
                    var newIndex = span.Slice(index).IndexOf(other, StringComparison.Ordinal);

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
        }

        public static SyntaxToken AsToken(this string source, SyntaxKind kind = SyntaxKind.StringLiteralToken)
        {
            switch (kind)
            {
                case SyntaxKind.IdentifierToken:
                    return SyntaxFactory.Identifier(source);

                default:
                    return SyntaxFactory.Token(default, kind, source, source, default);
            }
        }

        public static InterpolatedStringTextSyntax AsInterpolatedString(this ReadOnlySpan<char> value) => value.ToString().AsInterpolatedString();

        public static InterpolatedStringTextSyntax AsInterpolatedString(this string value) => SyntaxFactory.InterpolatedStringText(value.AsToken(SyntaxKind.InterpolatedStringTextToken));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values) => string.Concat(values.Where(_ => _ != null));

        public static StringBuilder ConcatenatedWith<T>(this IEnumerable<T> values) where T : class
        {
            var builder = new StringBuilder();

            foreach (var value in values)
            {
                if (value != null)
                {
                    builder.Append(value);
                }
            }

            return builder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith(this IEnumerable<string> values, string separator) => string.Join(separator, values);

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
            var valueLength = value.Length;
            var findingLength = finding.Length;

            var difference = findingLength - valueLength;

            if (difference == 0)
            {
                return QuickEquals();

                bool QuickEquals()
                {
                    const int QuickInspectionChars = 2;

                    if (valueLength > QuickInspectionChars)
                    {
                        var valueSpan = value.AsSpan(valueLength - QuickInspectionChars, QuickInspectionChars);
                        var findingSpan = finding.AsSpan(findingLength - QuickInspectionChars, QuickInspectionChars);

                        if (valueSpan.CompareTo(findingSpan, comparison) != 0)
                        {
                            return false;
                        }
                    }

                    return value.Equals(finding, comparison);
                }
            }

            if (difference > 0)
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

        public static bool ContainsAny(this string value, string[] phrases, StringComparison comparison)
        {
//// ncrunch: no coverage start

            if (value.HasCharacters())
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var index = 0; index < phrases.Length; index++)
                {
                    if (value.Contains(phrases[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;

//// ncrunch: no coverage end
        }

        public static bool ContainsAny(this ReadOnlySpan<char> value, string[] phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // use 'ToString' here for performance reasons
                // because 'IndexOf' on 'ReadOnlySpan<char>' converts the text into a string anyway
                return value.ToString().ContainsAny(phrases, comparison);
            }

            return false;
        }

        public static bool ContainsAny(this string value, IEnumerable<string> phrases, StringComparison comparison)
        {
            if (phrases is string[] array)
            {
                return value.ContainsAny(array, comparison);
            }

            if (value.HasCharacters())
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

        public static bool ContainsAny(this ReadOnlySpan<char> value, IEnumerable<string> phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // use 'ToString' here for performance reasons
                // because 'IndexOf' on 'ReadOnlySpan<char>' converts the text into a string anyway
                return value.ToString().ContainsAny(phrases, comparison);
            }

            return false;
        }

        public static bool EndsWith(this string value, char character) => value.HasCharacters() && value[value.Length - 1] == character;

        public static bool EndsWith(this ReadOnlySpan<char> value, char character) => value.Length > 0 && value[value.Length - 1] == character;

        public static bool EndsWith(this ReadOnlySpan<char> value, string characters) => characters.HasCharacters() && value.EndsWith(characters.AsSpan());

        public static bool EndsWith(this ReadOnlySpan<char> value, string characters, StringComparison comparison) => characters.HasCharacters() && value.EndsWith(characters.AsSpan(), comparison);

        public static bool EndsWithAny(this string value, string suffixCharacters) => value.HasCharacters() && suffixCharacters.Contains(value[value.Length - 1]);

        public static bool EndsWithAny(this string value, char[] suffixCharacters) => value.HasCharacters() && suffixCharacters.Contains(value[value.Length - 1]);

        public static bool EndsWithAny(this string value, IEnumerable<string> suffixes) => value.EndsWithAny(suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string suffixCharacters)
        {
            if (value.Length > 0)
            {
                var lastChar = value[value.Length - 1];

                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
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

                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
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

        public static bool EndsWithAny(this string value, string[] suffixes, StringComparison comparison)
        {
//// ncrunch: no coverage start

            if (value.HasCharacters())
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var index = 0; index < suffixes.Length; index++)
                {
                    if (value.EndsWith(suffixes[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;

//// ncrunch: no coverage end
        }

        public static bool EndsWithAny(this string value, IEnumerable<string> suffixes, StringComparison comparison)
        {
            if (suffixes is string[] array)
            {
                return value.EndsWithAny(array, comparison);
            }

            if (value.HasCharacters())
            {
                foreach (var suffix in suffixes)
                {
                    if (value.EndsWith(suffix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool EndsWithAny(this ReadOnlySpan<char> value, string[] suffixes, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
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

        public static bool EndsWithNumber(this string value) => value.HasCharacters() && value[value.Length - 1].IsNumber();

        public static bool EndsWithNumber(this ReadOnlySpan<char> value) => value.Length > 0 && value[value.Length - 1].IsNumber();

        public static bool Equals(this string value, ReadOnlySpan<char> other, StringComparison comparison) => value != null && value.AsSpan().Equals(other, comparison);

        public static bool Equals(this ReadOnlySpan<char> value, string other, StringComparison comparison) => other != null && value.Equals(other.AsSpan(), comparison);

        public static bool EqualsAny(this string value, IEnumerable<string> phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this ReadOnlySpan<char> value, string[] phrases) => EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
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

        public static bool EqualsAny(this string value, IEnumerable<string> phrases, StringComparison comparison)
        {
            if (phrases is string[] array)
            {
                return value.EqualsAny(array, comparison);
            }

            if (value.HasCharacters())
            {
                foreach (var phrase in phrases)
                {
                    if (value.Equals(phrase, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

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

        public static string SecondWord(this string value) => SecondWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> SecondWord(this ReadOnlySpan<char> value) => value.WithoutFirstWord().FirstWord();

        public static string ThirdWord(this string value) => ThirdWord(value.AsSpan()).ToString();

        public static ReadOnlySpan<char> ThirdWord(this ReadOnlySpan<char> value) => value.WithoutFirstWord().WithoutFirstWord().FirstWord();

        public static string LastWord(this string value)
        {
            if (value is null)
            {
                return null;
            }

            var span = value.AsSpan();
            var word = LastWord(span);

            return word != span
                   ? word.ToString()
                   : value;
        }

        public static ReadOnlySpan<char> LastWord(this ReadOnlySpan<char> value)
        {
            var text = value.TrimEnd();

            var lastSpace = text.LastIndexOfAny(Constants.WhiteSpaceCharacters) + 1;

            if (lastSpace <= 0)
            {
                return text;
            }

            return text.Slice(lastSpace);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, char arg0) => string.Format(format, arg0.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0) => string.Format(format, arg0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1) => string.Format(format, arg0, arg1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, string arg0, char arg1) => string.Format(format, arg0, arg1.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1, object arg2) => string.Format(format, arg0, arg1, arg2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, object arg0, object arg1, object arg2, object arg3) => string.Format(format, arg0, arg1, arg2, arg3);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatWith(this string format, char arg0, char arg1, char arg2, char arg3) => string.Format(format, arg0.ToString(), arg1.ToString(), arg2.ToString(), arg3.ToString());

        public static string GetNameOnlyPart(this string value) => GetNameOnlyPart(value.AsSpan());

        public static string GetNameOnlyPart(this ReadOnlySpan<char> value)
        {
            var genericIndexStart = value.IndexOf('<');
            var genericIndexEnd = value.LastIndexOf('>');

            if (genericIndexStart > 0 && genericIndexEnd > 0)
            {
                var indexAfterGenericStart = genericIndexStart + 1;

                var namePart = value.Slice(0, genericIndexStart).GetPartAfterLastDot().ToString();
                var genericParts = value.Slice(indexAfterGenericStart, genericIndexEnd - indexAfterGenericStart)
                                        .SplitBy(GenericTypeArgumentSeparator, StringSplitOptions.RemoveEmptyEntries);
                var count = genericParts.Count();

                if (count > 0)
                {
                    var i = 0;

                    var genericNameParts = new string[count];

                    foreach (ReadOnlySpan<char> part in genericParts)
                    {
                        genericNameParts[i++] = part.GetPartAfterLastDot().ToString();
                    }

                    var genericPart = string.Join(",", genericNameParts);

                    return string.Concat(namePart, "<", genericPart, ">");
                }
            }

            return value.GetPartAfterLastDot().ToString();
        }

        public static string GetNameOnlyPartWithoutGeneric(this ReadOnlySpan<char> value)
        {
            var genericIndexStart = value.IndexOf('<');

            var name = genericIndexStart > 0
                       ? value.Slice(0, genericIndexStart)
                       : value;

            return name.GetPartAfterLastDot().ToString();
        }

        public static string GetPartAfterLastDot(this string value)
        {
            if (value.HasCharacters())
            {
                return GetPartAfterLastDot(value.AsSpan()).ToString();
            }

            return null;
        }

        public static ReadOnlySpan<char> GetPartAfterLastDot(this ReadOnlySpan<char> value) => value.Slice(value.LastIndexOf('.') + 1);

        public static bool HasCollectionMarker(this string value) => value.EndsWithAny(Constants.Markers.Collections);

        public static bool HasEntityMarker(this string value)
        {
            var hasMarker = value.ContainsAny(Constants.Markers.Models);

            if (hasMarker)
            {
                if (value.ContainsAny(Constants.Markers.ViewModels))
                {
                    return false;
                }

                if (value.ContainsAny(Constants.Markers.SpecialModels))
                {
                    return false;
                }
            }

            return hasMarker;
        }

        public static bool HasCharacters(this string value) => string.IsNullOrEmpty(value) is false;

        public static bool HasUpperCaseLettersAbove(this string value, ushort limit) => value != null && HasUpperCaseLettersAbove(value.AsSpan(), limit);

        public static bool HasUpperCaseLettersAbove(this ReadOnlySpan<char> value, ushort limit)
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

            var separatorForLast = " " + lastSeparator + " ";

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

        public static int IndexOfAny(this ReadOnlySpan<char> value, string[] phrases, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // performance optimization to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see implementation inside MemoryExtensions)
                if (comparison == StringComparison.Ordinal)
                {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < phrases.Length; i++)
                    {
                        var phrase = phrases[i];

                        var index = value.IndexOf(phrase.AsSpan(), comparison);

                        if (index > -1)
                        {
                            return index;
                        }
                    }
                }
                else
                {
                    // use string here to avoid unnecessary 'ToString' calls on 'ReadOnlySpan' (see implementation inside MemoryExtensions)
                    return value.ToString().IndexOfAny(phrases, comparison);
                }
            }

            return -1;
        }

        public static int IndexOfAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value.HasCharacters())
            {
                if (comparison == StringComparison.Ordinal)
                {
                    return IndexOfAny(value.AsSpan(), phrases, comparison);
                }

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < phrases.Length; i++)
                {
                    var phrase = phrases[i];

                    var index = value.IndexOf(phrase, comparison);

                    if (index > -1)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        public static int LastIndexOfAny(this string value, string[] phrases, StringComparison comparison)
        {
            if (value is null)
            {
                return -1;
            }

            if (value.Length > 0)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < phrases.Length; i++)
                {
                    var phrase = phrases[i];

                    var index = value.LastIndexOf(phrase, comparison);

                    if (index > -1)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        public static bool IsAcronym(this string value) => value.HasCharacters() && value.None(_ => _.IsLowerCaseLetter());

        public static bool IsHyperlink(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(value, @"(www|ftp:|ftps:|http:|https:)+[^\s]+[\w]", RegexOptions.Compiled, 100.Milliseconds());
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(this char value) => char.IsLetter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this char value) => char.IsLower(value); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this char value) => value.IsLetter() && value.IsLowerCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value); // ncrunch: no coverage

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

        public static bool IsPascalCasing(this string value)
        {
            try
            {
                return Regex.IsMatch(value, "[a-z]+[A-Z]+", RegexOptions.Compiled, 100.Milliseconds());
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

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
        public static bool IsUpperCase(this char value) => char.IsUpper(value); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCaseLetter(this char value) => value.IsLetter() && value.IsUpperCase();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value) => char.IsWhiteSpace(value); // ncrunch: no coverage

        public static bool StartsWith(this string value, char character) => value.HasCharacters() && value[0] == character; // ncrunch: no coverage

        public static bool StartsWith(this ReadOnlySpan<char> value, char character) => value.Length > 0 && value[0] == character; // ncrunch: no coverage

        public static bool StartsWith(this ReadOnlySpan<char> value, string characters) => characters.HasCharacters() && value.StartsWith(characters.AsSpan());

        public static bool StartsWith(this ReadOnlySpan<char> value, string characters, StringComparison comparison) => characters.HasCharacters() && value.StartsWith(characters.AsSpan(), comparison);

        public static bool StartsWithAny(this string value, IEnumerable<char> characters) => value.HasCharacters() && characters.Contains(value[0]);

        public static bool StartsWithAny(this ReadOnlySpan<char> value, IEnumerable<char> characters) => value.Length > 0 && characters.Contains(value[0]);

        public static bool StartsWithAny(this string value, string[] prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this ReadOnlySpan<char> value, string[] prefixes) => value.StartsWithAny(prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this string value, string[] prefixes, StringComparison comparison)
        {
//// ncrunch: no coverage start

            if (value.HasCharacters())
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var index = 0; index < prefixes.Length; index++)
                {
                    if (value.StartsWith(prefixes[index], comparison))
                    {
                        return true;
                    }
                }
            }

            return false;

//// ncrunch: no coverage end
        }

        public static bool StartsWithAny(this string value, IEnumerable<string> prefixes, StringComparison comparison)
        {
            if (prefixes is string[] array)
            {
                return value.StartsWithAny(array, comparison);
            }

            if (value.HasCharacters())
            {
                foreach (var prefix in prefixes)
                {
                    if (value.StartsWith(prefix, comparison))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool StartsWithAny(this ReadOnlySpan<char> value, string[] prefixes, StringComparison comparison)
        {
            if (value.Length > 0)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                // ReSharper disable once LoopCanBeConvertedToQuery
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
        public static bool StartsWithNumber(this string value) => value.HasCharacters() && value[0].IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, char surrounding) => value?.SurroundedWith(surrounding.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, string surrounding) => string.Concat(surrounding, value, surrounding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => value?.SurroundedWith("\'");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithDoubleQuote(this string value) => value?.SurroundedWith("\"");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToLowerCase(this string source) => source?.ToLower(CultureInfo.InvariantCulture);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToLowerCase(this char source) => char.ToLowerInvariant(source);

        /// <summary>
        /// Gets a <see cref="string"/> where the characters are lower-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the character are lower-case.
        /// </returns>
        public static string ToLowerCase(this ReadOnlySpan<char> source)
        {
            var characters = source.ToArray();

            for (var index = 0; index < characters.Length; index++)
            {
                characters[index] = source[index].ToLowerCase();
            }

            return new string(characters);
        }

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is lower-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into lower-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is lower-case.
        /// </returns>
        public static string ToLowerCaseAt(this string source, int index)
        {
            if (source is null)
            {
                return null;
            }

            if (index >= source.Length)
            {
                return source;
            }

            var character = source[index];

            if (character.IsLowerCase())
            {
                return source;
            }

            return MakeLowerCaseAt(source, index);
        }

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is lower-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into lower-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is lower-case.
        /// </returns>
        public static string ToLowerCaseAt(this ReadOnlySpan<char> source, int index)
        {
            if (index >= source.Length)
            {
                return source.ToString();
            }

            var character = source[index];

            if (character.IsLowerCase())
            {
                return source.ToString();
            }

            return MakeLowerCaseAt(source, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ToUpperCase(this char source) => char.ToUpperInvariant(source);

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is upper-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into upper-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is upper-case.
        /// </returns>
        public static string ToUpperCaseAt(this string source, int index)
        {
            if (source is null)
            {
                return null;
            }

            if (index >= source.Length)
            {
                return source;
            }

            var character = source[index];

            if (character.IsUpperCase())
            {
                return source;
            }

            return MakeUpperCaseAt(source, index);
        }

        /// <summary>
        /// Gets a <see cref="string"/> where the specified character is upper-case.
        /// </summary>
        /// <param name="source">
        /// The original text.
        /// </param>
        /// <param name="index">
        /// The zero-based index inside <paramref name="source"/> that shall be changed into upper-case.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> where the specified character at <paramref name="index"/> is upper-case.
        /// </returns>
        public static string ToUpperCaseAt(this ReadOnlySpan<char> source, int index)
        {
            if (index >= source.Length)
            {
                return source.ToString();
            }

            var character = source[index];

            if (character.IsUpperCase())
            {
                return source.ToString();
            }

            return MakeUpperCaseAt(source, index);
        }

        /// <summary>
        /// Encapsulates the given term with a space or parenthesis before and a delimiter character behind.
        /// </summary>
        /// <param name="value">
        /// The term to place a space or parenthesis before and a delimiter character behind each single item.
        /// </param>
        /// <returns>
        /// An array of encapsulated terms.
        /// </returns>
        public static string[] WithDelimiters(this string value) => WithDelimiters(new[] { value });

        /// <summary>
        /// Encapsulates the given terms with a space or parenthesis before and a delimiter character behind.
        /// </summary>
        /// <param name="values">
        /// The terms to place a space or parenthesis before and a delimiter character behind each single item.
        /// </param>
        /// <returns>
        /// An array of encapsulated terms.
        /// </returns>
        public static string[] WithDelimiters(this string[] values)
        {
            var result = new List<string>();

            foreach (var delimiter in Constants.Comments.Delimiters)
            {
                foreach (var phrase in values)
                {
                    result.Add(' ' + phrase + delimiter);
                    result.Add('(' + phrase + delimiter);
                }
            }

            return result.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, char character) => value.Without(character.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        public static string Without(this string value, string[] phrases) => new StringBuilder(value).Without(phrases).Trim();

        public static StringBuilder Without(this StringBuilder value, string phrase) => value.ReplaceWithCheck(phrase, string.Empty);

        public static StringBuilder Without(this StringBuilder value, string[] phrases) => value.ReplaceAllWithCheck(phrases, string.Empty);

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

            // ReSharper disable once ForCanBeConvertedToForeach
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder WithoutParaTags(this StringBuilder value) => value.Without(Constants.ParaTags);

        public static IEnumerable<StringBuilder> WithoutParaTags(this IEnumerable<string> values) => values.Select(_ => new StringBuilder(_).WithoutParaTags());

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

        public static ReadOnlySpan<char> WithoutSuffix(this ReadOnlySpan<char> value, string suffix)
        {
            if (suffix != null && value.EndsWith(suffix))
            {
                var length = value.Length - suffix.Length;

                return length > 0
                       ? value.Slice(0, length)
                       : ReadOnlySpan<char>.Empty;
            }

            return value;
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

        public static ReadOnlySpan<char> WithoutSuffixes(this ReadOnlySpan<char> value, string[] suffixes)
        {
            return RemoveSuffixes(RemoveSuffixes(value)); // do it twice to remove consecutive suffixes

            ReadOnlySpan<char> RemoveSuffixes(ReadOnlySpan<char> slice)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < suffixes.Length; index++)
                {
                    var suffix = suffixes[index];

                    if (suffix != null && slice.EndsWith(suffix))
                    {
                        var length = slice.Length - suffix.Length;

                        if (length <= 0)
                        {
                            return ReadOnlySpan<char>.Empty;
                        }

                        slice = slice.Slice(0, length);
                    }
                }

                return slice;
            }
        }

        public static WordsReadOnlySpanEnumerator WordsAsSpan(this ReadOnlySpan<char> value) => new WordsReadOnlySpanEnumerator(value);

        private static string MakeUpperCaseAt(string source, int index)
        {
//// ncrunch: no coverage start
            var characters = source.ToCharArray();
            characters[index] = characters[index].ToUpperCase();

            return new string(characters);
//// ncrunch: no coverage end
        }

        private static string MakeUpperCaseAt(ReadOnlySpan<char> source, int index)
        {
//// ncrunch: no coverage start
            var characters = source.ToArray();
            characters[index] = characters[index].ToUpperCase();

            return new string(characters);
//// ncrunch: no coverage end
        }

        private static string MakeLowerCaseAt(string source, int index)
        {
//// ncrunch: no coverage start
            var characters = source.ToCharArray();
            characters[index] = characters[index].ToLowerCase();

            return new string(characters);
//// ncrunch: no coverage end
        }

        private static string MakeLowerCaseAt(ReadOnlySpan<char> source, int index)
        {
//// ncrunch: no coverage start
            var characters = source.ToArray();
            characters[index] = source[index].ToLowerCase();

            return new string(characters);
//// ncrunch: no coverage end
        }
    }
}