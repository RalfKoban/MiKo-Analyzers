using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2051_ExceptionTagDefaultPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2051";

        public MiKo_2051_ExceptionTagDefaultPhraseAnalyzer() : base(Id, typeof(Exception))
        {
        }
        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var results = CommentExtensions.GetExceptionComments(commentXml)
                          .Where(_ => _.StartsWithAny(Constants.Comments.ExceptionForbiddenStartingPhrase))
                          .Select(_ => Issue(symbol))
                          .ToList();
            return results;
        }
    }
}