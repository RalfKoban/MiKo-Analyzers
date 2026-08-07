using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a reusable multi-pattern matcher based on the Aho-Corasick algorithm that determines,
    /// in a single pass over the searched text, whether any of a fixed set of patterns is contained within it.
    /// </summary>
    /// <remarks>
    /// The matcher is built once from a set of patterns and can then be queried many times, each query costing
    /// <c>O(text length)</c> regardless of the number of patterns, instead of <c>O(patterns count * text length)</c>
    /// as with a naive per-pattern <see cref="string.IndexOf(string)"/> loop.
    /// Matching is performed using <see cref="StringComparison.Ordinal"/> semantics.
    /// </remarks>
    internal sealed class AhoCorasickMatcher
    {
        private readonly Node m_root = new Node();

        /// <summary>
        /// Initializes a new instance of the <see cref="AhoCorasickMatcher"/> class for the specified patterns.
        /// </summary>
        /// <param name="patterns">
        /// The patterns to seek for. <see langword="null"/> or empty entries are ignored.
        /// </param>
        private AhoCorasickMatcher(IEnumerable<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    continue;
                }

                var node = m_root;

                foreach (var c in pattern)
                {
                    if (node.Children.TryGetValue(c, out var next) is false)
                    {
                        next = new Node();

                        node.Children[c] = next;
                    }

                    node = next;
                }

                node.IsTerminal = true;
            }

            BuildFailureLinks();
        }

        /// <summary>
        /// Gets a new instance of the <see cref="AhoCorasickMatcher"/> class for the specified patterns.
        /// </summary>
        /// <param name="patterns">
        /// The patterns to seek for. <see langword="null"/> or empty entries are ignored.
        /// </param>
        /// <returns>
        /// The new created matcher initialized with the specified patterns.
        /// </returns>
        public static AhoCorasickMatcher For(IEnumerable<string> patterns) => new AhoCorasickMatcher(patterns);

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
                var c = text[index];

                while (node != m_root && node.Children.TryGetValue(c, out _) is false)
                {
                    node = node.Fail;
                }

                node = node.Children.TryGetValue(c, out var next) ? next : m_root;

                if (node.IsTerminal)
                {
                    return true;
                }
            }

            return false;
        }

        private void BuildFailureLinks()
        {
            var queue = new Queue<Node>();

            foreach (var child in m_root.Children.Values)
            {
                child.Fail = m_root;

                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var pair in current.Children)
                {
                    var c = pair.Key;
                    var child = pair.Value;

                    var failNode = current.Fail;

                    while (failNode != null && failNode.Children.TryGetValue(c, out _) is false)
                    {
                        failNode = failNode.Fail;
                    }

                    child.Fail = (failNode != null && failNode.Children.TryGetValue(c, out var failChild)) ? failChild : m_root;

                    if (child.Fail.IsTerminal)
                    {
                        child.IsTerminal = true;
                    }

                    queue.Enqueue(child);
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
