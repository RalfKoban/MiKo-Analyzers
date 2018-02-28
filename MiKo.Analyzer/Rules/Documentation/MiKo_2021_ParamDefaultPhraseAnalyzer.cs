using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2021_ParamDefaultPhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2021";

        public MiKo_2021_ParamDefaultPhraseAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            List<Diagnostic> results = null;

            foreach (var parameter in symbol.Parameters)
            {
                if (parameter.RefKind == RefKind.Out) continue;
                if (parameter.Type.Name == nameof(System.Boolean)) continue;
                if (CommentIsAcceptable(parameter, commentXml)) continue;

                if (results == null) results = new List<Diagnostic>();
                results.Add(ReportIssue(parameter, parameter.Name, Constants.Comments.ParameterStartingPhrase.ConcatenatedWith(", ")));
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }

        private static bool CommentIsAcceptable(IParameterSymbol parameter, string commentXml)
        {
            var comment = GetCommentForParameter(parameter, commentXml);
            if (comment.StartsWithAny(StringComparison.Ordinal, Constants.Comments.ParameterStartingPhrase)) return true;

            switch (comment)
            {
                case "Unused":
                case "Unused.":
                    return true;

                default:
                    return false;
            }
        }
    }
}