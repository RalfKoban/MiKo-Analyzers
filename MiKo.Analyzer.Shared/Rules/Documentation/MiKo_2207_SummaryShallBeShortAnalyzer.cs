using System;
using System.Collections.Generic;
using System.Text;

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
            var count = summaryXmls.Count;

            List<Diagnostic> issues = null;

            for (var index = 0; index < count; index++)
            {
                var xml = summaryXmls[index];

                if (HasIssue(xml))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(xml.StartTag));
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(XmlElementSyntax xml)
        {
            var builder = StringBuilderCache.Acquire();

            var summary = xml.GetTextWithoutTrivia(builder)
                             .ReplaceWithProbe(" - ", " ")
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
    }
}