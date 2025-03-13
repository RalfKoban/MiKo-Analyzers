﻿using System.Buffers;
using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers;
using MiKoSolutions.Analyzers.Linguistics;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        private const int QuickCompareLengthThreshold = 4;
        private const int QuickCompareRentLengthThreshold = 22;

        public static bool IsNullOrWhiteSpace(this StringBuilder value) => value is null || value.CountLeadingWhitespaces() == value.Length;

        public static StringBuilder AdjustFirstWord(this StringBuilder value, FirstWordHandling handling)
        {
            if (value is null)
            {
                return null;
            }

            var valueLength = value.Length;

            if (valueLength == 0)
            {
                return value;
            }

            if (value[0] == '<')
            {
                return value;
            }

            var spacesToKeep = StringExtensions.HasFlag(handling, FirstWordHandling.KeepLeadingSpace) ? 1 : 0;

            // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
            TrimLeadingSpacesTo(spacesToKeep);

            if (StringExtensions.HasFlag(handling, FirstWordHandling.MakeLowerCase))
            {
                MakeLowerCase();
            }
            else if (StringExtensions.HasFlag(handling, FirstWordHandling.MakeUpperCase))
            {
                MakeUpperCase();
            }

            if (StringExtensions.HasFlag(handling, FirstWordHandling.MakeInfinite))
            {
                MakeInfinite();
            }

            if (StringExtensions.HasFlag(handling, FirstWordHandling.MakePlural))
            {
                MakePlural();
            }

            if (StringExtensions.HasFlag(handling, FirstWordHandling.MakeThirdPersonSingular))
            {
                MakeThirdPersonSingular();
            }

            return value;

            void MakeLowerCase()
            {
                for (var i = 0; i < valueLength; i++)
                {
                    var c = value[i];

                    if (c.IsWhiteSpace())
                    {
                        continue;
                    }

                    if (c.IsUpperCase())
                    {
                        value[i] = c.ToLowerCase();
                    }

                    // we found it, so nothing more to do
                    return;
                }
            }

            void MakeUpperCase()
            {
                for (var i = 0; i < valueLength; i++)
                {
                    var c = value[i];

                    if (c.IsWhiteSpace())
                    {
                        continue;
                    }

                    if (c.IsLowerCase())
                    {
                        value[i] = c.ToUpperCase();
                    }

                    // we found it, so nothing more to do
                    return;
                }
            }

            void MakeInfinite()
            {
                var word = FirstWord(value, out var whitespacesBefore);

                value.Remove(whitespacesBefore, word.Length);

                var infiniteWord = Verbalizer.MakeInfiniteVerb(word);

                value.Insert(whitespacesBefore, infiniteWord);
            }

            void MakePlural()
            {
                var word = FirstWord(value, out var whitespacesBefore);

                value.Remove(whitespacesBefore, word.Length);

                var pluralWord = Pluralizer.MakePluralName(word);

                value.Insert(whitespacesBefore, pluralWord);
            }

            void MakeThirdPersonSingular()
            {
                var word = FirstWord(value, out var whitespacesBefore);

                value.Remove(whitespacesBefore, word.Length);

                var pluralWord = Verbalizer.MakeThirdPersonSingularVerb(word);

                value.Insert(whitespacesBefore, pluralWord);
            }

            void TrimLeadingSpacesTo(int count)
            {
                if (value[0] == ' ')
                {
                    var whitespaces = value.CountLeadingWhitespaces();

                    if (whitespaces > count)
                    {
                        value.Remove(count, whitespaces - count);

                        valueLength = value.Length;
                    }
                }
            }
        }

        public static bool EndsWith(this StringBuilder value, string ending, StringComparison comparison = StringComparison.Ordinal)
        {
            if (ending is null)
            {
                return false;
            }

            var valueLength = value.Length;
            var endingLength = ending.Length;

            if (valueLength >= endingLength)
            {
                var end = value.ToString(valueLength - endingLength, endingLength);

                return end.Equals(ending, comparison);
            }

            return false;
        }

        public static string FirstWord(this StringBuilder value, out int whitespacesBefore)
        {
            if (value is null)
            {
                whitespacesBefore = 0;

                return string.Empty;
            }

            var length = value.Length;

            // 1. Find word begin
            whitespacesBefore = value.CountLeadingWhitespaces();

            // 2. Find word end
            var wordLength = 0;

            for (var i = whitespacesBefore; i < length; i++)
            {
                var c = value[i];

                if (c.IsWhiteSpace() || c.IsSentenceEnding())
                {
                    break;
                }

                wordLength++;
            }

            // 3. Cut word
            return value.ToString(whitespacesBefore, wordLength);
        }

        public static bool HasWhitespaces(this StringBuilder value, int start = 0)
        {
            if (value is null)
            {
                return false;
            }

            var valueLength = value.Length;

            for (; start < valueLength; start++)
            {
                if (value[start].IsWhiteSpace())
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this StringBuilder value) => value is null || value.Length == 0; // ncrunch: no coverage

        public static bool IsSingleWord(this StringBuilder value) => value?.HasWhitespaces() is false;

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, ReadOnlySpan<Pair> replacementPairs)
        {
            var count = replacementPairs.Length;

            for (var index = 0; index < count; index++)
            {
                var pair = replacementPairs[index];
                var oldValue = pair.Key;

                if (oldValue.IsNullOrEmpty())
                {
                    // cannot replace any empty value
                    continue;
                }

                if (QuickCompare(ref value, ref oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, pair.Value);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, string[] texts, string replacement)
        {
            var length = texts.Length;

            for (var index = 0; index < length; index++)
            {
                var oldValue = texts[index];

                if (oldValue.IsNullOrEmpty())
                {
                    // cannot replace any empty value
                    continue;
                }

                if (QuickCompare(ref value, ref oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, replacement);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceWithCheck(this StringBuilder value, string oldValue, string newValue)
        {
            if (oldValue.IsNullOrEmpty())
            {
                // cannot replace an empty value
                return value;
            }

            if (QuickCompare(ref value, ref oldValue))
            {
                return value.Replace(oldValue, newValue);
            }

            // cannot be part in the replacement as value does not fit
            return value;
        }

        public static StringBuilder SeparateWords(this StringBuilder value, char separator, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            if (value.IsNullOrEmpty())
            {
                return value;
            }

            var multipleUpperCases = false;

            const int CharacterToStartWith = 1;

            for (var index = CharacterToStartWith; index < value.Length; index++)
            {
                var c = value[index];

                if (c == separator)
                {
                    // keep the existing separator
                    continue;
                }

                var isUpperCaseC = c.IsUpperCase();

                if (isUpperCaseC || c.IsNumber())
                {
                    if (index == CharacterToStartWith)
                    {
                        // multiple upper cases in a line at beginning of the name, so do not flip
                        multipleUpperCases = true;
                    }

                    var previousC = value[index - 1];

                    if (multipleUpperCases)
                    {
                        // let's see if we start with an IXyz interface
                        if (previousC == 'I')
                        {
                            // seems we are in an IXyz interface
                            multipleUpperCases = false;

                            continue;
                        }

                        if (isUpperCaseC && previousC.IsNumber())
                        {
                            // nothing to do here
                        }
                        else
                        {
                            // multiple upper cases in a line, so do not flip
                            continue;
                        }
                    }

                    // let's consider an upper-case 'A' as a special situation as that is a single word
                    var isSpecialCharA = c == 'A';

                    multipleUpperCases = isSpecialCharA is false;

                    var nextC = c.ToLowerCase();

                    var nextIndex = index + 1;

                    if ((nextIndex >= value.Length || (nextIndex < value.Length && value[nextIndex].IsUpperCaseLetter())) && isSpecialCharA is false)
                    {
                        // multiple upper cases in a line, so do not flip
                        nextC = c;
                    }

                    if (previousC == separator)
                    {
                        value[index] = nextC;
                    }
                    else
                    {
                        // only add an underline if we not already have one
                        value[index] = separator;
                        index++;

                        value.Insert(index, nextC);
                    }
                }
                else
                {
                    if (multipleUpperCases)
                    {
                        if (value[index - 1].IsUpperCaseLetter())
                        {
                            // we are behind multiple upper cases in a line, so add an underline
                            value[index] = separator;
                            index++;

                            value.Insert(index, c);
                        }
                    }

                    multipleUpperCases = false;
                }
            }

            return value.AdjustFirstWord(firstWordHandling);
        }

        public static StringBuilder Trimmed(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return value;
            }

            var start = value.CountLeadingWhitespaces();
            var end = value.CountTrailingWhitespaces(start);

            if (end > 0)
            {
                value.Remove(length - end, end);
            }

            if (start > 0)
            {
                value.Remove(0, start);
            }

            return value;
        }

        public static string Trim(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var start = value.CountLeadingWhitespaces();
            var end = value.CountTrailingWhitespaces(start);

            return value.ToString(start, length - start - end);
        }

        public static StringBuilder TrimmedStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return value;
            }

            var start = value.CountLeadingWhitespaces();

            return value.Remove(0, start);
        }

        public static string TrimStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var start = value.CountLeadingWhitespaces();

            return value.ToString(start, length - start);
        }

        public static string TrimEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var end = value.CountTrailingWhitespaces();

            return value.ToString(0, length - end);
        }

        public static StringBuilder TrimEndBy(this StringBuilder value, int count)
        {
            if (count == 0)
            {
                return value;
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var length = value.Length;

            if (count > length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return value.Remove(length - count, count);
        }

        public static StringBuilder TrimmedEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return value;
            }

            var end = value.CountTrailingWhitespaces();

            if (end == 0)
            {
                return value;
            }

            return value.Remove(length - end, end);
        }

        public static StringBuilder WithoutMultipleWhiteSpaces(this StringBuilder value) => value.ReplaceAllWithCheck(Constants.Comments.MultiWhitespaceStrings, Constants.Comments.SingleWhitespaceString); // ncrunch: no coverage

        public static StringBuilder WithoutNewLines(this StringBuilder value) => value.Replace("\r", string.Empty).Replace("\n", string.Empty); // ncrunch: no coverage

        public static StringBuilder Without(this StringBuilder value, string phrase) => value.ReplaceWithCheck(phrase, string.Empty); // ncrunch: no coverage

        public static StringBuilder Without(this StringBuilder value, string[] phrases) => value.ReplaceAllWithCheck(phrases, string.Empty); // ncrunch: no coverage

        public static StringBuilder WithoutAbbreviations(this StringBuilder value) => AbbreviationDetector.FindAndReplaceAllAbbreviations(value); // ncrunch: no coverage

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder WithoutParaTags(this StringBuilder value) => value.Without(Constants.ParaTags); // ncrunch: no coverage

        private static bool QuickCompare(ref StringBuilder current, ref string other)
        {
            var otherValueLength = other.Length;
            var currentValueLength = current.Length;

            var difference = currentValueLength - otherValueLength;

            if (difference < 0)
            {
                // cannot be part in the replacement as other value is too long and cannot fit current value
                return false;
            }

            if (difference > 0)
            {
                if (otherValueLength > QuickCompareLengthThreshold)
                {
                    var otherFirst = other[0];

                    var lastIndex = otherValueLength - 1;
                    var otherLast = other[lastIndex];

                    var length = difference + 1; // increased by 1 to have the complete difference investigated

                    if (difference >= QuickCompareRentLengthThreshold)
                    {
                        // use rented arrays here (if we have larger differences, the start and end may be in different chunks)
                        return QuickCompareAtIndicesWithRent(ref current, ref otherFirst, ref otherLast, ref length, ref lastIndex);
                    }

                    // could be part in the replacement only if characters match
                    return QuickCompareAtIndices(ref current, ref otherFirst, ref otherLast, ref length, ref lastIndex);
                }

                // can be part in the replacement as other value is smaller and could fit current value
                return true;
            }

            // if (difference == 0)
            {
                // both values have same length, so do the quick check on whether the characters fit the expected ones (do not limit length as there are only 3 values below length of 8)
                if (current[0] != other[0])
                {
                    return false;
                }

                var lastIndex = otherValueLength - 1;

                return current[lastIndex] == other[lastIndex];
            }
        }

        private static bool QuickCompareAtIndices(ref StringBuilder current, ref char first, ref char last, ref int length, ref int lastIndex)
        {
            for (var i = 0; i < length; i++)
            {
                if (current[i] == first)
                {
                    if (current[lastIndex + i] == last)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool QuickCompareAtIndicesWithRent(ref StringBuilder current, ref char first, ref char last, ref int length, ref int lastIndex)
        {
            // use a rented shared pool for performance reasons
            var shared = ArrayPool<char>.Shared;
            var begin = shared.Rent(length);
            var end = shared.Rent(length);

            try
            {
                current.CopyTo(0, begin, 0, length);
                current.CopyTo(lastIndex, end, 0, length);

                // could be part in the replacement only if characters match
                for (var i = 0; i < length; i++)
                {
                    if (begin[i] == first)
                    {
                        if (end[i] == last)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            finally
            {
                shared.Return(begin);
                shared.Return(end);
            }
        }

        private static int CountLeadingWhitespaces(this StringBuilder value, int start = 0)
        {
            var whitespaces = 0;
            var valueLength = value.Length;

            for (; start < valueLength; start++)
            {
                if (value[start].IsWhiteSpace())
                {
                    whitespaces++;
                }
                else
                {
                    break;
                }
            }

            return whitespaces;
        }

        private static int CountTrailingWhitespaces(this StringBuilder value, int start = 0)
        {
            var whitespaces = 0;

            for (var i = value.Length - 1; i >= start; i--)
            {
                if (value[i].IsWhiteSpace())
                {
                    whitespaces++;
                }
                else
                {
                    break;
                }
            }

            return whitespaces;
        }

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}