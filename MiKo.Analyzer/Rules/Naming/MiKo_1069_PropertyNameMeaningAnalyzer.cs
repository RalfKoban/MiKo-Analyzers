using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1069_PropertyNameMeaningAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1069";

        private static readonly HashSet<string> WellknownNames = new HashSet<string>();

        public MiKo_1069_PropertyNameMeaningAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && WellknownNames.Contains(symbol.Name) is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol)
        {
            var returnType = symbol.GetReturnType();
            if (returnType is null)
            {
                // may happen during typing
                return Enumerable.Empty<Diagnostic>();
            }

            return symbol.NameMatchesTypeName(returnType, 2)
                       ? new[] { Issue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}