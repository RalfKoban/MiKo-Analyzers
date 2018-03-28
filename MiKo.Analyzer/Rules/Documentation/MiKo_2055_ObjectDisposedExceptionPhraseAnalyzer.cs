using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2055_ObjectDisposedExceptionPhraseAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2055";

        private const string Phrase = Constants.Comments.ObjectDisposedExceptionEndingPhrase;

        public MiKo_2055_ObjectDisposedExceptionPhraseAnalyzer() : base(Id, typeof(ObjectDisposedException))
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment) => exceptionComment.EndsWith(Phrase, StringComparison.Ordinal)
                                                                                                                    ? Enumerable.Empty<Diagnostic>()
                                                                                                                    : new[] { ReportExceptionIssue(symbol, Phrase) };
    }
}
