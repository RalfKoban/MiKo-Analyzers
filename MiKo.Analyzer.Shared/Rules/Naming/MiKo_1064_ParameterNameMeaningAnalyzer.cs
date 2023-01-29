using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1064_ParameterNameMeaningAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1064";

        private static readonly HashSet<string> WellknownNames = new HashSet<string>
                                                                     {
                                                                         "command",
                                                                         "cancellationToken",
                                                                         "formatProvider",
                                                                         "progress",
                                                                         "project",
                                                                         "semanticModel",
                                                                         "symbol",
                                                                     };

        public MiKo_1064_ParameterNameMeaningAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (base.ShallAnalyze(symbol))
            {
                if (WellknownNames.Contains(symbol.Name))
                {
                    // ignore those that we cannot change
                    return false;
                }

                if (symbol.Name.EndsWith("anager", StringComparison.OrdinalIgnoreCase))
                {
                    // ignore managers as there is probably no better name available
                    return false;
                }

                return symbol.NameMatchesTypeName(symbol.Type);
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            // only investigate into more deep analysis if the name matches
            var method = symbol.GetEnclosingMethod();

            if (method.IsConstructor())
            {
                if (symbol.MatchesProperty() || symbol.MatchesField())
                {
                    // ignore those ctor parameters that get assigned to a property having the same name
                    yield break;
                }
            }

            if (method.IsOverride || method.IsInterfaceImplementation())
            {
                // ignore overrides/interfaces as the signatures should match the base signature
                yield break;
            }

            yield return Issue(symbol);
        }
    }
}