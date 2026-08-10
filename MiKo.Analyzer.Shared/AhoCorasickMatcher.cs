using System;
using System.Collections;
using System.Collections.Generic;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a reusable multi-pattern matcher based on the Aho-Corasick algorithm that determines whether any of a fixed set of patterns is contained within it.
    /// This is done in a single pass over the searched text.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calling <see cref="string.IndexOf(string)"/> once per pattern scans the text once for every pattern.
    /// This matcher builds a small state machine once from all patterns instead.
    /// It then scans the text only once, no matter how many patterns exist.
    /// </para>
    /// <para>
    /// Internally, patterns are combined into a trie (a tree of shared prefixes).
    /// "Failure links" let the matcher jump directly to the correct spot on a mismatch.
    /// This avoids restarting from scratch and turns the trie into a fast, deterministic state machine.
    /// </para>
    /// <para>
    /// To save memory, the transition table is not stored as one full row per state.
    /// Instead, each state only stores its most common target as a "default", plus a short list of "exceptions" for the few characters that behave differently.
    /// See <see cref="m_defaultTransition"/> for details.
    /// </para>
    /// <para>
    /// Matching uses <see cref="StringComparison.Ordinal"/> semantics (exact, case-sensitive).
    /// The whole table is built once during construction and never changes afterward, so instances are safe to share and query concurrently.
    /// </para>
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_algorithm"/>
    public sealed class AhoCorasickMatcher
    {
        /// <summary>
        /// The sorted list of every character that occurs in any pattern.
        /// Used to map a char to a dense alphabet index via binary search.
        /// </summary>
        private readonly char[] m_alphabet;

        /// <summary>
        /// For each state, the "default" target used by most alphabet characters.
        /// Characters that behave differently are stored separately as exceptions (see <see cref="m_exceptionChars"/> and <see cref="m_exceptionTargets"/>).
        /// This keeps memory usage low.
        /// </summary>
        private readonly int[] m_defaultTransition;

        /// <summary>
        /// For each state, the start index into <see cref="m_exceptionChars"/> / <see cref="m_exceptionTargets"/>.
        /// A state's exceptions span from <see cref="m_exceptionOffsets"/><c>[state]</c> to <see cref="m_exceptionOffsets"/><c>[state + 1]</c> (exclusive).
        /// </summary>
        private readonly int[] m_exceptionOffsets;

        /// <summary>
        /// The characters that do not use <see cref="m_defaultTransition"/> for their state.
        /// Sorted per state (see <see cref="m_exceptionOffsets"/>) so they can be binary-searched.
        /// </summary>
        private readonly char[] m_exceptionChars;

        /// <summary>
        /// The transition target for the corresponding entry in <see cref="m_exceptionChars"/>.
        /// </summary>
        private readonly int[] m_exceptionTargets;

        /// <summary>
        /// <c>m_isTerminal[state]</c> indicates whether reaching that state means a pattern was matched.
        /// Stored as a <see cref="BitArray"/> (1 bit per state) instead of <c>bool[]</c> (1 byte per state) to reduce memory usage.
        /// </summary>
        private readonly BitArray m_isTerminal;

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

            // compile the resolved 'Node' graph into flat arrays so that 'IsMatch' operates on cheap array indexing only.
            // No dictionary lookups, no pointer chasing.
            // The result never mutates any state afterward, so instances are safe to share and to query concurrently from multiple threads.
            m_alphabet = alphabet.ToArray();

            Array.Sort(m_alphabet);

            CompileToArrays(root, m_alphabet, out m_defaultTransition, out m_exceptionOffsets, out m_exceptionChars, out m_exceptionTargets, out m_isTerminal);
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
            var state = 0; // root

            for (int index = 0, length = text.Length; index < length; index++)
            {
                var c = text[index];

                // unknown character (not part of any pattern): simply stay at / return to the root
                state = Array.BinarySearch(m_alphabet, c) < 0 ? 0 : NextState(state, c);

                if (m_isTerminal[state])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resolves the trie into a deterministic state machine, so every node has an edge for every alphabet character.
        /// Each edge is either a real trie edge or a computed fallback via failure links.
        /// </summary>
        /// <remarks>
        /// This is the classic Aho-Corasick "failure link" construction.
        /// It is processed breadth-first, so each node's failure link (<see cref="Node.Fail"/>) is already fully resolved by the time it is needed.
        /// </remarks>
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
                        // 'current.Fail' was processed earlier (shallower BFS level) or is the root. Its transition table is already complete.
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
        /// Flattens the resolved <see cref="Node"/> graph into plain arrays (states, terminal flags, transitions).
        /// This uses the default/exception split described on <see cref="m_defaultTransition"/>.
        /// </summary>
        private static void CompileToArrays(Node root, in ReadOnlySpan<char> alphabet, out int[] defaultTransition, out int[] exceptionOffsets, out char[] exceptionChars, out int[] exceptionTargets, out BitArray isTerminal)
        {
            // assign a stable, dense integer id to every reachable node (root is always state 0).
            // it is stored directly on the node. This makes resolving a child's state later on a cheap field read instead of a dictionary lookup.
            var order = new List<Node>();

            var queue = new Queue<Node>();
            queue.Enqueue(root);
            root.Id = 0;
            order.Add(root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var child in current.Children.Values)
                {
                    if (child.Id < 0)
                    {
                        child.Id = order.Count;
                        order.Add(child);

                        queue.Enqueue(child);
                    }
                }
            }

            var alphabetLength = alphabet.Length;
            var stateCount = order.Count;

            isTerminal = new BitArray(stateCount);
            defaultTransition = new int[stateCount];
            exceptionOffsets = new int[stateCount + 1];

            var allExceptionChars = new List<char>();
            var allExceptionTargets = new List<int>();

            var rowTargets = new int[alphabetLength];
            var sortedTargets = new int[alphabetLength];

            for (var state = 0; state < stateCount; state++)
            {
                var node = order[state];
                isTerminal[state] = node.IsTerminal;

                for (var alphabetIndex = 0; alphabetIndex < alphabetLength; alphabetIndex++)
                {
                    // after 'BuildDeterministicTransitions', every node has an edge for every alphabet character.
                    // 'Id' was assigned during the BFS above. This is a cheap field read instead of a dictionary lookup.
                    var target = node.Children[alphabet[alphabetIndex]].Id;

                    rowTargets[alphabetIndex] = target;
                }

                // find the target that this state jumps to most often across the whole alphabet.
                // That target becomes the "default". Only the (comparatively few) deviating characters need to be stored explicitly as exceptions.
                // This is done by sorting a scratch copy of the row, so that identical values end up next to each other.
                // Then scan for the longest run of equal values.
                // This is considerably cheaper than tallying occurrences via a dictionary. It matters since this runs once per state.
                var defaultTarget = 0; // no alphabet characters at all (e.g. no patterns were provided): fall back to root

                if (alphabetLength > 0)
                {
                    Array.Copy(rowTargets, sortedTargets, alphabetLength);
                    Array.Sort(sortedTargets);

                    defaultTarget = sortedTargets[0];

                    var bestOccurrences = 1;
                    var runTarget = sortedTargets[0];
                    var runLength = 1;

                    for (var i = 1; i < alphabetLength; i++)
                    {
                        var target = sortedTargets[i];

                        if (runTarget == target)
                        {
                            runLength++;
                        }
                        else
                        {
                            runTarget = target;
                            runLength = 1;
                        }

                        if (runLength > bestOccurrences)
                        {
                            bestOccurrences = runLength;
                            defaultTarget = runTarget;
                        }
                    }
                }

                defaultTransition[state] = defaultTarget;

                exceptionOffsets[state] = allExceptionChars.Count;

                for (var alphabetIndex = 0; alphabetIndex < alphabetLength; alphabetIndex++)
                {
                    var target = rowTargets[alphabetIndex];

                    if (target != defaultTarget)
                    {
                        // 'alphabet' is sorted. Appending in increasing 'alphabetIndex' order keeps each state's exception range sorted as well.
                        // That sorted order is required for the binary search in 'NextState'.
                        allExceptionChars.Add(alphabet[alphabetIndex]);
                        allExceptionTargets.Add(target);
                    }
                }
            }

            exceptionOffsets[stateCount] = allExceptionChars.Count;

            exceptionChars = allExceptionChars.ToArray();
            exceptionTargets = allExceptionTargets.ToArray();
        }

        /// <summary>
        /// Determines the next state to move to from <paramref name="state"/> when the next character in the text is <paramref name="c"/>.
        /// </summary>
        /// <param name="state">
        /// The current state.
        /// </param>
        /// <param name="c">
        /// The next character in the text.
        /// </param>
        /// <returns>
        /// The state to move to.
        /// </returns>
        private int NextState(in int state, in char c)
        {
            var start = m_exceptionOffsets[state];
            var count = m_exceptionOffsets[state + 1] - start;

            if (count > 0)
            {
                var exceptionIndex = Array.BinarySearch(m_exceptionChars, start, count, c);

                if (exceptionIndex >= 0)
                {
                    return m_exceptionTargets[exceptionIndex];
                }
            }

            return m_defaultTransition[state];
        }

#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// Represents a single node (a "state") of the trie, corresponding to one prefix shared by one or more patterns.
        /// </summary>
        private sealed class Node
        {
            /// <summary>
            /// The outgoing edges of this node.
            /// Initially only real trie edges; after <see cref="BuildDeterministicTransitions"/>, computed fallback edges are added too.
            /// </summary>
            public readonly Dictionary<char, Node> Children = new Dictionary<char, Node>();

            /// <summary>
            /// The "failure link": the node for the longest proper suffix of this node's prefix that is also a prefix of some pattern.
            /// </summary>
            public Node Fail;

            /// <summary>
            /// Indicates whether reaching this node means that a whole pattern was matched.
            /// </summary>
            public bool IsTerminal;

            /// <summary>
            /// The dense integer id assigned to this node (see <see cref="CompileToArrays"/>). <c>-1</c> means "not yet assigned".
            /// Storing it directly on the node avoids a separate <c>Dictionary&lt;Node, int&gt;</c> lookup.
            /// </summary>
            public int Id = -1;
        }
#pragma warning restore SA1401 // Fields should be private
    }
}
