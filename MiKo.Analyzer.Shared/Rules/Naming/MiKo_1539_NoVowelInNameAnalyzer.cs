using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1539_NoVowelInNameAnalyzer : OverallNameAnalyzer
    {
        public const string Id = "MiKo_1539";

        private static readonly HashSet<string> AllowedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                                                   {
                                                                       "By",
                                                                       "CRC",
                                                                       "CSV",
                                                                       "HTML",
                                                                       "HTTP",
                                                                       "HTTPS",
                                                                       "TCP",
                                                                       "tcs", // TaskCancellationSource
                                                                       "Try",
                                                                       "XML",
                                                                   };

        public MiKo_1539_NoVowelInNameAnalyzer() : base(Id)
        {
        }

        // overridden as it is not important how the prefix looks like, so we can save some memory by not creating a new string for the field name
        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol.Name, symbol);

        protected override Diagnostic[] AnalyzeName(string symbolName, ISymbol symbol, string prefix = "") => HasIssue(symbolName)
                                                                                                              ? new[] { Issue(symbol) }
                                                                                                              : Array.Empty<Diagnostic>();

        private static bool HasIssue(string name)
        {
            var nameToInspect = name.AsSpan().WithoutNumberSuffix();

            if (nameToInspect.Length <= 1)
            {
                // we do not care for single letter names (such as for 'i' inside loops)
                return false;
            }

            if (nameToInspect.IndexOfAny("AEIOUaeiou".AsSpan()) < 0)
            {
                return AllowedNames.Contains(nameToInspect.ToString()) is false;
            }

            return false;
        }
    }
}