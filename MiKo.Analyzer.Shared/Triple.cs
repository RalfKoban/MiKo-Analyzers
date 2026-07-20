using System;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
namespace MiKoSolutions.Analyzers
{
    public readonly struct Triple : IEquatable<Triple>
    {
#pragma warning disable CA1051 // made as field instead of property for performance reasons
        public readonly string A; // made as field instead of property for performance reasons
        public readonly string B; // made as field instead of property for performance reasons
        public readonly string C; // made as field instead of property for performance reasons
#pragma warning restore CA1051

        /// <summary>
        /// Contains the pre-calculated hash code as the strings <see cref="A"/>, <see cref="B"/> and <see cref="C"/> will not change anymore after the constructor is finished.
        /// </summary>
        private readonly int m_hashCode;

        public Triple(string a, string b, string c)
        {
            A = a;
            B = b;
            C = c;

            m_hashCode = CalculateHashCode(A, B, C);
        }

        public static bool operator ==(in Triple left, in Triple right) => left.Equals(right);

        public static bool operator !=(in Triple left, in Triple right) => left.Equals(right) is false;

        public bool Equals(Triple other) => A.AsSpan().SequenceEqual(other.A.AsSpan())
                                         && B.AsSpan().SequenceEqual(other.B.AsSpan())
                                         && C.AsSpan().SequenceEqual(other.C.AsSpan());

        public override bool Equals(object obj) => obj is Triple other && Equals(other);

        public override int GetHashCode() => m_hashCode;

        public override string ToString() => string.Concat(A, ": ", B, " -> ", C);

        private static int CalculateHashCode(string a, string b, string c)
        {
            unchecked
            {
                var hashCode = a?.GetHashCode() ?? 0;

                hashCode = (hashCode * 397) ^ (b?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (c?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}