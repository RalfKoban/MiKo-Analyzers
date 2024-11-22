using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1054_HelperClassNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1054";

        private const string CorrectName = "Utilization";

        private static readonly string[] WrongNames = { "Helper", "Util" };

        // sorted by intent so that the best match is found until a more generic is found
        private static readonly string[] WrongNamesForConcreteLookup = { "Helpers", "Helper", "Utils", "Utility", "Utilities", "Util" };

        public MiKo_1054_HelperClassNameAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.Contains(CorrectName))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbolName.ContainsAny(WrongNames))
            {
                var wrongName = WrongNamesForConcreteLookup.First(_ => symbolName.Contains(_, StringComparison.OrdinalIgnoreCase));

                var proposal = symbolName.WithoutSuffix(wrongName);

                return new[] { Issue(symbol, wrongName, CreateBetterNameProposal(proposal)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}