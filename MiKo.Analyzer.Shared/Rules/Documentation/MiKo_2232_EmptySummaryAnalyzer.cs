﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2232_EmptySummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2232";

        public MiKo_2232_EmptySummaryAnalyzer() : base(Id)
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
                case SymbolKind.Field:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, string commentXml, IReadOnlyCollection<string> summaries)
        {
            var count = summaryXmls.Count;

            for (var index = 0; index < count; index++)
            {
                var summary = summaryXmls[index];
                var content = summary.Content;

                switch (content.Count)
                {
                    case 0:
                    case 1 when content[0] is XmlTextSyntax text && text.GetTextWithoutTrivia().IsNullOrEmpty():
                        return new[] { Issue(summary) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}