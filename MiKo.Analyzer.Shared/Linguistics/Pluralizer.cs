using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class Pluralizer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        private static readonly string[] AllowedNames =
                                                        {
                                                            "map",
                                                            "list",
                                                            "array",
                                                            "stack",
                                                            "playList",
                                                            "blackList",
                                                            "whiteList",
                                                            "collection",
                                                            "dictionary",
                                                        };

        private static readonly string[] AllowedListNames = Constants.Markers.FieldPrefixes.SelectMany(_ => AllowedNames, (prefix, name) => prefix + name).ToArray();

        /// <summary>
        /// Create a plural name for a given name, but returns the same name in case if fails to do so.
        /// </summary>
        /// <param name="name">
        /// The name to create a plural for.
        /// </param>
        /// <returns>
        /// The plural for the given <paramref name="name"/>.
        /// </returns>
        public static string MakePluralName(string name) => GetPluralName(name, StringComparison.Ordinal) ?? name;

        /// <summary>
        /// Attempts to create a plural name for a given name, but (in contrast to <see cref="MakePluralName"/>) returns <see langword="null"/> in case if fails to do so.
        /// </summary>
        /// <param name="name">
        /// The name to create a plural for.
        /// </param>
        /// <param name="comparison">
        /// One of the <see cref="StringComparison"/> values that specifies the rules for comparison.
        /// </param>
        /// <returns>
        /// The plural for the given <paramref name="name"/>.
        /// </returns>
        public static string GetPluralName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => GetPluralName(name, name, comparison);

        /// <summary>
        /// Attempts to create a plural name for a given proposed name, but (in contrast to <see cref="MakePluralName"/>) returns <see langword="null"/> in case if fails to do so.
        /// </summary>
        /// <param name="name">
        /// The name to create a plural for.
        /// </param>
        /// <param name="proposedName">
        /// The proposed name to create the plural name for.
        /// </param>
        /// <param name="comparison">
        /// One of the <see cref="StringComparison"/> values that specifies the rules for comparison.
        /// </param>
        /// <returns>
        /// The plural for the given <paramref name="name"/>.
        /// </returns>
        public static string GetPluralName(string name, string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (name.EndsWith("trivia", comparison))
            {
                return null;
            }

            return PluralNames.GetOrAdd(name, _ => CreatePluralName(proposedName.AsSpan(), comparison));
        }

        /// <summary>
        /// Attempts to create a plural name for a given name, but (in contrast to <see cref="MakePluralName"/>) returns <see langword="null"/> in case if fails to do so.
        /// </summary>
        /// <param name="name">
        /// The name to create a plural for.
        /// </param>
        /// <param name="comparison">
        /// One of the <see cref="StringComparison"/> values that specifies the rules for comparison.
        /// </param>
        /// <param name="suffixes">
        /// The suffixes to remove from the plural name.
        /// </param>
        /// <returns>
        /// The plural for the given <paramref name="name"/>.
        /// </returns>
        public static string GetPluralName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] suffixes)
        {
            if (IsAllowedListName(name, comparison))
            {
                return null;
            }

            var proposedName = name.AsSpan().WithoutSuffixes(suffixes);

            if (proposedName.EndsWithAny(Constants.Markers.Models) && proposedName.EndsWithAny(Constants.Markers.ViewModels) is false)
            {
                proposedName = proposedName.WithoutSuffixes(Constants.Markers.Models);
            }

            if (name.Length != proposedName.Length)
            {
                return GetPluralName(name, proposedName.ToString(), comparison);
            }

            return null;
        }

        private static bool IsAllowedListName(string symbolName, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => symbolName.EqualsAny(AllowedListNames, comparison);

        private static string CreatePluralName(ReadOnlySpan<char> proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (proposedName.EndsWith("y", comparison))
            {
                if (proposedName.EndsWith("ay", comparison))
                {
                    return proposedName.ConcatenatedWith('s');
                }

                if (proposedName.EndsWith("ey", comparison))
                {
                    return proposedName.ConcatenatedWith('s');
                }

                return proposedName.WithoutSuffix('y').ConcatenatedWith("ies");
            }

            var pluralName = proposedName.ToString();

            if (proposedName.EndsWith("s", comparison))
            {
                if (proposedName.EndsWith("ys", comparison))
                {
                    if (proposedName.EndsWith("ays", comparison))
                    {
                        return proposedName.ToString();
                    }

                    if (proposedName.EndsWith("eys", comparison))
                    {
                        return proposedName.ToString();
                    }

                    return proposedName.WithoutSuffix("ys").ConcatenatedWith("ies");
                }

                if (proposedName.EndsWith("ss", comparison))
                {
                    return proposedName.ConcatenatedWith("es");
                }

                if (proposedName.EndsWith("Datas", comparison))
                {
                    return proposedName.WithoutSuffix('s').ToString();
                }

                if (proposedName.EndsWith("nformations", comparison))
                {
                    return proposedName.WithoutSuffix('s').ToString();
                }
            }
            else
            {
                if (proposedName.EndsWith("sh", comparison))
                {
                    return proposedName.ConcatenatedWith("es");
                }

                if (proposedName.EndsWith("ed", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("rivia", comparison))
                {
                    return proposedName.ToString(); // keep 'trivia'
                }

                if (proposedName.EndsWith("child", comparison))
                {
                    return proposedName.ConcatenatedWith("ren");
                }

                if (proposedName.EndsWith("children", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("complete", comparison))
                {
                    return "all";
                }

                if (proposedName.EndsWith("Data", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("ndex", comparison))
                {
                    return proposedName.WithoutSuffix("ex").ConcatenatedWith("ices");
                }

                if (proposedName.EndsWith("nformation", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("ToConvert", comparison))
                {
                    pluralName = proposedName.WithoutSuffix("ToConvert").ToString();
                }
                else if (proposedName.EndsWith("ToModel", comparison))
                {
                    pluralName = proposedName.WithoutSuffix("ToModel").ToString();
                }
            }

            if (pluralName.HasEntityMarker())
            {
                pluralName = pluralName.Without(Constants.Markers.Models);
            }

            // we might end with an 's' when shortened, so inspect for that as well
            var candidate = pluralName.EndsWith("s", comparison) ? pluralName : pluralName + 's';

            if (candidate.Equals("bases", comparison))
            {
                return "items"; // special handling
            }

            if (candidate.Equals("_bases", comparison))
            {
                return "_items"; // special handling
            }

            if (candidate.Equals("m_bases", comparison))
            {
                return "m_items"; // special handling
            }

            if (candidate.Equals("sources", comparison))
            {
                return "source"; // special handling
            }

            if (candidate.Equals("_sources", comparison))
            {
                return "_source"; // special handling
            }

            if (candidate.Equals("m_sources", comparison))
            {
                return "m_source"; // special handling
            }

            // it might be that the plural name consists of multiple words (such as 'TestMe'), so just pick up the first word
            var firstWord = pluralName.FirstWord();

            if (firstWord.EndsWith("s", comparison))
            {
                // TODO: RKN check MiKo_1070 for spelling mistakes
                return pluralName;
            }

            return candidate;
        }
    }
}
