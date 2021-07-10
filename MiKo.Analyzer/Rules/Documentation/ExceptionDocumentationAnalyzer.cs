using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationAnalyzer : DocumentationAnalyzer
    {
        private readonly string m_exceptionTypeFullName;

        protected ExceptionDocumentationAnalyzer(string diagnosticId, Type exceptionType) : this(diagnosticId, exceptionType.FullName)
        {
        }

        protected ExceptionDocumentationAnalyzer(string diagnosticId, string exceptionTypeFullName) : base(diagnosticId, (SymbolKind)(-1)) => m_exceptionTypeFullName = exceptionTypeFullName;

        protected string ExceptionPhrase => string.Format(Constants.Comments.ExceptionPhrase, m_exceptionTypeFullName.GetNameOnlyPart());

        protected sealed override void InitializeCore(AnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected virtual IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, string exceptionComment) => Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var comment = CommentExtensions.GetExceptionComment(m_exceptionTypeFullName, commentXml);

            return comment is null
                       ? Enumerable.Empty<Diagnostic>()
                       : AnalyzeException(symbol, comment);
        }

        protected Diagnostic ExceptionIssue(ISymbol owningSymbol, string proposal) => Issue(owningSymbol, ExceptionPhrase, proposal);
    }
}