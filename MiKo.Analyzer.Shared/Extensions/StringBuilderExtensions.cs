using System.Collections.Generic;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
namespace System.Text
{
    public static class StringBuilderExtensions
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

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}