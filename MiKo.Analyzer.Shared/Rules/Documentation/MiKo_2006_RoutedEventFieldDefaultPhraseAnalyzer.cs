using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2006";

        public MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IFieldSymbol field && field.Type.IsRoutedEvent();

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.RoutedEventFieldSuffix, StringComparison.OrdinalIgnoreCase) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            var eventName = symbolName.WithoutSuffix(Constants.RoutedEventFieldSuffix);
            var containingType = symbol.ContainingType;

            if (containingType.GetMembersIncludingInherited<IEventSymbol>(eventName).None())
            {
                // it's an unknown event
                return Array.Empty<Diagnostic>();
            }

            var containingTypeFullName = containingType.ToString();

            var issues = new List<Diagnostic>(2);

            // loop over phrases for summaries and values
            var commentXml = symbol.GetDocumentationCommentXml();
            var summaries = CommentExtensions.GetSummaries(commentXml);

            if (summaries.Count != 0)
            {
                var summaryPhrases = Phrases(Constants.Comments.RoutedEventFieldSummaryPhrase, containingTypeFullName, eventName);

                if (summaries.Any(summary => summaryPhrases.None(_ => summary.StartsWith(_, StringComparison.Ordinal))))
                {
                    issues.Add(Issue(symbol, Constants.XmlTag.Summary, summaryPhrases[0]));
                }
            }

            var values = CommentExtensions.GetValue(commentXml);

            if (values.Count != 0)
            {
                var valuePhrases = Phrases(Constants.Comments.RoutedEventFieldValuePhrase, containingTypeFullName, eventName);

                if (values.Any(value => valuePhrases.None(_ => value.StartsWith(_, StringComparison.Ordinal))))
                {
                    issues.Add(Issue(symbol, Constants.XmlTag.Value, valuePhrases[0]));
                }
            }

            return issues;
        }

        private static List<string> Phrases(string[] phrases, string typeName, string eventName)
        {
            var eventFullName = typeName + "." + eventName;

            var results = new List<string>(2 * phrases.Length);
            results.AddRange(phrases.Select(_ => _.FormatWith(eventName))); // output as message to user
            results.AddRange(phrases.Select(_ => _.FormatWith(eventFullName)));

            return results;
        }
    }
}