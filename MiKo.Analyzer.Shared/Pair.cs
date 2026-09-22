using System;

// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
namespace MiKoSolutions.Analyzers
{
    public readonly struct Pair : IEquatable<Pair>
    {
#pragma warning disable CA1051 // made as field instead of property for performance reasons
        public readonly string Key; // made as field instead of property for performance reasons
        public readonly string Value; // made as field instead of property for performance reasons
#pragma warning restore CA1051

        public Pair(string key, string value = "")
        {
            Key = StringCache.Intern(key);
            Value = StringCache.Intern(value);
        }

        public static bool operator ==(in Pair left, in Pair right) => left.Equals(right);

        public static bool operator !=(in Pair left, in Pair right) => left.Equals(right) is false;

        public bool Equals(Pair other) => Key.AsSpan().SequenceEqual(other.Key.AsSpan()) && Value.AsSpan().SequenceEqual(other.Value.AsSpan());

        public override bool Equals(object obj) => obj is Pair other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key?.GetHashCode() ?? 0) * 397) ^ (Value?.GetHashCode() ?? 0);
            }
        }

        public override string ToString() => string.Concat(Key, " -> ", Value);
    }
}