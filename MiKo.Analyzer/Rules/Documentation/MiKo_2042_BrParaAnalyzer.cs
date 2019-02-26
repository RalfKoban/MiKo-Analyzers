using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2042_BrParaAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2042";

        public MiKo_2042_BrParaAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml)
        {
            var comment = GetComment(symbol);

            return comment.Contains("<br", StringComparison.OrdinalIgnoreCase)
                       ? new [] { ReportIssue(symbol) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}