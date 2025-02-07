using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2075_ActionFunctionParameterPhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2075";

        private static readonly string[] ActionTermsWithDelimiters = Constants.Comments.ActionTerms.WithDelimiters();

        public MiKo_2075_ActionFunctionParameterPhraseAnalyzer() : base(Id)
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

            if (text.ContainsAny(Constants.Comments.ActionTerms, StringComparison.Ordinal) is false)
            {
                yield break;
            }

            for (var i = 0; i < textTokensCount; i++)
            {
                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(textTokens[i], ActionTermsWithDelimiters, StringComparison.Ordinal, Offset, Offset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    for (var index = 0; index < locationsCount; index++)
                    {
                        yield return Issue(locations[index], Constants.Comments.CallbackTerm);
                    }
                }
            }
        }
    }
}