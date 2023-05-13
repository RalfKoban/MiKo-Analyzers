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
                value.Replace(text, replacement);
            }

            return value;
        }
    }
}