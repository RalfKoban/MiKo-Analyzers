using System;
using System.Collections.Generic;

//// ncrunch: rdi off
//// ncrunch: no coverage start
namespace MiKoSolutions.Analyzers.Linguistics
{
    internal sealed class AscendingStringComparer : IComparer<string>
    {
        public static readonly AscendingStringComparer Default = new AscendingStringComparer();

        public int Compare(string x, string y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }

            if (y is null)
            {
                return 1;
            }

            var result = string.Compare(x, 0, y, 0, Math.Min(x.Length, y.Length), StringComparison.OrdinalIgnoreCase);

            if (result is 0)
            {
                // same sub string, so investigate into length (if y is longer, prefer y but if x is longer, prefer x)
                return y.Length - x.Length;
            }

            // found something different
            return result;
        }
    }
}