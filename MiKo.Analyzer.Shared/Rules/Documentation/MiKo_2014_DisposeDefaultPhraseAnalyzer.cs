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

        public MiKo_2014_DisposeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(ISymbol symbol) => symbol is IMethodSymbol method && method.Name == nameof(IDisposable.Dispose);

        protected override IReadOnlyList<Diagnostic> AnalyzeSummaries(DocumentationCommentTriviaSyntax comment, ISymbol symbol, IReadOnlyList<XmlElementSyntax> summaryXmls, string commentXml, IReadOnlyCollection<string> summaries)
        {
            var method = (IMethodSymbol)symbol;

            List<Diagnostic> issues = null;

            var summariesCount = summaryXmls.Count;

            for (var index = 0; index < summariesCount; index++)
            {
                if (IsEqual(summaryXmls[index], SummaryPhrase) is false)
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(method, SummaryPhrase));
                }

                // check for parameter
                var parameters = method.Parameters;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var parametersLength = parameters.Length;

                if (parametersLength <= 0)
                {
                    continue;
                }

                for (var i = 0; i < parametersLength; i++)
                {
                    var parameter = parameters[i];
                    var parameterComment = comment.GetParameterComment(parameter.Name);

                    if (parameterComment is null)
                    {
                        continue;
                    }

                    if (IsEqual(parameterComment, ParameterPhrase) is false)
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(1);
                        }

                        issues.Add(Issue(parameter, ParameterPhrase));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }

        private static bool IsEqual(XmlElementSyntax syntax, string text)
        {
            var trimmed = syntax.GetTextTrimmed();

            return trimmed.Equals(text, StringComparison.Ordinal);
        }
    }
}