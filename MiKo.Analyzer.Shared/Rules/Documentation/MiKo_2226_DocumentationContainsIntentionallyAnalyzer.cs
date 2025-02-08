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

        private static readonly string[] Phrases = Constants.Comments.IntentionallyPhrase;

        private static readonly int MinimumPhraseLength = Phrases.Min(_ => _.Length);

        public MiKo_2226_DocumentationContainsIntentionallyAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                yield break;
            }

            var trimmed = textTokens.GetTextTrimmedWithParaTags();

            if (trimmed.ContainsAny(Phrases, StringComparison.OrdinalIgnoreCase) is false)
            {
                yield break;
            }

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];
                var text = token.ValueText;

                if (text.Length < MinimumPhraseLength)
                {
                    continue;
                }

                if (text.ContainsAny(Constants.Comments.ReasoningPhrases))
                {
                    continue;
                }

                var locations = GetAllLocations(token, Phrases, StringComparison.OrdinalIgnoreCase);
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