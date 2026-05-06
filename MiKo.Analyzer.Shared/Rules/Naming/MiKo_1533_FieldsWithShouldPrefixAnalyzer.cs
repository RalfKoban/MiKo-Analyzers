using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1533_FieldsWithShouldPrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1533";

        public MiKo_1533_FieldsWithShouldPrefixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var name = symbol.Name;
            var prefix = GetFieldPrefix(name);

            if (name.AsSpan(prefix.Length).StartsWithAny(Constants.Names.ShouldPrefixes, StringComparison.OrdinalIgnoreCase))
            {
                var betterName = FindBetterNameForShouldPrefix(name, prefix);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}