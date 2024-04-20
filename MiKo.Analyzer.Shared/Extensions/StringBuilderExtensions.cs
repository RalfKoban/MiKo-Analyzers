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
            if (replacementPairs is KeyValuePair<string, string>[] array)
            {
                return value.ReplaceAllWithCheck(array);
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var pair in replacementPairs)
            {
                if (pair.Key.Length > value.Length)
                {
                    // cannot be part in the replacement as value is too big
                    continue;
                }

                value.Replace(pair.Key, pair.Value);
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, KeyValuePair<string, string>[] replacementPairs)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < replacementPairs.Length; index++)
            {
                var pair = replacementPairs[index];

                var oldValue = pair.Key;

                if (oldValue.Length > value.Length)
                {
                    // cannot be part in the replacement as value is too big
                    continue;
                }

                value.Replace(oldValue, pair.Value);
            }

            return value;
        }

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, string[] texts, string replacement)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < texts.Length; index++)
            {
                var text = texts[index];

                if (text.Length > value.Length)
                {
                    // cannot be part in the replacement as value is too big
                    continue;
                }

                value.Replace(text, replacement);
            }

            return value;
        }

        public static StringBuilder ReplaceWithCheck(this StringBuilder value, string oldValue, string newValue)
        {
            if (oldValue.Length > value.Length)
            {
                // cannot be part in the replacement as value is too big
                return value;
            }

            return value.Replace(oldValue, newValue);
        }

        public static string ToStringTrimmed(this StringBuilder value)
        {
            if (value.Length == 0)
            {
                return string.Empty;
            }

            var start = 0;
            var end = 0;

            for (var i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                {
                    start++;
                }
                else
                {
                    break;
                }
            }

            for (var i = value.Length - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(value[i]))
                {
                    end++;
                }
                else
                {
                    break;
                }
            }

            if (start == 0 && end == 0)
            {
                // nothing to trim
                return value.ToString();
            }

            var length = end - start;

            return value.ToString(start, length);
        }

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}