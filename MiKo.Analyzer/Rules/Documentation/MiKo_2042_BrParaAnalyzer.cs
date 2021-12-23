using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2042_BrParaAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2042";

        private static readonly string[] ParagraphTags =
            {
                "<p>",
                "</p>",
            };

        public MiKo_2042_BrParaAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var comment = symbol.GetComment();

            if (comment.Contains("<br", StringComparison.OrdinalIgnoreCase))
            {
                yield return Issue(symbol, "<br/>");
            }

            if (comment.ContainsAny(ParagraphTags, StringComparison.OrdinalIgnoreCase))
            {
                yield return Issue(symbol, "<p>...</p>");
            }
        }
    }
}