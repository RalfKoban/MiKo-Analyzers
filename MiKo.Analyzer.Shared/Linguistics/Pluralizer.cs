using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class Pluralizer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        private static readonly HashSet<char> CharsForTwoCharacterEndingsWithS = new HashSet<char> { 'a', 'h', 'i', 'o', 's', 'u', 'x', 'z' };

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

        private static readonly string[] PluralEndings = { "gers", "tchers", "pters", "stors", "ptors", "tures", "ties", "dges", "rges", "sages" };
        private static readonly string[] NonPluralEndings = { "ges", "nues", "curs", "opts", "nforms", "ses" };
        private static readonly string[] SpecialPluralWords = { "ages", "loot" };
        private static readonly string[] SpecialNonPluralWords = { "does", "lets" };
        private static readonly string[] SpecialPluralEndingsWithS = { "deas", "llas" };
        private static readonly string[] SingularOrPluralEndings = { "data", "heep", "moose", "trivia", "ircraft", "nformation", "nested" };
        private static readonly string[] SpecialPluralEndingsWithoutS = new[] { "cti", "men", "ngi", "dren", "eeth", "feet", "mena", "mice", "eople", "teria" }.Concat(SingularOrPluralEndings).ToArray();

        public static bool IsSingularAndPlural(ReadOnlySpan<char> value) => IsPlural(value) && value.EndsWithAny(SingularOrPluralEndings);

        public static bool IsPlural(ReadOnlySpan<char> value)
        {
            if (value.Length <= 2)
            {
                return false;
            }

            if (value.EndsWith('s'))
            {
                if (value.EqualsAny(SpecialPluralWords, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (value.EqualsAny(SpecialNonPluralWords, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (CharsForTwoCharacterEndingsWithS.Contains(value[value.Length - 2]))
                {
                    return value.EndsWithAny(SpecialPluralEndingsWithS);
                }

                if (value.EndsWithAny(PluralEndings))
                {
                    return true;
                }

                if (value.EndsWithAny(NonPluralEndings))
                {
                    return false;
                }

                return true;
            }

            return value.EndsWithAny(SpecialPluralEndingsWithoutS);
        }

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
            if (name.EndsWithAny(SpecialPluralEndingsWithoutS, comparison))
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
            if (proposedName.EqualsAny(SpecialPluralWords, StringComparison.OrdinalIgnoreCase))
            {
                return proposedName.ToString();
            }

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

                if (proposedName.EndsWith("oy", comparison))
                {
                    return proposedName.ConcatenatedWith('s');
                }

                return proposedName.WithoutSuffix('y').ConcatenatedWith("ies");
            }

            if (proposedName.EndsWith("s", comparison))
            {
                if (proposedName.EndsWith("us", comparison))
                {
                    return proposedName.WithoutSuffix("us").ConcatenatedWith('i');
                }

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

                if (proposedName.EndsWith("sis", comparison))
                {
                    return proposedName.WithoutSuffix("sis").ConcatenatedWith("ses");
                }

                if (proposedName.EndsWith("Datas", comparison))
                {
                    return proposedName.WithoutSuffix('s').ToString();
                }

                if (proposedName.EndsWith("ices", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("nformations", comparison))
                {
                    return proposedName.WithoutSuffix('s').ToString();
                }
            }
            else
            {
                if (proposedName.EndsWith("ed", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("sh", comparison))
                {
                    return proposedName.ConcatenatedWith("es");
                }

                if (proposedName.EndsWith("ox", comparison))
                {
                    return proposedName.ConcatenatedWith("es");
                }

                if (proposedName.EndsWith("iz", comparison))
                {
                    return proposedName.ConcatenatedWith("zes");
                }

                if (proposedName.EndsWith("tz", comparison))
                {
                    return proposedName.ConcatenatedWith("es");
                }

                if (proposedName.EndsWith("ife", comparison))
                {
                    return proposedName.WithoutSuffix("fe").ConcatenatedWith("ves");
                }

                if (proposedName.EndsWith("ium", comparison))
                {
                    return proposedName.WithoutSuffix("um").ConcatenatedWith('a');
                }

                if (proposedName.EndsWith("man", comparison))
                {
                    return proposedName.WithoutSuffix("an").ConcatenatedWith("en");
                }

                if (proposedName.EndsWith("olf", comparison))
                {
                    return proposedName.WithoutSuffix('f').ConcatenatedWith("ves");
                }

                if (proposedName.EndsWith("oot", comparison))
                {
                    return proposedName.WithoutSuffix("oot").ConcatenatedWith("eet");
                }

                if (proposedName.EndsWith("ooth", comparison))
                {
                    return proposedName.WithoutSuffix("ooth").ConcatenatedWith("eeth");
                }

                if (proposedName.EndsWith("Data", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("ndex", comparison))
                {
                    return proposedName.WithoutSuffix("ex").ConcatenatedWith("ices");
                }

                if (proposedName.EndsWith("oose", comparison))
                {
                    return proposedName.WithoutSuffix("oose").ConcatenatedWith("eese");
                }

                if (proposedName.EndsWith("rion", comparison))
                {
                    return proposedName.WithoutSuffix("on").ConcatenatedWith('a');
                }

                if (proposedName.EndsWith("child", comparison))
                {
                    return proposedName.ConcatenatedWith("ren");
                }

                if (proposedName.EndsWith("menon", comparison))
                {
                    return proposedName.WithoutSuffix("on").ConcatenatedWith("a");
                }

                if (proposedName.EndsWith("mouse", comparison))
                {
                    return proposedName.WithoutSuffix("ouse").ConcatenatedWith("ice");
                }

                if (proposedName.EndsWith("rivia", comparison))
                {
                    return proposedName.ToString(); // keep 'trivia'
                }

                if (proposedName.EndsWith("person", comparison))
                {
                    return proposedName.WithoutSuffix("rson").ConcatenatedWith("ople");
                }

                if (proposedName.EndsWith("children", comparison))
                {
                    return proposedName.ToString();
                }

                if (proposedName.EndsWith("complete", comparison))
                {
                    return "all";
                }

                if (proposedName.EndsWith("nformation", comparison))
                {
                    return proposedName.ToString();
                }
            }

            var pluralName = proposedName.ToString();

            if (proposedName.EndsWith("s", comparison) is false)
            {
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
