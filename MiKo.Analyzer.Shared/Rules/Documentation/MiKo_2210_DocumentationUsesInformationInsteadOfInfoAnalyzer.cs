using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2210";

        internal const string Term = "info";

        internal static readonly string[] Terms = GetWithDelimiters(Term);

        public MiKo_2210_DocumentationUsesInformationInsteadOfInfoAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetXmlTextTokens())
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Terms, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}