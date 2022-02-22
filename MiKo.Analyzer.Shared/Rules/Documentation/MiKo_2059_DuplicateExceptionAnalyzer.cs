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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            var groups = comment.GetExceptionXmls()
                                .SelectMany(_ => _.GetAttributes<XmlCrefAttributeSyntax>())
                                .Select(_ => _.Cref.ToString())
                                .GroupBy(_ => _); // TODO: what about namespaces

            foreach (var g in groups.Where(_ => _.Count() > 1))
            {
                yield return Issue(symbol, g.Key);
            }
        }
    }
}