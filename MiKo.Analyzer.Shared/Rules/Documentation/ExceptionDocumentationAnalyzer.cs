using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExceptionDocumentationAnalyzer : DocumentationAnalyzer
    {
        private readonly Type m_exceptionType;

        protected ExceptionDocumentationAnalyzer(string diagnosticId, Type exceptionType) : base(diagnosticId) => m_exceptionType = exceptionType;

        protected string ExceptionPhrase => Constants.Comments.ExceptionPhrase.FormatWith(m_exceptionType.Name);

        protected sealed override bool ShallAnalyze(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Method:
                case SymbolKind.Property:
                    return true;

                default:
                    return false;
            }
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var comments = GetExceptionComments(comment);

            if (comments is XmlElementSyntax[] array && array.Length == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            return AnalyzeComment(symbol, comments);
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeException(ISymbol symbol, XmlElementSyntax exceptionComment) => Array.Empty<Diagnostic>();

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