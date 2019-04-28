using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ExceptionDocumentationAnalyzer(string diagnosticId, Type exceptionType) : this(diagnosticId, exceptionType.FullName)
        {
        }

        protected ExceptionDocumentationAnalyzer(string diagnosticId, string exceptionTypeFullName) : base(diagnosticId, (SymbolKind)(-1)) => m_exceptionTypeFullName = exceptionTypeFullName;

        protected sealed override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected virtual IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment) => Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var comment = CommentExtensions.GetExceptionComment(m_exceptionTypeFullName, commentXml);
            if (comment is null) return Enumerable.Empty<Diagnostic>();

            return AnalyzeException(symbol, comment);
        }

        protected Diagnostic ReportExceptionIssue(ISymbol owningSymbol, string proposal) => Issue(owningSymbol, ExceptionPhrase, proposal);

        protected string ExceptionPhrase => string.Format(Constants.Comments.ExceptionPhrase, m_exceptionTypeFullName.GetNameOnlyPart());

        private readonly string m_exceptionTypeFullName;
    }
}