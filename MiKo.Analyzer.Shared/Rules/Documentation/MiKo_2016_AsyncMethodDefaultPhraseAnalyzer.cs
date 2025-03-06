﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2016_AsyncMethodDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2016";

        private const string Phrase = Constants.Comments.AsynchronouslyStartingPhrase;

        public MiKo_2016_AsyncMethodDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsAsyncTaskBased() && base.ShallAnalyze(symbol);

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, Phrase);

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeSummariesStart(symbol, compilation, commentXml, comment);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWith(Phrase, comparison))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}