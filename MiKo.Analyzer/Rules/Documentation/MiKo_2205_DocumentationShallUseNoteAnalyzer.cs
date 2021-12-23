using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2205_DocumentationShallUseNoteAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2205";

        private static readonly string[] Triggers = { "Attention", "Caution!", "Caution:", "Caution !", "  Important: ", "  Note: ", "  Please note: " };

        public MiKo_2205_DocumentationShallUseNoteAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml) => commentXml.ContainsAny(Triggers, StringComparison.OrdinalIgnoreCase)
                                                                                                                                     ? new[] { Issue(symbol) }
                                                                                                                                     : Enumerable.Empty<Diagnostic>();
    }
}