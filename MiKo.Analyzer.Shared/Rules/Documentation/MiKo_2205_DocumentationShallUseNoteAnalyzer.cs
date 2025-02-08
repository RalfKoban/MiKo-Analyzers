using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                yield break;
            }

            var text = textTokens.GetTextTrimmedWithParaTags();

            if (text.ContainsAny(Triggers, StringComparison.OrdinalIgnoreCase) is false)
            {
                yield break;
            }

            for (var i = 0; i < textTokensCount; i++)
            {
                var locations = GetAllLocations(textTokens[i], Triggers, StringComparison.OrdinalIgnoreCase);
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