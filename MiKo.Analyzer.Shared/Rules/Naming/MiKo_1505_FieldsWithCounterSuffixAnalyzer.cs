using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_1505_FieldsWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1505";

        public MiKo_1505_FieldsWithCounterSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.Type.TypeKind == TypeKind.Struct && symbol.Name.EndsWith(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
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

            if (symbol.Type.TypeKind == TypeKind.Struct && symbolName.EndsWith(Constants.Names.Counter, StringComparison.OrdinalIgnoreCase))
            {
                var betterName = FindBetterName(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static string FindBetterName(string symbolName)
        {
            var prefix = GetFieldPrefix(symbolName);

            // be aware of field prefixes, such as 'm_'
            return prefix + "counted" + Pluralizer.MakePluralName(symbolName.AsSpan().Slice(prefix.Length, symbolName.Length - prefix.Length - Constants.Names.Counter.Length).ToUpperCaseAt(0));
        }
    }
}