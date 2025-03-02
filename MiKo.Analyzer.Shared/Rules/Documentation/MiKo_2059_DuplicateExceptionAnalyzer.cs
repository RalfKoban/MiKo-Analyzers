using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2059_DuplicateExceptionAnalyzer : ExceptionDocumentationAnalyzer
    {
        public const string Id = "MiKo_2059";

        public MiKo_2059_DuplicateExceptionAnalyzer() : base(Id, typeof(Exception))
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            List<Diagnostic> results = null;

            var groups = symbol.GetDocumentationCommentXml()
                               .GetExceptionsOfExceptionComments()
                               .GroupBy(_ => _) // TODO: what about namespaces
                               .Where(_ => _.MoreThan(1));

            foreach (var g in groups)
            {
                if (results is null)
                {
                    results = new List<Diagnostic>(1);
                }

                results.Add(Issue(symbol, g.Key));
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}