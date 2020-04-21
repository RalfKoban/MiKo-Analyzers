﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1094_NamespaceAsClassSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1094";

        private static readonly Dictionary<string, string> Suffixes = new Dictionary<string, string>
                                                                          {
                                                                              { "Management", "Manager" },
                                                                              { "Handling", "Handler" },
                                                                          };

        public MiKo_1094_NamespaceAsClassSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol)
        {
            var symbolName = symbol.Name;

            foreach (var pair in Suffixes)
            {
                if (symbolName.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var shortened = symbolName.WithoutSuffix(pair.Key);

                    yield return Issue(symbol, shortened + pair.Value);
                }
            }
        }
    }
}