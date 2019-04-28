using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1053_DelegateFieldNameSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1053";

        private static readonly string[] WrongNames = { "Action", "Delegate", "Func" };

        public MiKo_1053_DelegateFieldNameSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol)
        {
            var symbolType = symbol.Type;
            switch (symbolType.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Class when symbolType.ToString() == TypeNames.Delegate:
                    return symbol.Name.EndsWithAny(WrongNames)
                               ? new[] { Issue(symbol) }
                               : Enumerable.Empty<Diagnostic>();

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}