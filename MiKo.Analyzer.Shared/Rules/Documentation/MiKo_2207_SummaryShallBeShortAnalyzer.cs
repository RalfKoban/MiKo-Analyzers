using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2207_SummaryShallBeShortAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2207";

        private const int MaxAllowedWhitespaces = 42;

        public MiKo_2207_SummaryShallBeShortAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                    return true;

                case SymbolKind.Field:
                {
                    if (((IFieldSymbol)symbol).Type.IsEnum())
                    {
                        // remarks sections for enum fields do not work, see MiKo_2211
                        return false;
                    }

                    return true;
                }

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(
                                                                  DocumentationCommentTriviaSyntax comment,
                                                                  ISymbol symbol,
                                                                  IReadOnlyList<XmlElementSyntax> summaryXmls,
                                                                  Lazy<string> commentXml,
                                                                  Lazy<string[]> summaries)
        {
            return Analyze(summaryXmls);
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeContents(
                                                                 DocumentationCommentTriviaSyntax comment,
                                                                 ISymbol symbol,
                                                                 IReadOnlyList<XmlElementSyntax> contentXmls,
                                                                 Lazy<string> commentXml,
                                                                 Lazy<string[]> contents)
        {
            return Analyze(contentXmls);
        }

        private static bool HasIssue(XmlElementSyntax xml)
        {
            var builder = StringBuilderCache.Acquire();

            var summary = xml.GetTextWithoutTrivia(builder)
                             .ReplaceWithProbe(" - ", Constants.SingleSpace)
                             .ReplaceWithProbe(" />", "/>")
                             .ReplaceWithProbe(" </", "</")
                             .ReplaceWithProbe("> <", "><")
                             .ReplaceWithProbe(" cref=", "cref=")
                             .ReplaceWithProbe(" href=", "href=")
                             .ReplaceWithProbe(" type=", "type=")
                             .ReplaceWithProbe(" langref=", "langref=")
                             .ReplaceWithProbe(" langword=", "langword=")
                             .Trim();

            StringBuilderCache.Release(builder);

            return HasIssue(summary.AsSpan());
        }

        private static bool HasIssue(in ReadOnlySpan<char> summary)
        {
            var whitespaces = 0;

            var length = summary.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var c = summary[index];

                    if (c.IsWhiteSpace())
                    {
                        whitespaces++;
                    }

                    if (whitespaces > MaxAllowedWhitespaces)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Diagnostic[] Analyze(IReadOnlyList<XmlElementSyntax> xmls)
        {
            List<Diagnostic> issues = null;

            for (int index = 0, count = xmls.Count; index < count; index++)
            {
                var xml = xmls[index];

                if (HasIssue(xml))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(xml.StartTag));
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}