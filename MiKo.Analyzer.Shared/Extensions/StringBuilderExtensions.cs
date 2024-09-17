using System.Collections.Generic;

using MiKoSolutions.Analyzers.Linguistics;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        private const int QuickCompareLengthThreshold = 4;

        public static bool IsNullOrWhiteSpace(this StringBuilder value) => value is null || value.CountWhitespaces(0) == value.Length;

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
                var word = FirstWord(value, out var whitespacesBefore);

                value.Remove(whitespacesBefore, word.Length);

                // 4. Make word infinite
                var infiniteWord = Verbalizer.MakeInfiniteVerb(word);

                // 5. Insert word at correct position
                value.Insert(whitespacesBefore, infiniteWord);
            }

            void TrimLeadingSpacesTo(int count)
            {
                if (value[0] == ' ')
                {
                    var whitespaces = value.CountWhitespaces(0);

                    if (whitespaces > count)
                    {
                        value.Remove(count, whitespaces - count);

                        valueLength = value.Length;
                    }
                }
            }
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
            whitespacesBefore = value.CountWhitespaces(0);

            // 2. Find word end
            var wordLength = 0;
            for (var i = whitespacesBefore; i < length; i++)
            {
                if (value[i].IsWhiteSpace())
                {
                    break;
                }

                wordLength++;
            }

            // 3. Cut word
            return value.ToString(whitespacesBefore, wordLength);
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

                if (QuickCompare(ref value, ref oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, pair.Value);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, ReadOnlySpan<string> texts, string replacement)
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

            if (other.Length > QuickCompareLengthThreshold)
            {
                var otherFirst = other[0];

                var lastIndex = otherValueLength - 1;
                var otherLast = other[lastIndex];

                // could be part in the replacement only if characters match
                return QuickCompareAtIndices(ref current, ref otherFirst, ref lastIndex, ref otherLast, ref difference);
            }

            // can be part in the replacement as other value is smaller and could fit current value
            return true;
        }

        private static bool QuickCompareAtIndices(ref StringBuilder current, ref char first, ref int lastIndex, ref char last, ref int count)
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

        private static int CountWhitespaces(this StringBuilder value, int start)
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

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}