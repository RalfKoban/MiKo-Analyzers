using System;
using System.Collections.Generic;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Linguistics
{
    public sealed class AscendingStringComparer : IComparer<string>
    {
        public static readonly AscendingStringComparer Default = new AscendingStringComparer();

        public int Compare(string x, string y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            var lengthX = x.Length;
            var lengthY = y.Length;

            var minimumLength = Math.Min(lengthX, lengthY);

            var spanX = x.AsSpan(0, minimumLength);
            var spanY = y.AsSpan(0, minimumLength);

            var result = spanX.CompareTo(spanY, StringComparison.OrdinalIgnoreCase);

            if (result != 0)
            {
                // found something different
                return result;
            }

            // same sub string, so investigate into length
            var lengthDifference = lengthX - lengthY;
            if (lengthDifference < 0)
            {
                // y longer, so prefer y
                return 1;
            }

            if (lengthDifference > 0)
            {
                // x longer, so prefer x
                return -1;
            }

            return 0;
        }
    }
}