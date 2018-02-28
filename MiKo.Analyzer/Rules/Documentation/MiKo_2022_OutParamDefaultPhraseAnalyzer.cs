using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2022_OutParamDefaultPhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2022";

        public MiKo_2022_OutParamDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            List<Diagnostic> results = null;

            foreach (var parameter in symbol.Parameters.Where(_ => _.RefKind == RefKind.Out))
            {
                if (CommentIsAcceptable(parameter, commentXml)) continue;

                if (results == null) results = new List<Diagnostic>();
                results.Add(ReportIssue(parameter, parameter.Name, Constants.Comments.OutParameterStartingPhrase.ConcatenatedWith(", ")));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private static bool CommentIsAcceptable(IParameterSymbol parameter, string commentXml)
        {
            const StringComparison Comparison = StringComparison.Ordinal;

            var comment = GetCommentForParameter(parameter, commentXml);
            if (comment.StartsWithAny(Comparison, Constants.Comments.OutParameterStartingPhrase)) return true;
            if (comment.EqualsAny(Comparison, Constants.Comments.UnusedPhrase)) return true;

            return false;
        }
    }
}