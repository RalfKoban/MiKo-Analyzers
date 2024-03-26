using System.Collections.Generic;

// ncrunch: rdi off
// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
namespace System
{
    // Must be a ref struct as it contains a ReadOnlySpan<char>
    internal ref struct WordsReadOnlySpanEnumerator
    {
        private readonly ReadOnlySpan<char> m_text;
        private readonly int[] m_wordStartingPositions;
        private int m_currentIndex;

        public WordsReadOnlySpanEnumerator(ReadOnlySpan<char> text)
        {
            m_text = text;

            var words = 1;

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            for (var index = 1; index < text.Length; index++)
            {
                if (text[index].IsUpperCase())
                {
                    words++;
                }
            }

            m_wordStartingPositions = new int[words];
            m_wordStartingPositions[0] = 0;

            var i = 1;

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            for (var index = 1; index < text.Length; index++)
            {
                if (text[index].IsUpperCase())
                {
                    m_wordStartingPositions[i++] = index;
                }
            }

            m_currentIndex = 0;

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
        /// Returns the numbers of elements within the enumerator.
        /// </summary>
        /// <returns>
        /// The numbers of elements within the enumerator.
        /// </returns>
        public int Count() => m_wordStartingPositions.Length - 1; // first entry is zero and needs to be removed

        public IEnumerable<string> Select(WordsReadOnlySpanEnumeratorSelectDelegate callback)
        {
            var items = new List<string>();

            try
            {
                while (MoveNext())
                {
                    items.Add(callback(Current));
                }
            }
            finally
            {
                Reset(); // we need to reset it as we moved the enumerator for counting
            }

            return items;
        }

        public ReadOnlySpanEnumeratorEntry First()
        {
            if (m_text.Length == 0)
            {
                throw new InvalidOperationException("No item available");
            }

            if (m_wordStartingPositions.Length > 1)
            {
                return new ReadOnlySpanEnumeratorEntry(m_text.Slice(m_wordStartingPositions[0], m_wordStartingPositions[1]));
            }

            return new ReadOnlySpanEnumeratorEntry(m_text.Slice(m_wordStartingPositions[0]));
        }

        public ReadOnlySpanEnumeratorEntry Last()
        {
            if (m_text.Length == 0)
            {
                throw new InvalidOperationException("No item available");
            }

            return new ReadOnlySpanEnumeratorEntry(m_text.Slice(m_wordStartingPositions.Length - 1));
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// A <see cref="WordsReadOnlySpanEnumerator" /> that allows to iterate through the collection.
        /// </returns>
        public WordsReadOnlySpanEnumerator GetEnumerator() => this; // Needed to be compatible with the foreach operator

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            m_currentIndex = 0;

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
            if (m_currentIndex >= m_wordStartingPositions.Length)
            {
                return false;
            }

            var index = m_wordStartingPositions[m_currentIndex];

            if (m_currentIndex < m_wordStartingPositions.Length - 1)
            {
                var nextIndex = m_wordStartingPositions[m_currentIndex + 1];

                Current = new ReadOnlySpanEnumeratorEntry(m_text.Slice(index, nextIndex - index));
            }
            else
            {
                Current = new ReadOnlySpanEnumeratorEntry(m_text.Slice(index));
            }

            m_currentIndex++;

            return true;
        }
    }
}

// ncrunch: no coverage end
