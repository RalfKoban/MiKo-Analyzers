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
    }
}