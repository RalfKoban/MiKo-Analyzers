﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2003_EventHandlerSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2003";

        private const string StartingPhrase = Constants.Comments.EventHandlerSummaryStartingPhrase;

        public MiKo_2003_EventHandlerSummaryAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsEventHandler() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(SyntaxNode node) => Issue(node.GetLocation(), StartingPhrase);

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summaryXml in summaryXmls)
            {
                yield return AnalyzeTextStart(symbol, summaryXml);
            }
        }

        protected override bool AnalyzeTextStart(string valueText, out string problematicText, out StringComparison comparison)
        {
            comparison = StringComparison.Ordinal;

            var text = valueText.AsSpan().TrimStart();

            var startsWith = text.StartsWith(StartingPhrase, comparison);

            problematicText = valueText.FirstWord();

            return startsWith is false;
        }
    }
}