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

        private const string SummaryPhrase = Constants.Comments.DisposeSummaryPhrase;
        private const string ParameterPhrase = Constants.Comments.DisposeParameterPhrase;

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
                var parameters = symbol.Parameters;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var parametersLength = parameters.Length;

                for (var index = 0; index < parametersLength; index++)
                {
                    var parameter = parameters[index];
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

            return trimmed.Equals(text);
        }
    }
}