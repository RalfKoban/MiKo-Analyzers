using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationAnalyzer : DocumentationAnalyzer
    {
        private readonly Type m_exceptionType;

        protected ExceptionDocumentationAnalyzer(string diagnosticId, Type exceptionType) : base(diagnosticId, (SymbolKind)(-1)) => m_exceptionType = exceptionType;

        protected string ExceptionPhrase => string.Format(Constants.Comments.ExceptionPhrase, m_exceptionType.Name);

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected virtual IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment) => Enumerable.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                yield break;
            }

            var documentation = symbol.GetDocumentationCommentTriviaSyntax();
            if (documentation is null)
            {
                // no documentation
                yield break;
            }

            // var comment = CommentExtensions.GetExceptionComment(m_exceptionType.FullName, commentXml);
            foreach (var commentElement in GetExceptionComments(documentation))
            {
                foreach (var issue in AnalyzeException(symbol, commentElement))
                {
                    yield return issue;
                }
            }
        }

        protected virtual IEnumerable<XmlElementSyntax> GetExceptionComments(DocumentationCommentTriviaSyntax documentation)
        {
            return documentation.GetExceptionXmls().Where(_ => _.IsExceptionComment(m_exceptionType));
        }

        protected Diagnostic ExceptionIssue(XmlElementSyntax exceptionComment, string proposal) => Issue(string.Empty, exceptionComment, ExceptionPhrase, proposal);
    }
}