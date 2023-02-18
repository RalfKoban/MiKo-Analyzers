﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2047";

        private static readonly string StartingPhrases = Constants.Comments.AttributeSummaryStartingPhrase.OrderBy(_ => _).HumanizedConcatenated();

        public MiKo_2047_AttributeSummaryDefaultPhraseAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.InheritsFrom<Attribute>() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            return summaries.Any(_ => _.StartsWithAny(Constants.Comments.AttributeSummaryStartingPhrase, StringComparison.Ordinal))
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, StartingPhrases) };
        }
    }
}