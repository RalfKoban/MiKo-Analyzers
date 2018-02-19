using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1041_FieldCollectionSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1041";

        public MiKo_1041_FieldCollectionSuffixAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol)
        {
            var diagnostic = AnalyzeCollectionSuffix(symbol);
            return diagnostic != null
                       ? new[] { diagnostic }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}