using System;

// ncrunch: no coverage start
// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal readonly ref struct ReadOnlySpanEnumeratorEntry
    {
        public ReadOnlySpanEnumeratorEntry(in ReadOnlySpan<char> text) => Text = text;

        public ReadOnlySpan<char> Text { get; }

        public bool IsEmpty => Text.IsEmpty;

        public static implicit operator ReadOnlySpan<char>(in ReadOnlySpanEnumeratorEntry entry) => entry.Text;

        public override string ToString() => Text.ToString();
    }
}

// ncrunch: no coverage end
