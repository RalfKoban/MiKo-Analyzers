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

                var locations = GetAllLocations(token, Constants.Comments.IntentionallyPhrase, StringComparison.OrdinalIgnoreCase);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    for (var index = 0; index < locationsCount; index++)
                    {
                        yield return Issue(symbol.Name, locations[index]);
                    }
                }
            }
        }
    }
}