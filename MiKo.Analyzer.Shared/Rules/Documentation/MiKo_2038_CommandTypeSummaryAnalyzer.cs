﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2038_CommandTypeSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2038";

        public MiKo_2038_CommandTypeSummaryAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.IsCommand() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            const string Phrase = Constants.Comments.CommandSummaryStartingPhrase;

            if (summaries.None(_ => _.StartsWith(Phrase, StringComparison.Ordinal)))
            {
                yield return Issue(symbol, Constants.XmlTag.Summary, Phrase);
            }
        }
    }
}