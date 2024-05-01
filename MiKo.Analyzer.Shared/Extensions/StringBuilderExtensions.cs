using System.Collections.Generic;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, IEnumerable<KeyValuePair<string, string>> replacementPairs)
        {
            if (replacementPairs is IList<KeyValuePair<string, string>> list)
            {
                return value.ReplaceAllWithCheck(list);
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var pair in replacementPairs)
            {
                var oldValue = pair.Key;

                if (QuickCompare(value, oldValue))
                {
                    // can be part in the replacement as value seems to fit
                    value.Replace(oldValue, pair.Value);
                }
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, IList<KeyValuePair<string, string>> replacementPairs)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            var count = replacementPairs.Count;

            for (var index = 0; index < count; index++)
            {
                var pair = replacementPairs[index];
                var oldValue = pair.Key;

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

        private static bool QuickCompare(StringBuilder value, string oldValue)
        {
            var oldValueLength = oldValue.Length;
            var valueLength = value.Length;

            if (oldValueLength > valueLength)
            {
                // cannot be part in the replacement as value is too long
                return false;
            }

            if (oldValueLength < valueLength)
            {
                // can be part in the replacement as value is shorter
                return true;
            }

            // both values have same length, so do the quick check on whether the characters fit the expected ones
            if (oldValueLength >= 8)
            {
                if (oldValue[0] != value[0])
                {
                    // cannot be part in the replacement as characters do not match in current value
                    return false;
                }

                var lastIndex = oldValueLength - 1;

                if (oldValue[lastIndex] != value[lastIndex])
                {
                    // cannot be part in the replacement as characters do not match in current value
                    return false;
                }
            }

            return true;
        }

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}