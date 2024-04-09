using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1030_BaseTypePrefixSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1030";

        public MiKo_1030_BaseTypePrefixSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name.Without("Abstraction").Replace("BasedOn", "#");

            foreach (var marker in Constants.Markers.BaseClasses)
            {
                if (symbolName.Contains(marker))
                {
                    var betterName = symbolName.Without(Constants.Markers.BaseClasses);

                    yield return Issue(symbol, marker, CreateBetterNameProposal(betterName));
                }
            }
        }
    }
}