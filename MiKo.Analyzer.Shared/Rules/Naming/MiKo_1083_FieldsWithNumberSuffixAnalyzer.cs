using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1083_FieldsWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1083";

        public MiKo_1083_FieldsWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.Type.Name.EndsWithNumber())
            {
                if (symbol.Type.TypeKind == TypeKind.Struct && symbol.ContainingType.IsTestClass())
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

            if (symbolName.EndsWithCommonNumber())
            {
                return new[] { Issue(symbol, CreateBetterNameProposal(symbolName.WithoutNumberSuffix())) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}