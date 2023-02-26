﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2013_EnumSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2013";

        private const string StartingPhrase = Constants.Comments.EnumStartingPhrase;

        public MiKo_2013_EnumSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsEnum() && base.ShallAnalyze(symbol);

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

        protected override bool AnalyzeTextStart(string valueText, out string problematicText)
        {
            var text = valueText.AsSpan().TrimStart();

            var startsWith = text.StartsWith(StartingPhrase, StringComparison.Ordinal);

            problematicText = text.FirstWord().ToString();

            return startsWith is false;
        }
    }
}