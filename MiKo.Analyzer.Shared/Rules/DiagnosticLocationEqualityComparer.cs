using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules
{
    internal sealed class DiagnosticLocationEqualityComparer : IEqualityComparer<Diagnostic>
    {
        public static readonly DiagnosticLocationEqualityComparer Default = new DiagnosticLocationEqualityComparer();

        public bool Equals(Diagnostic x, Diagnostic y)
        {
            if (x is null) return false;
            if (y is null) return false;

            if (ReferenceEquals(x, y)) return true;

            return x.Location.Equals(y.Location);
        }

        public int GetHashCode(Diagnostic obj) => obj.Location.GetHashCode();
    }
}