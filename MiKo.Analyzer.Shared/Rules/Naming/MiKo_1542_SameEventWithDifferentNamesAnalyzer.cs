using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1542_SameEventWithDifferentNamesAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1542";

        private static readonly ConcurrentDictionary<string, string> EventNamesCache = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public MiKo_1542_SameEventWithDifferentNamesAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            // only report those that have events defined
            var ownEvents = symbol.GetMembers<IEventSymbol>();

            if (ownEvents.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            // only report those that occur more than once
            var eventsWithSameName = symbol.GetEventsIncludingInherited()
                                           .GroupBy(FindName)
                                           .Where(_ => _.Count() > 1)
                                           .ToList();

            if (eventsWithSameName.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> issues = new List<Diagnostic>();

            // but only report those that are in the same type to avoid exceptions being thrown because the type e.g. has no source location in the analyzed file (compilation unit)
            foreach (var grouped in eventsWithSameName)
            {
                issues.AddRange(grouped.Where(ownEvents.Contains).Where(_ => _.IsInterfaceImplementation() is false).Select(_ => Issue(_)));
            }

            return issues;
        }

        private static string FindName(IEventSymbol e) => EventNamesCache.GetOrAdd(e.Name, _ => NamesFinder.FindBetterEventName(_).ToUpperInvariant());
    }
}