using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2202";

        public MiKo_2202_DocumentationUsesIdentifierInsteadOfIdAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment) => AnalyzeComment(symbol, comment);

        private IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, DocumentationCommentTriviaSyntax comment)
        {
            var textTokens = comment.GetXmlTextTokens(_ => CodeTags.Contains(_.GetName()) is false);
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                yield break;
            }

            var trimmed = textTokens.GetTextTrimmedWithParaTags();

            if (trimmed.ContainsAny(Constants.Comments.IdTerms, StringComparison.OrdinalIgnoreCase) is false)
            {
                yield break;
            }

            for (var i = 0; i < textTokensCount; i++)
            {
                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(textTokens[i], Constants.Comments.IdTerms, StringComparison.OrdinalIgnoreCase, Offset, Offset);
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