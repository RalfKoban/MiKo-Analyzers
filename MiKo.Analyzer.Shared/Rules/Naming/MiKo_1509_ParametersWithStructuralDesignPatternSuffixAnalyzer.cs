using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1509";

        private static readonly Dictionary<string, string> StructuralDesignPatternNames = new Dictionary<string, string>
                                                                                              {
                                                                                                  { Constants.Names.Patterns.Adapter, "adapted" },
                                                                                                  { Constants.Names.Patterns.Wrapper, "wrapped" },
                                                                                                  { Constants.Names.Patterns.Decorator, "decorated" },
                                                                                              };

        public MiKo_1509_ParametersWithStructuralDesignPatternSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol) => symbol.Name.EndsWithAny(StructuralDesignPatternNames.Keys, StringComparison.OrdinalIgnoreCase);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            var betterName = FindBetterName(symbol.Name);

            return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
        }

        private static string FindBetterName(string name)
        {
            foreach (var pair in StructuralDesignPatternNames)
            {
                if (name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var count = name.Length - pair.Key.Length;

                    if (count is 0)
                    {
                        return pair.Value;
                    }

                    return pair.Value
                               .AsCachedBuilder()
                               .Append(name, 0, count)
                               .ToUpperCaseAt(pair.Value.Length)
                               .ToString();
                }
            }

            return name;
        }
    }
}