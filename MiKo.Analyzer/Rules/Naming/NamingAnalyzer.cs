using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        protected NamingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method, bool isEnabledByDefault = true) : base(nameof(Naming), diagnosticId, kind, isEnabledByDefault)
        {
        }

        protected IEnumerable<Diagnostic> AnalyzeEntityMarkers(ISymbol symbol)
        {
            if (!symbol.Name.HasEntityMarker()) return Enumerable.Empty<Diagnostic>();

            var expected = HandleSpecialEntityMarkerSituations(symbol.Name.RemoveAll(Constants.EntityMarkers));

            if (expected.HasCollectionMarker())
                expected = FindPluralName(expected, StringComparison.OrdinalIgnoreCase, Constants.CollectionMarkers);

            return new[] { ReportIssue(symbol, expected) };

        }

        private static string HandleSpecialEntityMarkerSituations(string name)
        {
            const string Item = "item";

            switch (name.Length)
            {
                case 0: return Item;
                case 1:
                    switch (name[0])
                    {
                        case 's': return Item + "s";
                        case '_': return "_" + Item;
                        default: return name;
                    }
                case 2:
                    switch (name)
                    {
                        case "s_": return "s_" + Item;
                        case "m_": return "m_" + Item;
                        default: return name;
                    }
                default: return name;
            }
        }

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol) => Constants.CollectionMarkers.Select(suffix => AnalyzeCollectionSuffix(symbol, suffix)).FirstOrDefault(_ => _ != null);

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var betterName = FindPluralName(symbol.Name, comparison, suffix);
            return betterName.IsNullOrWhiteSpace() ? null : ReportIssue(symbol, betterName);
        }

        protected static string FindPluralName(string symbolName, StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] suffixes)
        {
            foreach (var suffix in suffixes)
            {
                if (!symbolName.EndsWith(suffix, comparison)) continue;

                if (symbolName == "blackList" || symbolName == "whiteList") continue;

                var length = symbolName.Length - suffix.Length;
                if (length <= 0) continue;

                var proposedName = symbolName.Substring(0, length);
                if (symbolName.IsEntityMarker())
                    proposedName = proposedName.RemoveAll(Constants.EntityMarkers);

                return GetPluralName(symbolName, proposedName, comparison);
            }

            return null;
        }

        protected static string GetPluralName(string symbolName, string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => PluralNames.GetOrAdd(symbolName, _ => CreatePluralName(proposedName, comparison));

        private static string CreatePluralName(string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (proposedName.EndsWith("y", comparison)) return proposedName.Substring(0, proposedName.Length - 1) + "ies";
            if (proposedName.EndsWith("ss", comparison)) return proposedName + "es";
            if (proposedName.EndsWith("ed", comparison)) return proposedName;
            if (proposedName.EndsWith("child", comparison)) return proposedName + "ren";
            if (proposedName.EndsWith("children", comparison)) return proposedName;
            if (proposedName.EndsWith("complete", comparison)) return "all";
            if (proposedName.EndsWith("Data", comparison)) return proposedName;
            if (proposedName.EndsWith("Datas", comparison)) return proposedName.Substring(0, proposedName.Length - 1);
            if (proposedName.EndsWith("ndex", comparison)) return proposedName.Substring(0, proposedName.Length - 2) + "ices";
            if (proposedName.EndsWith("nformation", comparison)) return proposedName;
            if (proposedName.EndsWith("nformations", comparison)) return proposedName.Substring(0, proposedName.Length - 1);

            var pluralName = proposedName;
            if (proposedName.EndsWith("ToConvert", comparison))
                pluralName = proposedName.Substring(0, proposedName.Length - "ToConvert".Length);

            if (proposedName.EndsWith("ToModel", comparison))
                pluralName = proposedName.Substring(0, proposedName.Length - "ToModel".Length);

            if (proposedName.HasEntityMarker())
                pluralName = proposedName.RemoveAll(Constants.EntityMarkers);

            var candidate = pluralName.EndsWith("s", comparison) ? pluralName : pluralName + "s";

            if (candidate.Equals("sources", comparison)) return "source"; // special handling
            if (candidate.Equals("_sources", comparison)) return "_source"; // special handling

            return candidate;
        }
    }
}