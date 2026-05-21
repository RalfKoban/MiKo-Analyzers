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

            return AnalyzeSummaryPhrases(symbol, summaries.Value, summaryXmls, phrases.Concat(symbolNames));
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
            var names = new HashSet<string> { symbol.Name.ConcatenatedWith(Constants.Space) };

            switch (symbol)
            {
                case INamedTypeSymbol s:
                {
                    var interfaces = s.AllInterfaces;

                    if (interfaces.Length > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name.ConcatenatedWith(Constants.Space)));
                    }

                    break;
                }

                case ISymbol s:
                {
                    names.Add(s.ContainingType.Name);

                    var interfaces = s.ContainingType.AllInterfaces;

                    if (interfaces.Length > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name.ConcatenatedWith(Constants.Space)));
                    }

                    break;
                }
            }

            return names;
        }

        private Diagnostic[] AnalyzeSummaryPhrases(ISymbol symbol, in ReadOnlySpan<string> summaries, IReadOnlyList<XmlElementSyntax> summaryXmls, IEnumerable<string> phrases)
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
                var summaryXml = summaryXmls.ElementAtOrDefault(index);

                if (summary.StartsWith(Constants.Comments.XmlElementStartingTagChar))
                {
                    var i = summary.AsSpan().IndexOf(Constants.Comments.XmlElementEndingTag.AsSpan());
                    var phrase = i > 0 ? summary.AsSpan(0, i + 2).ToString() : Constants.Comments.XmlElementStartingTag;

                    return ReportIssueStartingPhrase(symbol, summaryXml, phrase);
                }

                foreach (var phrase in phrases)
                {
                    if (summary.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        return ReportIssueStartingPhrase(symbol, summaryXml, phrase);
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
                        if (phrase.EndsWith(" used", StringComparison.Ordinal) && summary.Contains(" used in ", StringComparison.Ordinal))
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

                        return ReportIssueContainsPhrase(symbol, summaryXml, phrase);
                    }
                }

                if (property != null && isExpectedPropertyStart is false)
                {
                    return ReportIssueStartingPhrase(property, summaryXml, summary.FirstWord());
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] ReportIssueContainsPhrase(ISymbol symbol, XmlElementSyntax summaryXml, string phrase) => Issue(symbol, summaryXml, "contain", phrase);

        private Diagnostic[] ReportIssueStartingPhrase(ISymbol symbol, XmlElementSyntax summaryXml, string phrase) => Issue(symbol, summaryXml, "start with", phrase);

        private Diagnostic[] Issue(ISymbol symbol, XmlElementSyntax summaryXml, string condition, string phrase)
        {
            // safety check to see if we have really a summary XML (for whatever reason that could be null as tested with a real-life project)
            if (summaryXml != null)
            {
                var startOffset = phrase.StartsWith(Constants.Space) ? 1 : 0; // we do not want to underline the first space
                var endOffset = phrase.EndsWith(Constants.Space) ? 1 : 0; // we do not want to underline the last space

                // let's find the phrase in the summary XML to report the issue at the correct location
                foreach (var textToken in summaryXml.GetXmlTextTokens())
                {
                    var location = GetFirstLocation(textToken, phrase, startOffset: startOffset, endOffset: endOffset);

                    if (location is null)
                    {
                        // it is not part of the current text token, so we have to search further
                        continue;
                    }

                    return new[] { Issue(symbol.Name, location, condition, phrase.HumanizedTakeFirst(200)) };
                }
            }

            // fall-back to default location if the phrase is not found in the summary XML (could happen if the phrase spans multiple lines)
            return new[] { Issue(symbol, condition, phrase.HumanizedTakeFirst(200)) };
        }
    }
}