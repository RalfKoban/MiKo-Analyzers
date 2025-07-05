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

            return AnalyzeSummaryPhrases(symbol, summaries.Value, symbolNames.Concat(phrases));
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

        private static HashSet<string> GetSelfSymbolNames(ISymbol symbol)
        {
            var names = new List<string> { symbol.Name };

            switch (symbol)
            {
                case INamedTypeSymbol s:
                {
                    var interfaces = s.AllInterfaces;

                    if (interfaces.Length > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name));
                    }

                    break;
                }

                case ISymbol s:
                {
                    names.Add(s.ContainingType.Name);

                    var interfaces = s.ContainingType.AllInterfaces;

                    if (interfaces.Length > 0)
                    {
                        names.AddRange(interfaces.Select(_ => _.Name));
                    }

                    break;
                }
            }

            return names.ToHashSet(_ => _ + " ");
        }

        private Diagnostic[] AnalyzeSummaryPhrases(ISymbol symbol, in ReadOnlySpan<string> summaries, IEnumerable<string> phrases)
        {
            var summariesLength = summaries.Length;

            if (summariesLength is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            for (var index = 0; index < summariesLength; index++)
            {
                var summary = summaries[index];

                foreach (var phrase in phrases)
                {
                    if (summary.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        return ReportIssueStartingPhrase(symbol, phrase.AsSpan());
                    }
                }

                if (summary.StartsWith(Constants.Comments.XmlElementStartingTagChar))
                {
                    var i = summary.AsSpan().IndexOf(Constants.Comments.XmlElementEndingTag.AsSpan());
                    var phrase = i > 0 ? summary.AsSpan(0, i + 2) : Constants.Comments.XmlElementStartingTag.AsSpan();

                    return ReportIssueStartingPhrase(symbol, phrase);
                }

                foreach (var phrase in Constants.Comments.MeaninglessPhrase)
                {
                    if (summary.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                    {
                        return ReportIssueContainsPhrase(symbol, phrase.AsSpan());
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private Diagnostic[] ReportIssueContainsPhrase(ISymbol symbol, in ReadOnlySpan<char> phrase) => new[] { Issue(symbol, "contain", phrase.HumanizedTakeFirst(200)) };

        private Diagnostic[] ReportIssueStartingPhrase(ISymbol symbol, in ReadOnlySpan<char> phrase) => new[] { Issue(symbol, "start with", phrase.HumanizedTakeFirst(200)) };
    }
}