using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override Diagnostic AnalyzeSummary(ISymbol symbol, SyntaxNode summaryXml)
        {
            var summary = string.Concat(summaryXml.DescendantNodes<XmlTextSyntax>().Select(_ => _.GetTextWithoutTrivia()));

            return HasIssue(summary) ? Issue(symbol) : null;
        }

        private static bool HasIssue(string summary)
        {
            var whitespaces = 0;

            var clearedSummary = summary.Replace(" - ", " ")
                                        .Replace(" />", "/>")
                                        .Replace(" </", "</")
                                        .Replace("> <", "><")
                                        .Replace(" cref=", "cref=")
                                        .Replace(" href=", "href=")
                                        .Replace(" type=", "type=")
                                        .Replace(" langword=", "langword=")
                                        .Trim();
            foreach (var c in clearedSummary)
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