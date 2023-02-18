using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2016_AsyncMethodDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2016";

        private const string Phrase = Constants.Comments.AsynchrounouslyStartingPhrase;

        public MiKo_2016_AsyncMethodDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsAsyncTaskBased() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeSummary(ISymbol symbol, Compilation compilation, IEnumerable<string> summaries, DocumentationCommentTriviaSyntax comment)
        {
            if (summaries.None(_ => _.StartsWith(Phrase, StringComparison.Ordinal)))
            {
                return new [] { Issue(symbol, Phrase) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}