using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var summaries = comment.GetSummaryXmls();

            foreach (var summary in summaries)
            {
                if (IsEqual(summary, SummaryPhrase) is false)
                {
                    yield return Issue(symbol, SummaryPhrase);
                }

                // check for parameter
                foreach (var parameter in symbol.Parameters)
                {
                    var parameterComment = comment.GetParameterComment(parameter.Name);

                    if (parameterComment != null)
                    {
                        if (IsEqual(parameterComment, ParameterPhrase) is false)
                        {
                            yield return Issue(parameter, ParameterPhrase);
                        }
                    }
                }
            }
        }

        private static bool IsEqual(XmlElementSyntax syntax, string text)
        {
            var trimmed = syntax.GetTextTrimmed();

            return trimmed.SequenceEqual(text.AsSpan());
        }
    }
}