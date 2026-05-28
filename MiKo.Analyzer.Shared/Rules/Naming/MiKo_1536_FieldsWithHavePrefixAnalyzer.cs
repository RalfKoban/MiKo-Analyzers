using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1536_FieldsWithHavePrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1536";

        public MiKo_1536_FieldsWithHavePrefixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;
            var prefix = GetFieldPrefix(name);

            if (name.AsSpan(prefix.Length).StartsWith(Constants.Markers.Have, StringComparison.OrdinalIgnoreCase))
            {
                var betterName = NamesFinder.FindBetterNameForHavePrefix(name, prefix);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}