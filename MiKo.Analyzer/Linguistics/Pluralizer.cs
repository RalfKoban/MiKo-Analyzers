﻿using System;
using System.Collections.Concurrent;

namespace MiKoSolutions.Analyzers.Linguistics
{
    public static class Pluralizer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        private static readonly string[] AllowedListNames =
            {
                "map",
                "array",
                "collection",
                "dictionary",
                "list",
                "blackList",
                "whiteList",
                "playList",
                "stack",
            };

        public static string GetPluralName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => GetPluralName(name, name, comparison);

        public static string GetPluralName(string name, string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => PluralNames.GetOrAdd(name, _ => CreatePluralName(proposedName, comparison));

        public static string GetPluralName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] suffixes)
        {
            if (IsAllowedListName(name, comparison))
            {
                return null;
            }

            foreach (var suffix in suffixes)
            {
                if (name.EndsWith(suffix, comparison))
                {
                    var proposedName = name.WithoutSuffix(suffix);

                    if (name.IsEntityMarker())
                    {
                        proposedName = proposedName.Without(Constants.Markers.Entities);
                    }

                    return GetPluralName(name, proposedName, comparison);
                }
            }

            return null;
        }

        private static bool IsAllowedListName(string symbolName, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => symbolName.EqualsAny(AllowedListNames, comparison);

#pragma warning disable CA1502 // Avoid excessive complexity
        private static string CreatePluralName(string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (proposedName.EndsWith("ay", comparison))
            {
                return proposedName + "s";
            }

            if (proposedName.EndsWith("ey", comparison))
            {
                return proposedName + "s";
            }

            if (proposedName.EndsWith("y", comparison))
            {
                return proposedName.WithoutSuffix("y") + "ies";
            }

            if (proposedName.EndsWith("ays", comparison))
            {
                return proposedName;
            }

            if (proposedName.EndsWith("eys", comparison))
            {
                return proposedName;
            }

            if (proposedName.EndsWith("ys", comparison))
            {
                return proposedName.WithoutSuffix("ys") + "ies";
            }

            if (proposedName.EndsWith("ss", comparison))
            {
                return proposedName + "es";
            }

            if (proposedName.EndsWith("sh", comparison))
            {
                return proposedName + "es";
            }

            if (proposedName.EndsWith("ed", comparison))
            {
                return proposedName;
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

            if (proposedName.EndsWith("Datas", comparison))
            {
                return proposedName.WithoutSuffix("s");
            }

            if (proposedName.EndsWith("ndex", comparison))
            {
                return proposedName.WithoutSuffix("ex") + "ices";
            }

            if (proposedName.EndsWith("nformation", comparison))
            {
                return proposedName;
            }

            if (proposedName.EndsWith("nformations", comparison))
            {
                return proposedName.WithoutSuffix("s");
            }

            var pluralName = proposedName;
            if (proposedName.EndsWith("ToConvert", comparison))
            {
                pluralName = proposedName.WithoutSuffix("ToConvert");
            }

            if (proposedName.EndsWith("ToModel", comparison))
            {
                pluralName = proposedName.WithoutSuffix("ToModel");
            }

            if (pluralName.HasEntityMarker())
            {
                pluralName = pluralName.Without(Constants.Markers.Entities);
            }

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

            var firstWord = pluralName.FirstWord();
            if (firstWord.EndsWith("s", comparison))
            {
                // TODO: RKN check MiKo_1070 for spelling mistakes
                return pluralName;
            }

            return candidate;
        }
#pragma warning restore CA1502
    }
}