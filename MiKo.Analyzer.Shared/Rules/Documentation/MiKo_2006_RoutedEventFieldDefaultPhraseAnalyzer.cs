using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2006";

        public MiKo_2006_RoutedEventFieldDefaultPhraseAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol) => symbol.Type.IsRoutedEvent() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Constants.RoutedEventFieldSuffix, StringComparison.OrdinalIgnoreCase))
            {
                var eventName = symbolName.WithoutSuffix(Constants.RoutedEventFieldSuffix);
                var containingType = symbol.ContainingType;

                if (containingType.GetMembersIncludingInherited<IEventSymbol>(eventName).Any())
                {
                    var containingTypeFullName = containingType.ToString();

                    // loop over phrases for summaries and values
                    var summaries = CommentExtensions.GetSummaries(commentXml);

                    if (summaries.Any())
                    {
                        var summaryPhrases = Phrases(Constants.Comments.RoutedEventFieldSummaryPhrase, containingTypeFullName, eventName);

                        if (summaries.Any(summary => summaryPhrases.None(_ => summary.StartsWith(_, StringComparison.Ordinal))))
                        {
                            yield return Issue(symbol, Constants.XmlTag.Summary, summaryPhrases[0]);
                        }
                    }

                    var values = CommentExtensions.GetValue(commentXml);

                    if (values.Any())
                    {
                        var valuePhrases = Phrases(Constants.Comments.RoutedEventFieldValuePhrase, containingTypeFullName, eventName);

                        if (values.Any(value => valuePhrases.None(_ => value.StartsWith(_, StringComparison.Ordinal))))
                        {
                            yield return Issue(symbol, Constants.XmlTag.Value, valuePhrases[0]);
                        }
                    }
                }
                else
                {
                    // it's an unknown event
                }
            }
        }

        private static List<string> Phrases(string[] phrases, string typeName, string eventName)
        {
            var results = new List<string>(2 * phrases.Length);
            results.AddRange(phrases.Select(_ => _.FormatWith(eventName))); // output as message to user
            results.AddRange(phrases.Select(_ => _.FormatWith(typeName + "." + eventName)));

            return results;
        }
    }
}