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

        internal const string SummaryPhrase = "Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.";
        internal const string ParameterPhrase = "Indicates whether unmanaged resources shall be freed.";

        public MiKo_2014_DisposeDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Name == nameof(IDisposable.Dispose) && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml)
        {
            var summaries = CommentExtensions.GetSummaries(commentXml);

            if (summaries.Any() && summaries.All(_ => _ != SummaryPhrase))
            {
                yield return Issue(symbol, SummaryPhrase);
            }

            // check for parameter
            foreach (var parameter in symbol.Parameters)
            {
                var comment = parameter.GetComment(commentXml);

                switch (comment)
                {
                    case null:
                    case ParameterPhrase:
                        continue;
                }

                yield return Issue(parameter, ParameterPhrase);
            }
        }
    }
}