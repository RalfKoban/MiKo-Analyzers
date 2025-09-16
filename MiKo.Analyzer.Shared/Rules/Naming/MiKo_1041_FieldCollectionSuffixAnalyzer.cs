using System;
using System.Collections.Generic;

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

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.ContainingType?.IsEnum() is true)
            {
                return false; // ignore enum definitions
            }

            var type = symbol.Type;

            if (type.IsXmlNode())
            {
                return false;
            }

            if (type.IsString())
            {
                return symbol.Name.EndsWithAny(Constants.Markers.Collections);
            }

            return true;
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation)
        {
            var diagnostic = AnalyzeCollectionSuffix(symbol);

            return diagnostic is null
                   ? Array.Empty<Diagnostic>()
                   : new[] { diagnostic };
        }
    }
}