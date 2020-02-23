using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        private static readonly char[] GenericTypeArgumentSeparator = { ',' };

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

        public static bool Contains(this string value, string finding, StringComparison comparison) => value.IndexOf(finding, comparison) >= 0;

        public static bool Contains(this string value, string finding, Func<char, bool> nextCharValidationCallback, StringComparison comparison)
        {
            var index = 0;
            var valueLength = value.Length;
            var findingLength = finding.Length;

            while (true)
            {
                index = value.IndexOf(finding, index, comparison);

                if (index <= -1)
                {
                    break;
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

            return false;
        }

        public static bool ContainsAny(this string value, string[] phrases) => ContainsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsAny(this string value, string[] phrases, StringComparison comparison) => string.IsNullOrEmpty(value) is false && phrases.Any(_ => value.Contains(_, comparison));

        public static bool EqualsAny(this string value, string[] phrases) => string.IsNullOrEmpty(value) is false && EqualsAny(value, phrases, StringComparison.OrdinalIgnoreCase);

        public static bool EqualsAny(this string value, string[] phrases, StringComparison comparison) => string.IsNullOrEmpty(value) is false && phrases.Any(_ => value.Equals(_, comparison));

        public static bool StartsWithAny(this string value, string[] prefixes) => StartsWithAny(value, prefixes, StringComparison.OrdinalIgnoreCase);

        public static bool StartsWithAny(this string value, string[] prefixes, StringComparison comparison) => string.IsNullOrEmpty(value) is false && prefixes.Any(_ => value.StartsWith(_, comparison));

        public static bool EndsWithAny(this string value, string[] suffixes) => EndsWithAny(value, suffixes, StringComparison.OrdinalIgnoreCase);

        public static bool EndsWithAny(this string value, string[] suffixes, StringComparison comparison) => string.IsNullOrEmpty(value) is false && suffixes.Any(_ => value.EndsWith(_, comparison));

        public static bool EndsWithNumber(this string value) => string.IsNullOrEmpty(value) is false && value.Last().IsNumber();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpace(this char value) => char.IsWhiteSpace(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUpperCase(this char value) => char.IsUpper(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCase(this char value) => char.IsLower(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this char value) => char.IsNumber(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLetter(this char value) => char.IsLetter(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLowerCaseLetter(this char value) => value.IsLetter() && value.IsLowerCase();

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
        public static string ConcatenatedWith<T>(this IEnumerable<T> values) where T : class => string.Concat(values.Where(_ => _ != null));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcatenatedWith<T>(this IEnumerable<T> values, string separator) => string.Join(separator, values);

        public static string HumanizedConcatenated(this IEnumerable<string> values)
        {
            var items = values.Select(_ => _.SurroundedWithApostrophe()).ToList();

            const string Separator = ", ";
            const string SeparatorForLast = " or ";

            var count = items.Count;
            switch (count)
            {
                case 0: return string.Empty;
                case 1: return items[0];
                case 2: return items.ConcatenatedWith(SeparatorForLast);
                default: return string.Concat(items.Take(count - 1).ConcatenatedWith(Separator), SeparatorForLast, items.Last());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWith(this string value, string surrounding) => string.Concat(surrounding, value, surrounding);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SurroundedWithApostrophe(this string value) => string.Concat("\'", value, "\'");

        public static string HumanizedTakeFirst(this string value, int max)
        {
            var index = Math.Min(max, value.Length);
            return index <= 0 || index == value.Length
                       ? value
                       : value.Substring(0, index) + "...";
        }

        public static bool IsEntityMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Entities) && symbolName.EndsWithAny(Constants.Markers.ViewModels) is false;

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

        public static bool HasCollectionMarker(this string symbolName) => symbolName.EndsWithAny(Constants.Markers.Collections);

        public static string WithoutParaTags(this string value) => value.Without(Constants.ParaTags);

        public static IEnumerable<string> WithoutParaTags(this IEnumerable<string> values) => values.Select(WithoutParaTags);

        public static string Without(this string value, string phrase) => value.Replace(phrase, string.Empty);

        public static string Without(this string value, string[] values) => values.Aggregate(value, (current, s) => current.Without(s));

        public static string WithoutSuffix(this string value, string suffix)
        {
            if (value is null)
            {
                return null;
            }

            var length = value.Length - suffix.Length;
            return length > 0 ? value.Substring(0, length) : string.Empty;
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

            return end < value.Length - 1
                       ? value.Substring(0, end)
                       : value;
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

        public static HashSet<string> ToHashSet(this IEnumerable<string> source) => new HashSet<string>(source);

        public static bool HasUpperCaseLettersAbove(this string value, ushort limit)
        {
            var count = 0;
            for (var index = 0; index < value.Length; index++)
            {
                if (char.IsUpper(value[index]))
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
    }
}