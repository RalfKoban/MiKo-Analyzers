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

        protected override bool ShallAnalyzeField(IFieldSymbol symbol) => symbol.Type.Name == "DependencyProperty" || symbol.Type.Name == "System.Windows.DependencyProperty";

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var symbolName = symbol.Name;
            if (!symbolName.EndsWith(DependencyPropertyFieldSuffix, StringComparison.OrdinalIgnoreCase)) return Enumerable.Empty<Diagnostic>();

            var propertyName = symbolName.WithoutSuffix(DependencyPropertyFieldSuffix);
            var containingType = symbol.ContainingType;
            if (containingType.GetMembers().OfType<IPropertySymbol>().All(_ => _.Name != propertyName)) return Enumerable.Empty<Diagnostic>();

            var link = containingType.Name + "." + propertyName;

            List<Diagnostic> results = null;

            // loop over phrases for summaries and values
            ValidatePhrases(symbol, GetSummaries(commentXml), () => GetSummaryPhrases(link), "summary", ref results);
            ValidatePhrases(symbol, GetComments(commentXml, "value"), () => GetValuePhrases(link), "value", ref results);

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private void ValidatePhrases(IFieldSymbol symbol, IEnumerable<string> comments, Func<IList<string>> phrasesProvider, string xmlElement, ref List<Diagnostic> results)
        {
            var phrases = phrasesProvider();
            foreach (var comment in comments)
            {
                foreach (var phrase in phrases)
                {
                    if (phrase == comment) return;
                }

                if (results == null) results = new List<Diagnostic>();
                results.Add(ReportIssue(symbol, symbol.Name, xmlElement, phrases[0]));
            }
        }

        private static List<string> GetSummaryPhrases(string link) => Constants.Comments.DependencyPropertyFieldSummaryPhrase.Select(phrase => string.Format(phrase, link)).ToList();

        private static List<string> GetValuePhrases(string link) => Constants.Comments.DependencyPropertyFieldValuePhrase.Select(phrase => string.Format(phrase, link)).ToList();
    }
}