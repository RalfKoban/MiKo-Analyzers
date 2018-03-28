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

        private const StringComparison Comparison = StringComparison.Ordinal;

        public MiKo_2055_ObjectDisposedExceptionPhraseAnalyzer() : base(Id, typeof(ObjectDisposedException))
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment)
        {
            if (exceptionComment.EndsWith(Constants.Comments.ObjectDisposedExceptionEndingPhrase, Comparison)) return Enumerable.Empty<Diagnostic>();

            // alternative check for Closed methods
            if (HasCloseMethod(symbol) && exceptionComment.EndsWith(Constants.Comments.ObjectDisposedExceptionAlternatingEndingPhrase, Comparison))
                return Enumerable.Empty<Diagnostic>();

            return new[] { ReportExceptionIssue(symbol, Constants.Comments.ObjectDisposedExceptionEndingPhrase) };
        }

        private static bool HasCloseMethod(ISymbol symbol) => symbol
                                                              .FindContainingType()
                                                              .IncludingAllBaseTypes()
                                                              .SelectMany(_ => _.GetMembers("Close").OfType<IMethodSymbol>())
                                                              .Any();
    }
}
