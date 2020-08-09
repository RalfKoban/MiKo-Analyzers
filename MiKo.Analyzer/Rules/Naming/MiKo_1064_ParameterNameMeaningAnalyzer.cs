using System.Collections.Generic;
using System.Linq;

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

        protected override bool ShallAnalyze(IParameterSymbol symbol) => base.ShallAnalyze(symbol) && WellknownNames.Contains(symbol.Name) is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol)
        {
            var method = symbol.GetEnclosingMethod();

            if (method.MethodKind == MethodKind.Constructor)
            {
                if (symbol.MatchesProperty() || symbol.MatchesField())
                {
                    // ignore those ctor parameters that get assigned to a property having the same name
                    return Enumerable.Empty<Diagnostic>();
                }
            }

            if (method.IsOverride || method.IsInterfaceImplementation())
            {
                // ignore overrides/interfaces as the signatures should match the base signature
                return Enumerable.Empty<Diagnostic>();
            }

            return symbol.NameMatchesTypeName(symbol.Type)
                       ? new[] { Issue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}