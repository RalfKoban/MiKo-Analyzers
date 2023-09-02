// ReSharper disable once CheckNamespace
namespace System
{
    public readonly ref struct ReadOnlySpanEnumeratorEntry
    {
        public ReadOnlySpanEnumeratorEntry(ReadOnlySpan<char> text) => Text = text;

        public ReadOnlySpan<char> Text { get; }

        public bool IsEmpty => Text.IsEmpty;

        public static implicit operator ReadOnlySpan<char>(ReadOnlySpanEnumeratorEntry entry) => entry.Text;

        public override string ToString() => Text.ToString();
    }
}