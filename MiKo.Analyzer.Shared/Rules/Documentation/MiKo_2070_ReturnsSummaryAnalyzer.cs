﻿using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2070_ReturnsSummaryAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2070";

        public MiKo_2070_ReturnsSummaryAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected override bool ConsiderEmptyTextAsIssue(ISymbol symbol) => false;

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            switch (symbol.Name)
            {
                case nameof(ToString):
                case nameof(IEnumerable.GetEnumerator):
                    return false;

                default:
                    return base.ShallAnalyze(symbol);
            }
        }

        protected override Diagnostic StartIssue(ISymbol symbol, SyntaxNode node) => null; // this is no issue as we do not start with any word

        protected override Diagnostic StartIssue(ISymbol symbol, Location location) => Issue(symbol.Name, location, GetProposal(symbol));

        // TODO RKN: Move this to SummaryDocumentAnalyzer when finished
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeSummariesStart(symbol, compilation, commentXml, comment);

        protected override bool AnalyzeTextStart(ISymbol symbol, string valueText, out string problematicText, out StringComparison comparison)
        {
            problematicText = string.Empty;
            comparison = StringComparison.OrdinalIgnoreCase;

            var firstWord = valueText.Without(Constants.Comments.AsynchronouslyStartingPhrase) // skip over async starting phrase
                                     .FirstWord();

            if (firstWord.EqualsAny(Constants.Comments.ReturnWords))
            {
                problematicText = valueText.FirstWord();
                return true;
            }

            return false;
        }

        private static string GetProposal(ISymbol symbol)
        {
            if (symbol is IMethodSymbol m)
            {
                var startText = m.ReturnType.IsBoolean()
                                ? Constants.Comments.DeterminesWhetherPhrase
                                : "Gets";

                if (m.IsAsync)
                {
                    return Constants.Comments.AsynchronouslyStartingPhrase + startText.ToLowerCaseAt(0);
                }

                return startText;
            }

            return "Gets";
        }
    }
}