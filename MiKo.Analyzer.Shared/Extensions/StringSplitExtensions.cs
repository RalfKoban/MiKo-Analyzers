using System;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringSplitExtensions
    {
        public static SplitEnumerator SplitBy(this string text, params char[] separatorChars) => SplitBy(text.AsSpan(), separatorChars, StringSplitOptions.None);

        public static SplitEnumerator SplitBy(this ReadOnlySpan<char> text, params char[] separatorChars) => SplitBy(text, separatorChars, StringSplitOptions.None);

        public static SplitEnumerator SplitBy(this string text, char[] separatorChars, StringSplitOptions options) => SplitBy(text.AsSpan(), separatorChars, options);

        public static SplitEnumerator SplitBy(this ReadOnlySpan<char> text, char[] separatorChars, StringSplitOptions options) => new SplitEnumerator(text, separatorChars, options);

        // Must be a ref struct as it contains a ReadOnlySpan<char>
        public ref struct SplitEnumerator
        {
            private readonly char[] m_separatorChars;
            private readonly StringSplitOptions m_options;
            private readonly ReadOnlySpan<char> m_initialText;
            private ReadOnlySpan<char> m_spanAfterMoveNext;

            public SplitEnumerator(ReadOnlySpan<char> text, char[] separatorChars, StringSplitOptions options)
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
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public SplitEntry Current { get; private set; }

            /// <summary>
            /// Returns the numbers of elements within the enumerator.
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
            /// A <see cref="SplitEnumerator" /> that allows to iterate through the collection.
            /// </returns>
            public SplitEnumerator GetEnumerator() => this; // Needed to be compatible with the foreach operator

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

                        } while (next);

                        return next;
                    }

                    default: // not yet defined, so we simply move next
                        return MoveNextCore();
                }
            }

            private bool MoveNextCore()
            {
                var spanBeforeMoveNext = m_spanAfterMoveNext;

                if (spanBeforeMoveNext.Length == 0)
                {
                    // we reached the end of the string
                    return false;
                }

                var index = spanBeforeMoveNext.IndexOfAny(m_separatorChars);

                if (index == -1)
                {
                    // The remaining string is an empty string
                    m_spanAfterMoveNext = ReadOnlySpan<char>.Empty;
                    Current = new SplitEntry(spanBeforeMoveNext);

                    return true;
                }

                m_spanAfterMoveNext = spanBeforeMoveNext.Slice(index + 1);
                Current = new SplitEntry(spanBeforeMoveNext.Slice(0, index));

                return true;
            }
        }

        public readonly ref struct SplitEntry
        {
            public SplitEntry(ReadOnlySpan<char> text) => Text = text;

            public ReadOnlySpan<char> Text { get; }

            public bool IsEmpty => Text.IsEmpty;

            public static implicit operator ReadOnlySpan<char>(SplitEntry entry) => entry.Text;

            public override string ToString() => Text.ToString();
        }
    }
}