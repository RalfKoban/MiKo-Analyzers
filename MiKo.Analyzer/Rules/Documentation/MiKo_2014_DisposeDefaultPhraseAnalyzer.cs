using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2014_DisposeDefaultPhraseAnalyzer : SummaryDocumentationAnalyzer
    {
        public const string Id = "MiKo_2014";

        private const string SummaryPhrase = "Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.";
        private const string ParameterPhrase = "Indicates whether unmanaged resources shall be freed.";

        public MiKo_2014_DisposeDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyzeMethod(IMethodSymbol symbol) => symbol.Name == nameof(IDisposable.Dispose);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var summaries = CommentExtensions.GetSummaries(commentXml);
            var results = summaries.Any() && summaries.All(_ => _ != SummaryPhrase)
                          ? new List<Diagnostic> { Issue(symbol, SummaryPhrase) }
                          : null;

            // check for parameter
            foreach (var parameter in symbol.Parameters)
            {
                var comment = parameter.GetComment(commentXml);
                if (comment is null) continue;
                if (comment == ParameterPhrase) continue;

                if (results == null) results = new List<Diagnostic>();
                results.Add(Issue(parameter, ParameterPhrase));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}