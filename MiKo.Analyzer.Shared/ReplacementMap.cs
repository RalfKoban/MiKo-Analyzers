using System;

namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Represents a named map of terms and their replacements to look up and replace specific text fragments, such as terms within documentation comments.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class ReplacementMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplacementMap"/> class with an identifier and the terms to replace.
        /// </summary>
        /// <param name="id">
        /// The identifier of the replacement map.
        /// </param>
        /// <param name="pairs">
        /// The map of terms to their replacements (<see cref="Pair.Key"/> = original, <see cref="Pair.Value"/> = replacement).
        /// </param>
        /// <param name="keys">
        /// The keys to look up the corresponding replacements in <paramref name="pairs"/> (see <see cref="Keys"/>).
        /// </param>
        public ReplacementMap(string id, Pair[] pairs, string[] keys)
        {
            Id = id;
            Pairs = pairs;
            Keys = keys;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplacementMap"/> class with an identifier and the terms to replace.
        /// </summary>
        /// <param name="id">
        /// The identifier of the replacement map.
        /// </param>
        /// <param name="pairs">
        /// The map of terms to their replacements (<see cref="Pair.Key"/> = original, <see cref="Pair.Value"/> = replacement).
        /// </param>
        /// <param name="keysSelector">
        /// The callback that selects the keys to look up the corresponding replacements in <paramref name="pairs"/> (see <see cref="Keys"/>).
        /// </param>
        public ReplacementMap(string id, Pair[] pairs, Func<Pair[], string[]> keysSelector)
        {
            Id = id;
            Pairs = pairs;
            Keys = keysSelector(pairs);
        }

        /// <summary>
        /// Gets the identifier of the replacement map.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that contains the identifier of the replacement map.
        /// </value>
        public string Id { get; }

#pragma warning disable CA1819
        /// <summary>
        /// Gets the map of terms to their replacements (<see cref="Pair.Key"/> = original, <see cref="Pair.Value"/> = replacement).
        /// </summary>
        /// <value>
        /// An array of terms to their replacements (<see cref="Pair.Key"/> = original, <see cref="Pair.Value"/> = replacement).
        /// </value>
        public Pair[] Pairs { get; }

        /// <summary>
        /// Gets or sets the keys use to look-up for their replacements in <see cref="Pairs"/>.
        /// </summary>
        /// <value>
        /// An array of keys to look up the corresponding replacements in <see cref="Pairs"/>.
        /// </value>
        /// <remarks>
        /// The keys do not need to contain all keys of <see cref="Pairs"/>. Instead, they can contain the most common sub-sequences of those keys to optimize searching.
        /// </remarks>
        public string[] Keys { get; set; }
#pragma warning restore CA1819
    }
}