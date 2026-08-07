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
    public sealed class AhoCorasickMatcher
    {
        // sorted list of every character that occurs in any pattern; used to map a char to a dense alphabet index via binary search
        private readonly char[] m_alphabet;

        // flattened deterministic transition table: m_transitions[(state * m_alphabet.Length) + alphabetIndex] = next state
        private readonly int[] m_transitions;

        // m_isTerminal[state] indicates whether reaching that state means a pattern was matched
        private readonly bool[] m_isTerminal;

        /// <summary>
        /// Initializes a new instance of the <see cref="AhoCorasickMatcher"/> class for the specified patterns.
        /// </summary>
        /// <param name="patterns">
        /// The patterns to seek for. <see langword="null"/> or empty entries within <paramref name="patterns"/> are ignored.
        /// </param>
        private AhoCorasickMatcher(IEnumerable<string> patterns)
        {
            var root = new Node();
            var alphabet = new HashSet<char>();

            foreach (var pattern in patterns)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    continue;
                }

                var node = root;

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

            // resolve the whole transition table once, up-front, based on the mutable 'Node' graph
            BuildDeterministicTransitions(root, alphabet);

            // compile the resolved 'Node' graph into flat arrays so that 'IsMatch' operates on cheap array indexing only
            // (no dictionary lookups, no pointer chasing) and never mutates any state afterward;
            // instances are therefore safe to share and to query concurrently from multiple threads.
            m_alphabet = alphabet.ToArray();

            Array.Sort(m_alphabet);

            CompileToArrays(root, m_alphabet, out m_transitions, out m_isTerminal);
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
            var alphabetLength = m_alphabet.Length;

            var state = 0; // root

            for (int index = 0, length = text.Length; index < length; index++)
            {
                var alphabetIndex = Array.BinarySearch(m_alphabet, text[index]);

                // unknown character (not part of any pattern): simply stay at / return to the root
                state = alphabetIndex < 0 ? 0 : m_transitions[(state * alphabetLength) + alphabetIndex];

                if (m_isTerminal[state])
                {
                    return true;
                }
            }

            return false;
        }

        private static void BuildDeterministicTransitions(Node root, HashSet<char> alphabet)
        {
            // snapshot the root's genuine trie edges before extending them with fallback shortcuts
            var rootRealChildren = new List<Node>(root.Children.Values);

            foreach (var c in alphabet)
            {
                if (root.Children.ContainsKey(c) is false)
                {
                    root.Children[c] = root; // unknown characters seen while at the root simply stay at the root
                }
            }

            var queue = new Queue<Node>(rootRealChildren);

            foreach (var child in rootRealChildren)
            {
                child.Fail = root;
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

        /// <summary>
        /// Flattens the resolved <see cref="Node"/> graph (states, terminal flags, and per-character transitions)
        /// into plain arrays so that <see cref="IsMatch"/> only ever performs array indexing at query time.
        /// </summary>
        private static void CompileToArrays(Node root, char[] alphabet, out int[] transitions, out bool[] isTerminal)
        {
            // assign a stable, dense integer id to every reachable node (root is always state 0)
            var stateOf = new Dictionary<Node, int>();
            var order = new List<Node>();

            var queue = new Queue<Node>();
            queue.Enqueue(root);
            stateOf[root] = 0;
            order.Add(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var child in current.Children.Values)
                {
                    if (stateOf.ContainsKey(child) is false)
                    {
                        stateOf[child] = order.Count;
                        order.Add(child);

                        queue.Enqueue(child);
                    }
                }
            }

            var alphabetLength = alphabet.Length;
            var stateCount = order.Count;

            transitions = new int[stateCount * alphabetLength];
            isTerminal = new bool[stateCount];

            for (var state = 0; state < stateCount; state++)
            {
                var node = order[state];
                isTerminal[state] = node.IsTerminal;

                var baseIndex = state * alphabetLength;

                for (var alphabetIndex = 0; alphabetIndex < alphabetLength; alphabetIndex++)
                {
                    // after 'BuildDeterministicTransitions', every node has an edge for every alphabet character
                    var next = node.Children[alphabet[alphabetIndex]];

                    transitions[baseIndex + alphabetIndex] = stateOf[next];
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
