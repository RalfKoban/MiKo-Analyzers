using System;
using System.Collections.Generic;

// ncrunch: rdi off
// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a specialized enumerator for words within a <see cref="ReadOnlySpan{T}"/> of characters, where words are identified by uppercase characters.
    /// </summary>
    /// <remarks>
    /// The enumerator splits text into words based on uppercase character positions and provides methods to access and transform these words.
    /// </remarks>
    internal ref struct WordsReadOnlySpanEnumerator // Must be a ref struct as it contains a ReadOnlySpan<char>
    {
        private readonly ReadOnlySpan<char> m_text;
        private readonly int[] m_wordStartingPositions;
        private int m_currentIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordsReadOnlySpanEnumerator"/> struct that contains words from the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to split into words based on uppercase characters.
        /// </param>
        /// <remarks>
        /// Words are identified by uppercase characters within the text.
        /// </remarks>
        public WordsReadOnlySpanEnumerator(in ReadOnlySpan<char> text)
        {
            m_text = text;

            var textLength = text.Length;
            var words = 1;

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            for (var index = 1; index < textLength; index++)
            {
                if (text[index].IsUpperCase())
                {
                    words++;
                }
            }

            m_wordStartingPositions = new int[words];
            m_wordStartingPositions[0] = 0;

            // start at index 1 to skip first upper case character (and avoid return of empty word)
            for (int index = 1, i = 1; index < textLength; index++)
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
        /// Gets the numbers of elements within the enumerator.
        /// </summary>
        /// <returns>
        /// The numbers of elements within the enumerator.
        /// </returns>
        public int Count() => m_wordStartingPositions.Length - 1; // first entry is zero and needs to be removed

        /// <summary>
        /// Projects each element of the collection into a string by applying a specified transformation.
        /// </summary>
        /// <param name="callback">
        /// A transformation to apply to each element in the collection.
        /// </param>
        /// <returns>
        /// A collection of strings that contains the projected elements from the current collection.
        /// </returns>
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

        /// <summary>
        /// Gets the first element in the collection.
        /// </summary>
        /// <returns>
        /// The first element in the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The collection is empty.
        /// </exception>
        public ReadOnlySpanEnumeratorEntry First()
        {
            if (m_text.Length is 0)
            {
                throw new InvalidOperationException("No item available");
            }

            if (m_wordStartingPositions.Length > 1)
            {
                return new ReadOnlySpanEnumeratorEntry(m_text.Slice(m_wordStartingPositions[0], m_wordStartingPositions[1]));
            }

            return new ReadOnlySpanEnumeratorEntry(m_text.Slice(m_wordStartingPositions[0]));
        }

        /// <summary>
        /// Gets the last element in the collection.
        /// </summary>
        /// <returns>
        /// The last element in the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The collection is empty.
        /// </exception>
        public ReadOnlySpanEnumeratorEntry Last()
        {
            if (m_text.Length is 0)
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
