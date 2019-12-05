﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2070";

        private static readonly string[] Phrases = { "Return", "Returns" };

        public MiKo_2070_ReturnsSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, IEnumerable<string> summaries) => summaries.Any(StartsWithPhrase)
                                                                                                                        ? new[] { Issue(symbol, GetProposal(symbol)) }
                                                                                                                        : Enumerable.Empty<Diagnostic>();

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol)
        {
            switch (symbol.Name)
            {
                case nameof(ToString):
                case nameof(IEnumerable.GetEnumerator):
                    return false;

                default:
                    return true;
            }
        }

        private static bool StartsWithPhrase(string summary)
        {
            var firstWord = summary.Remove(Constants.Comments.AsynchrounouslyStartingPhrase).Trim() // skip over async starting phrase
                                   .FirstWord();

            return firstWord.EqualsAny(Phrases);
        }

        private static string GetProposal(ISymbol symbol)
        {
            switch (symbol)
            {
                case IPropertySymbol p when p.GetReturnType()?.IsBoolean() is true:
                case IMethodSymbol m when m.ReturnType.IsBoolean():
                {
                    return "Determines";
                }

                default:
                {
                    return "Gets";
                }
            }
        }
    }
}