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

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            if (symbol.IsImport())
            {
                // ignore imports
                return Enumerable.Empty<Diagnostic>();
            }

            var returnType = symbol.GetReturnType();

            if (returnType is null)
            {
                // may happen during typing
                return Enumerable.Empty<Diagnostic>();
            }

            if (symbol.NameMatchesTypeName(returnType, 2))
            {
                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}