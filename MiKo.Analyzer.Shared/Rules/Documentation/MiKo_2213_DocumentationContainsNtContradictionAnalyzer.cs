using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2213_DocumentationContainsNtContradictionAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2213";

        public MiKo_2213_DocumentationContainsNtContradictionAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, Constants.Comments.NotContradictionPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}