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

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        protected virtual IReadOnlyList<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment) => Array.Empty<Diagnostic>();

        protected virtual IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
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

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                var symbol = context.ContainingSymbol;

                switch (symbol?.Kind)
                {
                    case SymbolKind.Method:
                    case SymbolKind.Property:
                    {
                        var issues = AnalyzeComment(comment, symbol);

                        if (issues.Count > 0)
                        {
                            ReportDiagnostics(context, issues);
                        }

                        break;
                    }
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(ISymbol symbol, IEnumerable<XmlElementSyntax> comments)
        {
            List<Diagnostic> results = null;

            foreach (var comment in comments)
            {
                var issues = AnalyzeException(symbol, comment);
                var count = issues.Count;

                if (count > 0)
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(count);
                    }

                    results.AddRange(issues);
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}