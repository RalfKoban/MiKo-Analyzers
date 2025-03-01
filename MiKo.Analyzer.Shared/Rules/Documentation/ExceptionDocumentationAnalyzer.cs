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

        protected string ExceptionPhrase => Constants.Comments.ExceptionPhrase.FormatWith(m_exceptionType.Name);

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property);

        protected virtual IEnumerable<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment) => Array.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var comments = GetExceptionComments(comment);

            if (comments is XmlElementSyntax[] array && array.Length == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return AnalyzeComment(symbol, comments);
        }

        protected virtual IEnumerable<XmlElementSyntax> GetExceptionComments(DocumentationCommentTriviaSyntax documentation)
        {
            var exceptionXmls = documentation.GetExceptionXmls();

            if (exceptionXmls.Count == 0)
            {
                return Array.Empty<XmlElementSyntax>();
            }

            return exceptionXmls.Where(_ => _.IsExceptionComment(m_exceptionType));
        }

        protected Diagnostic ExceptionIssue(XmlElementSyntax exceptionComment, string proposal) => Issue(exceptionComment.GetContentsLocation(), ExceptionPhrase, proposal, CreatePhraseProposal(proposal));

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, IEnumerable<XmlElementSyntax> comments)
        {
            foreach (var comment in comments)
            {
                foreach (var issue in AnalyzeException(symbol, comment))
                {
                    yield return issue;
                }
            }
        }
    }
}