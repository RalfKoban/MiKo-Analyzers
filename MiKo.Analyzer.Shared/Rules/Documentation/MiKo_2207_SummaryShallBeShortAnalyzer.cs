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

        public MiKo_2207_SummaryShallBeShortAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.Type.IsEnum())
            {
                // remarks sections for enum fields do not work, see MiKo_2211
                return false;
            }

            return base.ShallAnalyze(symbol);
        }

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            if (summaryXmls.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return AnalyzeSummaries(summaryXmls);
        }

        private static bool HasIssue(XmlElementSyntax xml)
        {
            var builder = StringBuilderCache.Acquire();

            var summary = xml.GetTextWithoutTrivia(builder)
                             .ReplaceWithCheck(" - ", " ")
                             .ReplaceWithCheck(" />", "/>")
                             .ReplaceWithCheck(" </", "</")
                             .ReplaceWithCheck("> <", "><")
                             .ReplaceWithCheck(" cref=", "cref=")
                             .ReplaceWithCheck(" href=", "href=")
                             .ReplaceWithCheck(" type=", "type=")
                             .ReplaceWithCheck(" langref=", "langref=")
                             .ReplaceWithCheck(" langword=", "langword=")
                             .Trim();

            StringBuilderCache.Release(builder);

            return HasIssue(summary.AsSpan());
        }

        private static bool HasIssue(ReadOnlySpan<char> summary)
        {
            var whitespaces = 0;

            foreach (var c in summary)
            {
                if (c.IsWhiteSpace())
                {
                    whitespaces++;
                }

                if (whitespaces > MaxAllowedWhitespaces)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<Diagnostic> AnalyzeSummaries(IReadOnlyList<XmlElementSyntax> summaryXmls)
        {
            var count = summaryXmls.Count;

            for (var index = 0; index < count; index++)
            {
                var xml = summaryXmls[index];

                if (HasIssue(xml))
                {
                    yield return Issue(xml.StartTag);
                }
            }
        }
    }
}