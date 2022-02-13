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

        protected override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            var symbolName = symbol.Name;
            if (symbolName.EndsWith(Constants.RoutedEventFieldSuffix, StringComparison.OrdinalIgnoreCase) is false)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var eventName = symbolName.WithoutSuffix(Constants.RoutedEventFieldSuffix);
            var containingType = symbol.ContainingType;
            if (containingType.GetMembersIncludingInherited<IEventSymbol>().All(_ => _.Name != eventName))
            {
                // it's an unknown event
                return Enumerable.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            var containingTypeFullName = containingType.ToString();

            // loop over phrases for summaries and values
            ValidatePhrases(symbol, comment.GetSummaryXmls(), () => Phrases(Constants.Comments.RoutedEventFieldSummaryPhrase, containingTypeFullName, eventName), Constants.XmlTag.Summary, ref results);
            ValidatePhrases(symbol, comment.GetValueXmls(), () => Phrases(Constants.Comments.RoutedEventFieldValuePhrase, containingTypeFullName, eventName), Constants.XmlTag.Value, ref results);

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private static List<string> Phrases(string[] phrases, string typeName, string eventName)
        {
            var results = new List<string>(2 * phrases.Length);
            results.AddRange(phrases.Select(_ => string.Format(_, eventName))); // output as message to user
            results.AddRange(phrases.Select(_ => string.Format(_, typeName + "." + eventName)));

            return results;
        }

        private void ValidatePhrases(IFieldSymbol symbol, IEnumerable<XmlElementSyntax> comments, Func<IList<string>> phrasesProvider, string xmlElement, ref List<Diagnostic> results)
        {
            var phrases = phrasesProvider();

            // TODO RKN Fix code
            foreach (var comment in comments.Select(_ => _.GetTextWithoutTrivia()))
            {
                if (phrases.Any(_ => comment.StartsWith(_, StringComparison.Ordinal)))
                {
                    return;
                }

                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(Issue(symbol, xmlElement, phrases[0]));
            }
        }
    }
}