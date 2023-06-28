using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2226_DocumentationContainsIntentionallyAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2226";

        public MiKo_2226_DocumentationContainsIntentionallyAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            return from token in comment.GetXmlTextTokens()
                   from location in GetAllLocations(token, Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase)
                   select Issue(symbol.Name, location);
        }
    }
}