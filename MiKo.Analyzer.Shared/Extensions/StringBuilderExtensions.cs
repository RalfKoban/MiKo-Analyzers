﻿using System.Collections.Generic;

using MiKoSolutions.Analyzers.Linguistics;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        public static string AdjustFirstWord(this StringBuilder value, FirstWordHandling handling)
        {
            if (value is null)
            {
                return string.Empty;
            }

            var valueLength = value.Length;

            if (valueLength == 0)
            {
                return string.Empty;
            }

            if (value[0] == '<')
            {
                return value.ToString();
            }

            var spacesToKeep = handling.HasFlag(FirstWordHandling.KeepLeadingSpace) ? 1 : 0;

            // only keep it if there is already a leading space (otherwise it may be on the same line without any leading space, and we would fix it in a wrong way)
            TrimLeadingSpacesTo(spacesToKeep);

            if (handling.HasFlag(FirstWordHandling.MakeLowerCase))
            {
                MakeLowerCase();
            }
            else if (handling.HasFlag(FirstWordHandling.MakeUpperCase))
            {
                MakeUpperCase();
            }

            if (handling.HasFlag(FirstWordHandling.MakeInfinite))
            {
                MakeInfinite();
            }

            return value.ToString();

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
                // 1. Find word begin
                var whitespacesBefore = CountWhitespaces(0);

                // 2. Find word end
                var wordLength = 0;
                for (var i = whitespacesBefore; i < valueLength; i++)
                {
                    if (value[i].IsWhiteSpace())
                    {
                        break;
                    }

                    wordLength++;
                }

                // 3. Cut word
                var word = value.ToString(whitespacesBefore, wordLength);
                value.Remove(whitespacesBefore, wordLength);

                // 4. Make word infinite
                var infiniteWord = Verbalizer.MakeInfiniteVerb(word);

                // 5. Insert word at correct position
                value.Insert(whitespacesBefore, infiniteWord);
            }

            void TrimLeadingSpacesTo(int count)
            {
                if (value[0] == ' ')
                {
                    var whitespaces = CountWhitespaces(0);

                    if (whitespaces > count)
                    {
                        value.Remove(count, whitespaces - count);

                        valueLength = value.Length;
                    }
                }
            }

            int CountWhitespaces(int i)
            {
                var whitespaces = 0;

                for (; i < valueLength; i++)
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
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, IEnumerable<KeyValuePair<string, string>> replacementPairs)
        {
            if (replacementPairs is KeyValuePair<string, string>[] array)
            {
                return value.ReplaceAllWithCheck(array.AsSpan());
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var pair in replacementPairs)
            {
                var oldValue = pair.Key;

                if (oldValue.IsNullOrEmpty())
                {
                    // cannot replace any empty value
                    continue;
                }

                if (QuickCompare(value, oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, pair.Value);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, ReadOnlySpan<KeyValuePair<string, string>> replacementPairs)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
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

                if (QuickCompare(value, oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, pair.Value);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, string[] texts, string replacement)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            var length = texts.Length;

            for (var index = 0; index < length; index++)
            {
                var oldValue = texts[index];

                if (oldValue.IsNullOrEmpty())
                {
                    // cannot replace any empty value
                    continue;
                }

                if (QuickCompare(value, oldValue))
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

            if (QuickCompare(value, oldValue))
            {
                return value.Replace(oldValue, newValue);
            }

            // cannot be part in the replacement as value does not fit
            return value;
        }

        public static string Trim(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var start = 0;
            var end = 0;

            for (var i = 0; i < length; i++)
            {
                if (value[i].IsWhiteSpace())
                {
                    start++;
                }
                else
                {
                    break;
                }
            }

            for (var i = length - 1; i >= start; i--)
            {
                if (value[i].IsWhiteSpace())
                {
                    end++;
                }
                else
                {
                    break;
                }
            }

            return value.ToString(start, length - (start + end));
        }

        public static string TrimStart(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var start = 0;

            for (var i = 0; i < length; i++)
            {
                if (value[i].IsWhiteSpace())
                {
                    start++;
                }
                else
                {
                    break;
                }
            }

            return value.ToString(start, length - start);
        }

        public static string TrimEnd(this StringBuilder value)
        {
            var length = value.Length;

            if (length == 0)
            {
                return string.Empty;
            }

            var end = 0;

            for (var i = length - 1; i >= 0; i--)
            {
                if (value[i].IsWhiteSpace())
                {
                    end++;
                }
                else
                {
                    break;
                }
            }

            return value.ToString(0, length - end);
        }

        private static bool QuickCompare(StringBuilder current, string other)
        {
            var otherValueLength = other.Length;
            var currentValueLength = current.Length;

            var difference = currentValueLength - otherValueLength;

            if (difference < 0)
            {
                // cannot be part in the replacement as other value is too long and cannot fit current value
                return false;
            }

            if (difference == 0)
            {
                // both values have same length, so do the quick check on whether the characters fit the expected ones (do not limit length as there are only 3 values below length of 8)
                if (current[0] != other[0])
                {
                    return false;
                }

                var lastIndex = otherValueLength - 1;

                return current[lastIndex] == other[lastIndex];
            }

            if (other.Length > 4)
            {
                var otherFirst = other[0];

                var lastIndex = otherValueLength - 1;
                var otherLast = other[lastIndex];

                // could be part in the replacement only if characters match
                return QuickCompareAtIndices(current, otherFirst, lastIndex, otherLast, difference);
            }

            // can be part in the replacement as other value is smaller and could fit current value
            return true;
        }

        private static bool QuickCompareAtIndices(StringBuilder current, char first, int lastIndex, char last, int count)
        {
            // include count as value
            for (var i = 0; i <= count; i++)
            {
                if (current[i] == first && current[lastIndex + i] == last)
                {
                    return true;
                }
            }

            return false;
        }

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}