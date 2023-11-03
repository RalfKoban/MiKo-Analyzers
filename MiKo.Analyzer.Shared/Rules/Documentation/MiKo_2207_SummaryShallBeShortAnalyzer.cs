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

        private const int MaxAllowedWhitespaces = 50;

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
            foreach (var xml in comment.GetSummaryXmls())
            {
                var summary = xml.GetTextWithoutTrivia();

                if (HasIssue(summary))
                {
                    yield return Issue(xml.StartTag);
                }
            }
        }

        private static bool HasIssue(StringBuilder builder)
        {
            var clearedSummary = builder.ReplaceWithCheck(" - ", " ")
                                        .ReplaceWithCheck(" />", "/>")
                                        .ReplaceWithCheck(" </", "</")
                                        .ReplaceWithCheck("> <", "><")
                                        .ReplaceWithCheck(" cref=", "cref=")
                                        .ReplaceWithCheck(" href=", "href=")
                                        .ReplaceWithCheck(" type=", "type=")
                                        .ReplaceWithCheck(" langword=", "langword=")
                                        .ToString()
                                        .AsSpan()
                                        .Trim();

            return HasIssue(clearedSummary);
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
    }
}