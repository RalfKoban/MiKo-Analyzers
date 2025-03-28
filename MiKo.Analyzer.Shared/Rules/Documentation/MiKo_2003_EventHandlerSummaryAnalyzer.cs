﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2003_EventHandlerSummaryAnalyzer : SummaryStartDocumentationAnalyzer
    {
        public const string Id = "MiKo_2003";

        private const string StartingPhrase = Constants.Comments.EventHandlerSummaryStartingPhrase;

        public MiKo_2003_EventHandlerSummaryAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.IsEventHandler();

        protected override Diagnostic TextStartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, StartingPhrase);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.Ordinal;

            if (valueText.AsSpan().TrimStart().StartsWith(StartingPhrase, comparison))
            {
                return false;
            }

            problematicText = valueText.FirstWord();

            return true;
        }
    }
}