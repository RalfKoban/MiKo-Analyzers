using System;

// ncrunch: no coverage start
// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides an enumerator that splits a <see cref="ReadOnlySpan{T}"/> of characters into parts based on separator characters.
    /// </summary>
    /// <remarks>
    /// This enumerator supports the <see cref="StringSplitOptions"/> to control how empty entries are handled during enumeration.
    /// The enumerator can be reset and provides a count of elements that respects the specified split options.
    /// <para>
    /// <note type="important">
    /// This is a ref struct that enables efficient enumeration without allocating strings for each part.
    /// </note>
    /// </para>
    /// </remarks>
    //// Note: It must be a 'ref' struct as it contains a ReadOnlySpan<char> which requires the 'ref'.
    internal ref struct SplitReadOnlySpanEnumerator
    {
        private readonly ReadOnlySpan<char> m_separatorChars;
        private readonly StringSplitOptions m_options;
        private readonly ReadOnlySpan<char> m_initialText;
        private ReadOnlySpan<char> m_spanAfterMoveNext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitReadOnlySpanEnumerator"/> struct that splits the specified text using the provided separator characters and options.
        /// </summary>
        /// <param name="text">
        /// The text to be split.
        /// </param>
        /// <param name="separatorChars">
        /// The characters to separate the text into parts.
        /// </param>
        /// <param name="options">
        /// A bitwise combination of the enumeration members that specifies how the splitting operations are performed.
        /// </param>
        public SplitReadOnlySpanEnumerator(in ReadOnlySpan<char> text, in ReadOnlySpan<char> separatorChars, in StringSplitOptions options)
        {
            m_initialText = text;
            m_spanAfterMoveNext = text;
            m_separatorChars = separatorChars;
            m_options = options;

            Current = default;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the collection at the current position of the enumerator.
        /// </value>
        public ReadOnlySpanEnumeratorEntry Current { get; private set; }

        /// <summary>
        /// Gets the numbers of elements within the enumerator.
        /// </summary>
        /// <returns>
        /// The numbers of elements within the enumerator, taking the specified <see cref="StringSplitOptions"/> into account.
        /// </returns>
        public int Count()
        {
            try
            {
                var count = 0;

                while (MoveNext())
                {
                    count++;
                }

                return count;
            }
            finally
            {
                Reset(); // we need to reset it as we moved the enumerator for counting
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// A <see cref="SplitReadOnlySpanEnumerator" /> that allows to iterate through the collection.
        /// </returns>
        public SplitReadOnlySpanEnumerator GetEnumerator() => this; // Needed to be compatible with the foreach operator

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            m_spanAfterMoveNext = m_initialText;
            Current = default;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if the enumerator was successfully advanced to the next element; otherwise, <see langword="false" />.
        /// In such case the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            switch (m_options)
            {
                case StringSplitOptions.None:
                    return MoveNextCore();

                case StringSplitOptions.RemoveEmptyEntries:
                {
                    bool next;

                    // filter empty parts
                    do
                    {
                        next = MoveNextCore();

                        if (Current.IsEmpty)
                        {
                            continue;
                        }

                        break;
                    }
                    while (next);

                    return next;
                }

                default: // not yet defined, so we simply move next
                    return MoveNextCore();
            }
        }

        private bool MoveNextCore()
        {
            var spanBeforeMoveNext = m_spanAfterMoveNext;

            if (spanBeforeMoveNext.Length is 0)
            {
                // we reached the end of the string
                return false;
            }

            var index = spanBeforeMoveNext.IndexOfAny(m_separatorChars);

            if (index is -1)
            {
                // The remaining string is an empty string
                m_spanAfterMoveNext = ReadOnlySpan<char>.Empty;
                Current = new ReadOnlySpanEnumeratorEntry(spanBeforeMoveNext);

                return true;
            }

            m_spanAfterMoveNext = spanBeforeMoveNext.Slice(index + 1);
            Current = new ReadOnlySpanEnumeratorEntry(spanBeforeMoveNext.Slice(0, index));

            return true;
        }
    }
}

// ncrunch: no coverage end
