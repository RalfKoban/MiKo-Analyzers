using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Text
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder ReplaceAll(this StringBuilder value, IEnumerable<string> texts, string replacement)
        {
            foreach (var text in texts)
            {
                value.ReplaceWithCheck(text, replacement);
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

        public static StringBuilder ReplaceAllWithCheck(this StringBuilder value, IEnumerable<KeyValuePair<string, string>> replacementPairs)
        {
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

        // TODO RKN: StringReplace with StringComparison http://stackoverflow.com/a/244933/84852
    }
}