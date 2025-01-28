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

        private static readonly string[] FindTermsWithDelimiters = Constants.Comments.FindTerms.WithDelimiters();

        public MiKo_2220_DocumentationShouldUseToSeekAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int Offset = 1; // we do not want to underline the first and last char

                var locations = GetAllLocations(token, FindTermsWithDelimiters, StringComparison.OrdinalIgnoreCase, Offset, Offset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    for (var index = 0; index < locationsCount; index++)
                    {
                        yield return Issue(symbol.Name, locations[index], Constants.Comments.ToSeekTerm);
                    }
                }
            }
        }
    }
}