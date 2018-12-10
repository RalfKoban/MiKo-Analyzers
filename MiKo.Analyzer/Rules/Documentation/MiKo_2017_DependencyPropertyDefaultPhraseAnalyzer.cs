using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2017";

        private const string DependencyPropertyFieldSuffix = "Property";

        public MiKo_2017_DependencyPropertyDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyzeField(IFieldSymbol symbol) => symbol.Type.IsDependencyProperty();

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var symbolName = symbol.Name;
            if (!symbolName.EndsWith(DependencyPropertyFieldSuffix, StringComparison.OrdinalIgnoreCase)) return Enumerable.Empty<Diagnostic>();

            var propertyName = symbolName.WithoutSuffix(DependencyPropertyFieldSuffix);
            var containingType = symbol.ContainingType;
            if (containingType.GetMembers().OfType<IPropertySymbol>().All(_ => _.Name != propertyName)) return Enumerable.Empty<Diagnostic>();

            List<Diagnostic> results = null;

            var containingTypeFullName = containingType.ToString();

            // loop over phrases for summaries and values
            ValidatePhrases(symbol, GetSummaries(commentXml), () => Phrases(Constants.Comments.DependencyPropertyFieldSummaryPhrase, containingTypeFullName, propertyName), Constants.XmlTag.Summary, ref results);
            ValidatePhrases(symbol, GetComments(commentXml, Constants.XmlTag.Value), () => Phrases(Constants.Comments.DependencyPropertyFieldValuePhrase, containingTypeFullName, propertyName), Constants.XmlTag.Value, ref results);

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private void ValidatePhrases(IFieldSymbol symbol, IEnumerable<string> comments, Func<IList<string>> phrasesProvider, string xmlElement, ref List<Diagnostic> results)
        {
            var phrases = phrasesProvider();
            foreach (var comment in comments)
            {
                if (phrases.Any(_ => comment.StartsWith(_, StringComparison.Ordinal)))
                {
                    return;
                }

                if (results == null) results = new List<Diagnostic>();
                results.Add(ReportIssue(symbol, symbol.Name, xmlElement, phrases[0]));
            }
        }

        private static List<string> Phrases(string[] phrases, string typeName, string propertyName)
        {
            var results = new List<string>(2 * phrases.Length);
            results.AddRange(phrases.Select(_ => string.Format(_, propertyName))); // output as message to user
            results.AddRange(phrases.Select(_ => string.Format(_, typeName + "." + propertyName)));
            return results;
        }
    }
}