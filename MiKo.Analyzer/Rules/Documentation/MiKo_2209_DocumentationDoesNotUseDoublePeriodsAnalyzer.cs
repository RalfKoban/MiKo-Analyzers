using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2209";

        public MiKo_2209_DocumentationDoesNotUseDoublePeriodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, string commentXml) => commentXml.Contains("..", _ => _ != '.', StringComparison.OrdinalIgnoreCase)
                                                                                                        ? new[] { Issue(symbol) }
                                                                                                        : Enumerable.Empty<Diagnostic>();
    }
}