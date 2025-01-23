﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2001_EventSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2001";

        private const string StartingPhrase = Constants.Comments.EventSummaryStartingPhrase;

        public MiKo_2001_EventSummaryAnalyzer() : base(Id, SymbolKind.Event)
        {
        }

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var summaryXml in summaryXmls)
            {
                var issue = AnalyzeTextStart(symbol, summaryXml);

                if (issue != null)
                {
                    yield return issue;
                }
            }
        }

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.Ordinal;

            var text = valueText.AsSpan().TrimStart();

            var startsWith = text.StartsWith(StartingPhrase, comparison);

            problematicText = text.FirstWord().ToString();

            return startsWith is false;
        }
    }
}