using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2037_CommandPropertySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2037";

        public MiKo_2037_CommandPropertySummaryAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyzeProperty(IPropertySymbol symbol) => symbol.Type.Implements<ICommand>();

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries)
        {
            var phrases = GetStartingPhrase((IPropertySymbol)symbol);

            return summaries.All(_ => !_.StartsWithAny(StringComparison.Ordinal, phrases))
                       ? new[] { ReportIssue(symbol, Constants.XmlTag.Summary, phrases.First()) }
                       : Enumerable.Empty<Diagnostic>();
        }

        private static string[] GetStartingPhrase(IPropertySymbol symbol)
        {
            if (symbol.IsWriteOnly) return Constants.Comments.CommandPropertySetterOnlySummaryStartingPhrase;
            if (symbol.IsReadOnly) return Constants.Comments.CommandPropertyGetterOnlySummaryStartingPhrase;
            return Constants.Comments.CommandPropertyGetterSetterSummaryStartingPhrase;
        }
    }
}