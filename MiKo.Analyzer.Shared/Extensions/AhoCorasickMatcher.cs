using System;
using System.Collections.Generic;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a reusable multi-pattern matcher based on the Aho-Corasick algorithm that determines,
    /// in a single pass over the searched text, whether any of a fixed set of patterns is contained within it.
    /// </summary>
    /// <remarks>
    /// The matcher is built once from a set of patterns and can then be queried many times,
    /// each query costing <c>O(text length)</c> regardless of the number of patterns,
    /// instead of <c>O(patterns count * text length)</c> as with a naive per-pattern <see cref="string.IndexOf(string)"/> loop.
    /// <para />
    /// Matching is performed using <see cref="StringComparison.Ordinal"/> semantics.
    /// <para />
    /// The full transition table is resolved once, up-front, during construction, so that <see cref="IsMatch"/> never
    /// mutates any state afterward; instances are therefore safe to share and to query concurrently from multiple threads.
    /// </remarks>
    internal sealed class AhoCorasickMatcher
    {
        private readonly Node m_root = new Node();

        /// <summary>
        /// Initializes a new instance of the <see cref="AhoCorasickMatcher"/> class for the specified patterns.
        /// </summary>
        /// <param name="patterns">
        /// The patterns to seek for. <see langword="null"/> or empty entries within <paramref name="patterns"/> are ignored.
        /// </param>
        private AhoCorasickMatcher(IEnumerable<string> patterns)
        {
            var alphabet = new HashSet<char>();

            foreach (var pattern in patterns)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    continue;
                }

                var node = m_root;

                foreach (var c in pattern)
                {
                    alphabet.Add(c);

                    if (node.Children.TryGetValue(c, out var next) is false)
                    {
                        next = new Node();

                        node.Children[c] = next;
                    }

                    node = next;
                }

                node.IsTerminal = true;
            }

            // resolve the whole transition table once, up-front, so 'IsMatch' becomes read-only afterward (and therefore safe to call concurrently)
            BuildDeterministicTransitions(alphabet);
        }

        /// <summary>
        /// Gets a new instance of the <see cref="AhoCorasickMatcher"/> class for the specified patterns.
        /// </summary>
        /// <param name="patterns">
        /// The patterns to seek for. <see langword="null"/> or empty entries within <paramref name="patterns"/> are ignored.
        /// </param>
        /// <returns>
        /// The new created matcher initialized with the specified patterns.
        /// </returns>
        public static AhoCorasickMatcher For(IEnumerable<string> patterns) => patterns is null
                                                                              ? throw new ArgumentNullException(nameof(patterns))
                                                                              : new AhoCorasickMatcher(patterns);

        /// <summary>
        /// Determines whether the specified text contains any of the patterns that this matcher was constructed with.
        /// </summary>
        /// <param name="text">
        /// The text to search in.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any of the patterns are found; otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsMatch(in ReadOnlySpan<char> text)
        {
            var node = m_root;

            for (int index = 0, length = text.Length; index < length; index++)
            {
                node = node.Children.TryGetValue(text[index], out var next) ? next : m_root;

                if (node.IsTerminal)
                {
                    return true;
                }
            }

            return false;
        }

        private void BuildDeterministicTransitions(HashSet<char> alphabet)
        {
            // snapshot the root's genuine trie edges before extending them with fallback shortcuts
            var rootRealChildren = new List<Node>(m_root.Children.Values);

            foreach (var c in alphabet)
            {
                if (m_root.Children.ContainsKey(c) is false)
                {
                    m_root.Children[c] = m_root; // unknown characters seen while at the root simply stay at the root
                }
            }

            var queue = new Queue<Node>(rootRealChildren);

            foreach (var child in rootRealChildren)
            {
                child.Fail = m_root;
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                // snapshot the node's genuine trie edges before extending them with fallback shortcuts
                var realChildren = new Dictionary<char, Node>(current.Children);

                foreach (var c in alphabet)
                {
                    if (realChildren.TryGetValue(c, out var child))
                    {
                        // 'current.Fail' was processed earlier (shallower BFS level) or is the root, so its transition table is already complete
                        child.Fail = current.Fail.Children[c];

                        if (child.Fail.IsTerminal)
                        {
                            child.IsTerminal = true;
                        }

                        queue.Enqueue(child);
                    }
                    else
                    {
                        // fallback shortcut, resolved from the already-complete parent transition (no real trie edge for this character)
                        current.Children[c] = current.Fail.Children[c];
                    }
                }
            }
        }

#pragma warning disable SA1401 // Fields should be private
        private sealed class Node
        {
            public readonly Dictionary<char, Node> Children = new Dictionary<char, Node>();

            public Node Fail;

            public bool IsTerminal;
        }
#pragma warning restore SA1401 // Fields should be private
    }
}
