using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2056_ObjectDisposedExceptionWrongPlacedAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2056";

        public MiKo_2056_ObjectDisposedExceptionWrongPlacedAnalyzer() : base(Id, typeof(ObjectDisposedException))
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment) => symbol.FindContainingType().Implements<IDisposable>()
                                                                                                                    ? Enumerable.Empty<Diagnostic>()
                                                                                                                    : new[] { ReportExceptionIssue(symbol, string.Empty) };
    }
}