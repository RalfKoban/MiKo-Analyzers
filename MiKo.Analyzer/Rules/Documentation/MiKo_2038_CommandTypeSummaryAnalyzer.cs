using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2038_CommandTypeSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2038";

        public MiKo_2038_CommandTypeSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsCommand() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            const string Phrase = Constants.Comments.CommandSummaryStartingPhrase;

            if (summaries.None(_ => _.StartsWith(Phrase, StringComparison.Ordinal)))
            {
                return new[] { Issue(symbol, Constants.XmlTag.Summary, Phrase) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}