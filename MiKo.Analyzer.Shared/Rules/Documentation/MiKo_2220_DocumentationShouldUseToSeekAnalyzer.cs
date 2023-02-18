using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2220_DocumentationShouldUseToSeekAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2220";

        internal const string Replacement = "to seek";

        internal static readonly string[] Terms = { "to find", "to inspect for", "to look for", "to test for" };

        private static readonly string[] TermsWithDelimiters = GetWithDelimiters(Terms);

        public MiKo_2220_DocumentationShouldUseToSeekAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, TermsWithDelimiters, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location, Replacement);
                }
            }
        }
    }
}