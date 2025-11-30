using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2012_MeaninglessSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2012";

        public MiKo_2012_MeaninglessSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    return symbol is INamedTypeSymbol type && type.IsNamespace is false && type.IsEnum() is false && type.IsException() is false;

                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                case SymbolKind.Field:
                    return true;
            }

            return false;
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            var symbolNames = GetSelfSymbolNames(symbol);
            var phrases = GetPhrases(symbol);

            return AnalyzeSummaryPhrases(symbol, summaries.Value, phrases.Concat(symbolNames));
        }

        private static string[] GetPhrases(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Field: return Constants.Comments.MeaninglessFieldStartingPhrase;
                case SymbolKind.NamedType: return Constants.Comments.MeaninglessTypeStartingPhrase;
                default: return Constants.Comments.MeaninglessStartingPhrase;
            }
        }

        private static string GetExpectedStartPhrase(IPropertySymbol property)
        {
            var isBoolean = property.Type.IsBoolean();

            if (property.GetMethod is null)
            {
                return isBoolean ? Constants.Comments.BooleanPropertySetterStartingPhrase : Constants.Comments.PropertySetterStartingPhrase;
            }

            if (property.SetMethod is null)
            {
                return isBoolean ? Constants.Comments.BooleanPropertyGetterStartingPhrase : Constants.Comments.PropertyGetterStartingPhrase;
            }

            return isBoolean ? Constants.Comments.BooleanPropertyGetterSetterStartingPhrase : Constants.Comments.PropertyGetterSetterStartingPhrase;
        }

        private static HashSet<string> GetSelfSymbolNames(ISymbol symbol)
        {
            var names = new HashSet<string> { symbol.Name.ConcatenatedWith(' ') };

            switch (symbol)
            {
                case INamedTypeSymbol s:
                {
                    var interfaces = s.AllInterfaces;

                    if (interfaces.Length > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name.ConcatenatedWith(' ')));
                    }

                    break;
                }

                case ISymbol s:
                {
                    names.Add(s.ContainingType.Name);

                    var interfaces = s.ContainingType.AllInterfaces;

                    if (interfaces.Length > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name.ConcatenatedWith(' ')));
                    }

                    break;
                }
            }

            return names;
        }

        private Diagnostic[] AnalyzeSummaryPhrases(ISymbol symbol, in ReadOnlySpan<string> summaries, IEnumerable<string> phrases)
        {
            var summariesLength = summaries.Length;

            if (summariesLength is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var property = symbol as IPropertySymbol;

            for (var index = 0; index < summariesLength; index++)
            {
                var summary = summaries[index];

                if (summary.StartsWith(Constants.Comments.XmlElementStartingTagChar))
                {
                    var i = summary.AsSpan().IndexOf(Constants.Comments.XmlElementEndingTag.AsSpan());
                    var phrase = i > 0 ? summary.AsSpan(0, i + 2) : Constants.Comments.XmlElementStartingTag.AsSpan();

                    return ReportIssueStartingPhrase(symbol, phrase);
                }

                foreach (var phrase in phrases)
                {
                    if (summary.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        return ReportIssueStartingPhrase(symbol, phrase.AsSpan());
                    }
                }

                bool isExpectedPropertyStart = false;
                var expectedPropertyStart = string.Empty;

                if (property != null)
                {
                    expectedPropertyStart = GetExpectedStartPhrase(property);

                    isExpectedPropertyStart = summary.StartsWith(expectedPropertyStart, StringComparison.Ordinal);
                }

                foreach (var phrase in Constants.Comments.MeaninglessPhrase)
                {
                    if (summary.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        if (phrase.EndsWith(" used", StringComparison.Ordinal) && summary.Contains(" used in ", StringComparison.OrdinalIgnoreCase))
                        {
                            // ignore the specific phrase
                            continue;
                        }

                        if (phrase is "able to" && summary.Contains("vailable to", StringComparison.Ordinal))
                        {
                            if (summary.Replace("vailable", "vail").Contains(phrase, StringComparison.OrdinalIgnoreCase) is false)
                            {
                                // ignore phrase 'available'
                                continue;
                            }
                        }

                        if (isExpectedPropertyStart)
                        {
                            if (phrase.Equals("value indicating whether", StringComparison.OrdinalIgnoreCase) && summary.AsSpan(expectedPropertyStart.Length).Contains(phrase, StringComparison.OrdinalIgnoreCase) is false)
                            {
                                // ignore phrase as it is the expected property start
                                continue;
                            }
                        }

                        return ReportIssueContainsPhrase(symbol, phrase.AsSpan());
                    }
                }

                if (property != null && isExpectedPropertyStart is false)
                {
                    return ReportIssueStartingPhrase(property, summary.AsSpan().FirstWord());
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] ReportIssueContainsPhrase(ISymbol symbol, in ReadOnlySpan<char> phrase) => new[] { Issue(symbol, "contain", phrase.HumanizedTakeFirst(200)) };

        private Diagnostic[] ReportIssueStartingPhrase(ISymbol symbol, in ReadOnlySpan<char> phrase) => new[] { Issue(symbol, "start with", phrase.HumanizedTakeFirst(200)) };
    }
}