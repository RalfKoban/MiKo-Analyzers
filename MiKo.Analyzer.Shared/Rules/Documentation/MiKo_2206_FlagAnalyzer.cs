using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2206_FlagAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2206";

        public MiKo_2206_FlagAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int Offset = 1; // we do not want to underline the first and last char

                foreach (var location in GetAllLocations(token, Constants.Comments.FlagTermsWithDelimiters, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    yield return Issue(location);
                }
            }
        }
    }
}