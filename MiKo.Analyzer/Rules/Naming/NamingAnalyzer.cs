using System;
using System.Collections.Concurrent;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        protected NamingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method, bool isEnabledByDefault = true) : base(nameof(Naming), diagnosticId, kind, isEnabledByDefault)
        {
        }

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol) => AnalyzeCollectionSuffix(symbol, "List")
                                                                     ?? AnalyzeCollectionSuffix(symbol, "Dictionary")
                                                                     ?? AnalyzeCollectionSuffix(symbol, "ObservableCollection")
                                                                     ?? AnalyzeCollectionSuffix(symbol, "Collection")
                                                                     ?? AnalyzeCollectionSuffix(symbol, "Array")
                                                                     ?? AnalyzeCollectionSuffix(symbol, "HashSet");

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var symbolName = symbol.Name;
            if (!symbolName.EndsWith(suffix, comparison)) return null;

            if (symbolName == "blackList" || symbolName == "whiteList")
            {
                return null;
            }

            var length = symbolName.Length - suffix.Length;
            if (length <= 0) return null; // use complete symbol name

            var proposedName = symbolName.Substring(0, length);
            if (symbolName.IsEntityMarker())
                proposedName = proposedName.Substring(0, proposedName.Length - 5);

            var betterName = PluralNames.GetOrAdd(symbolName, _ => GetPluralName(proposedName, comparison));
            return ReportIssue(symbol, betterName);
        }

        protected static string GetPluralName(string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
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

            var candidate = pluralName.EndsWith("s", comparison) ? pluralName : pluralName + "s";

            if (candidate.Equals("sources", comparison)) return "source"; // special handling
            if (candidate.Equals("_sources", comparison)) return "_source"; // special handling

            return candidate;
        }
    }
}