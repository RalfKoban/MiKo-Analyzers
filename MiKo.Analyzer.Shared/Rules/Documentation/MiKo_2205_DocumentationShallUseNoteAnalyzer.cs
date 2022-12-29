using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2205_DocumentationShallUseNoteAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2205";

        private static readonly string[] Triggers = { "Attention", "Caution", "Important:", "Note:" };

        public MiKo_2205_DocumentationShallUseNoteAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, Triggers, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}