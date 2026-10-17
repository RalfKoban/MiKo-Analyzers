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
            return AnalyzePhrases(symbol, summaries.Value, summaryXmls);
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeContents(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> contentXmls, Lazy<string> commentXml, Lazy<string[]> contents)
        {
            return AnalyzePhrases(symbol, contents.Value, contentXmls);
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
                    var interfacesLength = interfaces.Length;

                    if (interfacesLength > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name.ConcatenatedWith(Constants.Space)));
                    }

                    break;
                }

                case ISymbol s:
                {
                    names.Add(s.ContainingType.Name);

                    var interfaces = s.ContainingType.AllInterfaces;
                    var interfacesLength = interfaces.Length;

                    if (interfacesLength > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name.ConcatenatedWith(Constants.Space)));
                    }

                    break;
                }
            }

            return names;
        }

        private Diagnostic[] AnalyzePhrases(ISymbol symbol, in ReadOnlySpan<string> texts, IReadOnlyList<XmlElementSyntax> xmls)
        {
            if (texts.Length is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var phrases = GetPhrases(symbol);

            var symbolNames = GetSelfSymbolNames(symbol);

            return AnalyzePhrases(symbol, texts, xmls, phrases.Concat(symbolNames));
        }

        private Diagnostic[] AnalyzePhrases(ISymbol symbol, in ReadOnlySpan<string> texts, IReadOnlyList<XmlElementSyntax> xmls, IEnumerable<string> phrases)
        {
            var property = symbol as IPropertySymbol;

            for (int index = 0, textsLength = texts.Length; index < textsLength; index++)
            {
                var text = texts[index];
                var xml = xmls.ElementAtOrDefault(index);

                if (text.StartsWith(Constants.Comments.XmlElementStartingTagChar))
                {
                    var i = text.AsSpan().IndexOf(Constants.Comments.XmlElementEndingTag.AsSpan());
                    var phrase = i > 0 ? text.AsSpan(0, i + 2).ToString() : Constants.Comments.XmlElementStartingTag;

                    return ReportIssueStartingPhrase(symbol, xml, phrase);
                }

                foreach (var phrase in phrases)
                {
                    if (text.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        return ReportIssueStartingPhrase(symbol, xml, phrase);
                    }
                }

                bool isExpectedPropertyStart = false;
                var expectedPropertyStart = string.Empty;

                if (property != null)
                {
                    expectedPropertyStart = GetExpectedStartPhrase(property);

                    isExpectedPropertyStart = text.StartsWith(expectedPropertyStart, StringComparison.Ordinal);
                }

                foreach (var phrase in Constants.Comments.MeaninglessPhrase)
                {
                    if (text.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        if (phrase.EndsWith(" used", StringComparison.Ordinal) && text.Contains(" used in ", StringComparison.Ordinal))
                        {
                            // ignore the specific phrase
                            continue;
                        }

                        if (phrase is "able to" && text.Contains("vailable to", StringComparison.Ordinal))
                        {
                            if (text.Replace("vailable", "vail").Contains(phrase, StringComparison.OrdinalIgnoreCase) is false)
                            {
                                // ignore phrase 'available'
                                continue;
                            }
                        }

                        if (isExpectedPropertyStart)
                        {
                            if (phrase.Equals("value indicating whether", StringComparison.OrdinalIgnoreCase) && text.AsSpan(expectedPropertyStart.Length).Contains(phrase, StringComparison.OrdinalIgnoreCase) is false)
                            {
                                // ignore phrase as it is the expected property start
                                continue;
                            }
                        }

                        return ReportIssueContainsPhrase(symbol, xml, phrase);
                    }
                }

                if (property != null && isExpectedPropertyStart is false)
                {
                    return ReportIssueStartingPhrase(property, xml, text.FirstWord());
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] ReportIssueContainsPhrase(ISymbol symbol, XmlElementSyntax xml, string phrase) => Issue(symbol, xml, "contain", phrase);

        private Diagnostic[] ReportIssueStartingPhrase(ISymbol symbol, XmlElementSyntax xml, string phrase) => Issue(symbol, xml, "start with", phrase);

        private Diagnostic[] Issue(ISymbol symbol, XmlElementSyntax xml, string condition, string phrase)
        {
            // safety check to see if we have really an XML (for whatever reason that could be null as tested with a real-life project)
            if (xml is null)
            {
                return new[] { Issue(symbol, condition, phrase.HumanizedTakeFirst(200)) };
            }

            // let's find the phrase in the XML to report the issue at the correct location
            if (phrase.StartsWith(Constants.Comments.XmlElementStartingTagChar))
            {
                // we have an XML link here, such as /// <see cref="..."/>
                foreach (var content in xml.Content)
                {
                    switch (content)
                    {
                        case XmlEmptyElementSyntax _:
                            return new[] { Issue(symbol.Name, content, condition, phrase.HumanizedTakeFirst(200)) };

                        case XmlElementSyntax e when e.IsPara():
                        {
                            // we have a <para> tag around, so we have to get rid of it
                            foreach (var subContent in e.Content)
                            {
                                if (subContent is XmlEmptyElementSyntax)
                                {
                                    return new[] { Issue(symbol.Name, subContent, condition, phrase.HumanizedTakeFirst(200)) };
                                }
                            }

                            break;
                        }
                    }
                }
            }

            var startOffset = phrase.StartsWith(Constants.Space) ? 1 : 0; // we do not want to underline the first space
            var endOffset = phrase.EndsWith(Constants.Space) ? 1 : 0; // we do not want to underline the last space

            var xmlTextTokens = xml.GetXmlTextTokens();

            foreach (var textToken in xmlTextTokens)
            {
                var location = textToken.GetFirstLocation(phrase, comparison: StringComparison.OrdinalIgnoreCase, startOffset: startOffset, endOffset: endOffset);

                if (location is null)
                {
                    // it is not part of the current text token, so we have to search further
                    continue;
                }

                return new[] { Issue(symbol.Name, location, condition, phrase.HumanizedTakeFirst(200)) };
            }

            // fall-back to default location if the phrase is not found in the XML (could happen if the phrase spans multiple lines)
            return new[] { Issue(symbol.Name, xml.GetContentsLocation(), condition, phrase.HumanizedTakeFirst(200)) };
        }
    }
}