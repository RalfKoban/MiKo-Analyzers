// for performance reasons we switch of RDI and NCrunch instrumentation
//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    public readonly struct Pair : IEquatable<Pair>
    {
        public readonly string Key; // made as field instead of property for performance reasons
        public readonly string Value; // made as field instead of property for performance reasons

        public Pair(string key, string value = "")
        {
            Key = key;
            Value = value;
        }

        public static bool operator ==(Pair left, Pair right) => left.Equals(right);

        public static bool operator !=(Pair left, Pair right) => left.Equals(right) is false;

        public bool Equals(Pair other) => Key == other.Key && Value == other.Value;

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