using System;
using System.Collections.Concurrent;

namespace MiKoSolutions.Analyzers.Linguistics
{
    internal static class Pluralizer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        private static readonly string[] AllowedListNames =
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

        public static string GetPluralName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => GetPluralName(name, name, comparison);

        public static string GetPluralName(string name, string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (name.EndsWith("trivia", comparison))
            {
                return null;
            }

            return PluralNames.GetOrAdd(name, _ => CreatePluralName(proposedName, comparison));
        }

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

        private static string CreatePluralName(string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (proposedName.EndsWith("y", comparison))
            {
                if (proposedName.EndsWith("ay", comparison))
                {
                    return proposedName + "s";
                }

                if (proposedName.EndsWith("ey", comparison))
                {
                    return proposedName + "s";
                }

                return proposedName.WithoutSuffix("y") + "ies";
            }

            var pluralName = proposedName;

            if (proposedName.EndsWith("s", comparison))
            {
                if (proposedName.EndsWith("ys", comparison))
                {
                    if (proposedName.EndsWith("ays", comparison))
                    {
                        return proposedName;
                    }

                    if (proposedName.EndsWith("eys", comparison))
                    {
                        return proposedName;
                    }

                    return proposedName.WithoutSuffix("ys") + "ies";
                }

                if (proposedName.EndsWith("ss", comparison))
                {
                    return proposedName + "es";
                }

                if (proposedName.EndsWith("Datas", comparison))
                {
                    return proposedName.WithoutSuffix("s");
                }

                if (proposedName.EndsWith("nformations", comparison))
                {
                    return proposedName.WithoutSuffix("s");
                }
            }
            else
            {
                if (proposedName.EndsWith("sh", comparison))
                {
                    return proposedName + "es";
                }

                if (proposedName.EndsWith("ed", comparison))
                {
                    return proposedName;
                }

                if (proposedName.EndsWith("rivia", comparison))
                {
                    return proposedName; // keep 'trivia'
                }

                if (proposedName.EndsWith("child", comparison))
                {
                    return proposedName + "ren";
                }

                if (proposedName.EndsWith("children", comparison))
                {
                    return proposedName;
                }

                if (proposedName.EndsWith("complete", comparison))
                {
                    return "all";
                }

                if (proposedName.EndsWith("Data", comparison))
                {
                    return proposedName;
                }

                if (proposedName.EndsWith("ndex", comparison))
                {
                    return proposedName.WithoutSuffix("ex") + "ices";
                }

                if (proposedName.EndsWith("nformation", comparison))
                {
                    return proposedName;
                }

                if (proposedName.EndsWith("ToConvert", comparison))
                {
                    pluralName = proposedName.WithoutSuffix("ToConvert");
                }
                else if (proposedName.EndsWith("ToModel", comparison))
                {
                    pluralName = proposedName.WithoutSuffix("ToModel");
                }
            }

            if (pluralName.HasEntityMarker())
            {
                pluralName = pluralName.Without(Constants.Markers.Models);
            }

            // we might end with an 's' when shortened, so inspect for that as well
            var candidate = pluralName.EndsWith("s", comparison) ? pluralName : pluralName + "s";

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
