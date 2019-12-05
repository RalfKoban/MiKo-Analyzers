using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1073";

        private static readonly string[] RawPrefixes =
            {
                "Is",
                "Are",
                "is",
                "are",
            };

        private static readonly string[] Prefixes = Constants.Markers.FieldPrefixes.SelectMany(_ => RawPrefixes, (prefix, name) => prefix + name).ToArray();

        public MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsBoolean();

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol) => AnalyzeName(symbol);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var name = symbol.Name;
            if (name.Length <= 5)
            {
                // skip all short names (such as isIP)
                yield break;
            }

            if (name.StartsWithAny(Prefixes, StringComparison.Ordinal) && name.HasUpperCaseLettersAbove(2))
            {
                yield return Issue(symbol);
            }
        }
    }
}