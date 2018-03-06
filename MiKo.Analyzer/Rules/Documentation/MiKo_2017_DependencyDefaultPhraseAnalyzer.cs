using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2017_DependencyDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2017";

        private const string DependencyPropertyFieldSuffix = "Property";

        public MiKo_2017_DependencyDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyzeField(IFieldSymbol symbol) => symbol.Type.Name == "System.Windows.DependencyObject";

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var symbolName = symbol.Name;
            if (!symbolName.EndsWith(DependencyPropertyFieldSuffix, StringComparison.OrdinalIgnoreCase)) return Enumerable.Empty<Diagnostic>();

            var propertyName = symbolName.WithoutSuffix(DependencyPropertyFieldSuffix);
            if (symbol.ContainingType.GetMembers().OfType<IPropertySymbol>().All(_ => _.Name != propertyName)) return Enumerable.Empty<Diagnostic>();

            // TODO: RKN loop over phrases for summaries and values

            var summaryPhrase = $"Identifies the <see cref=\"{propertyName}\"/> dependency property.";
            var valuePhrase = $"The identifier for the <see cref=\"{propertyName}\"/> dependency property.";

            var summaries = GetSummaries(commentXml);
            var results = summaries.Any() && summaries.All(_ => _ != summaryPhrase)
                              ? new List<Diagnostic> { ReportIssue(symbol, symbolName, "summary", summaryPhrase) }
                              : null;

            // check for field
            var comments = GetComments(commentXml, "value").ToImmutableHashSet();
            if (comments.Any() && comments.All(_ => _ != valuePhrase))
            {
                if (results == null) results = new List<Diagnostic>();
                results.Add(ReportIssue(symbol, symbolName, "value", valuePhrase));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}