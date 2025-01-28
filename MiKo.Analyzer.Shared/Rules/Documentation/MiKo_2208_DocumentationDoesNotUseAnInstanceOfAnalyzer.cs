using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2208";

        public MiKo_2208_DocumentationDoesNotUseAnInstanceOfAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                const int EndOffset = 1; // we do not want to underline the last char

                var locations = GetAllLocations(token, Constants.Comments.InstanceOfPhrases, StringComparison.Ordinal, 0, EndOffset);
                var locationsCount = locations.Count;

                if (locationsCount > 0)
                {
                    for (var index = 0; index < locationsCount; index++)
                    {
                        yield return Issue(locations[index]);
                    }
                }
            }
        }
    }
}