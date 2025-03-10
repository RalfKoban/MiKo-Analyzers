using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_1505_FieldsWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1505";

        private const string Counter = "Counter";

        public MiKo_1505_FieldsWithCounterSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.Type.TypeKind == TypeKind.Struct && symbol.Name.EndsWith(Counter, StringComparison.OrdinalIgnoreCase))
            {
                if (symbol.ContainingType.IsTestClass())
                {
                    // ignore only structs in tests
                    return false;
                }

                return true;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbol.Type.TypeKind == TypeKind.Struct && symbolName.EndsWith(Counter, StringComparison.OrdinalIgnoreCase))
            {
                var prefix = GetFieldPrefix(symbolName);

                // be aware of field prefixes, such as 'm_'
                var betterName = prefix + "counted" + symbolName.AsSpan().Slice(prefix.Length, symbolName.Length - prefix.Length - Counter.Length).ToUpperCaseAt(0);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}