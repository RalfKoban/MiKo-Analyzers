using System;
using System.Collections.Generic;

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
            foreach (var token in comment.GetXmlTextTokens())
            {
                var text = token.ValueText;

                if (text.Length <= 2 && text.IsNullOrWhiteSpace())
                {
                    // nothing to inspect as the text is too short and consists of whitespaces only
                    continue;
                }

                if (text.ContainsAny(Constants.Comments.ReasoningPhrases))
                {
                    continue;
                }

                foreach (var location in GetAllLocations(token, Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}