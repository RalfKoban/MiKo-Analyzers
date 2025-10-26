using System;

// ncrunch: no coverage start
// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Represents an entry for enumerating over a read-only span of characters.
    /// </summary>
    internal readonly ref struct ReadOnlySpanEnumeratorEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySpanEnumeratorEntry"/> struct with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to be stored in the entry.
        /// </param>
        public ReadOnlySpanEnumeratorEntry(in ReadOnlySpan<char> text) => Text = text;

        /// <summary>
        /// Gets the text of the entry.
        /// </summary>
        /// <value>
        /// The text of the entry.
        /// </value>
        public ReadOnlySpan<char> Text { get; }

        /// <summary>
        /// Gets a value indicating whether the text is empty.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the text is empty; otherwise, <see langword="false"/>.
        /// This property has no default value.
        /// </value>
        public bool IsEmpty => Text.IsEmpty;

        /// <summary>
        /// Converts the entry to a read-only span of characters.
        /// </summary>
        /// <param name="value">
        /// The entry to convert.
        /// </param>
        /// <returns>
        /// A read-only span of characters representing the text of the entry.
        /// </returns>
        public static implicit operator ReadOnlySpan<char>(in ReadOnlySpanEnumeratorEntry value) => value.Text;

        /// <inheritdoc/>
        public override string ToString() => Text.ToString();
    }
}

// ncrunch: no coverage end
