using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2204_DocumentationShallUseListAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2204";

//// ncrunch: rdi off
        private static readonly string[] Delimiters = { ".)", ".", ")", ":" };

        private static readonly string[] Triggers = Array.Empty<string>()
                                                         .Union(new[] { " -", "--", "---", "*" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => string.Concat(delimiter, _, " ")))
                                                         .Union(new[] { "1", "2", "3", "a", "b", "c", "A", "B", "C" }.SelectMany(_ => Delimiters, (_, delimiter) => string.Concat(" ", _, delimiter, " ")))
                                                         .Union(new[] { " -- ", " --- ", " * ", " ** ", " *** " })
                                                         .ToArray(AscendingStringComparer.Default);
//// ncrunch: rdi default

        public MiKo_2204_DocumentationShallUseListAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            var textTokens = comment.GetXmlTextTokens();
            var textTokensCount = textTokens.Count;

            if (textTokensCount == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var commentXml = textTokens.GetTextTrimmedWithParaTags();

            if (commentXml.ContainsAny(Triggers, StringComparison.Ordinal) is false)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];
                var text = token.ValueText;

                if (text.Length <= Constants.MinimumCharactersThreshold && text.IsNullOrWhiteSpace())
                {
                    // nothing to inspect as the text is too short and consists of whitespaces only
                    continue;
                }

                // we do not want to find a ' - ' in the middle of the text (except it contains lots of whitespaces)
                if (token.HasLeadingTrivia && text.AsSpan().TrimStart().StartsWith("- ", StringComparison.Ordinal))
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(Issue(token));
                }
                else
                {
                    const int Offset = 1; // we do not want to underline the first and last char

                    var locations = GetAllLocations(token, Triggers, StringComparison.Ordinal, Offset, Offset);
                    var locationsCount = locations.Count;

                    if (locationsCount > 0)
                    {
                        if (results is null)
                        {
                            results = new List<Diagnostic>(locationsCount);
                        }

                        for (var index = 0; index < locationsCount; index++)
                        {
                            results.Add(Issue(locations[index]));
                        }
                    }
                }
            }

            // only report if we found more than 1 issue
            if (results?.Count > 1)
            {
                return results;
            }

            return Array.Empty<Diagnostic>();
        }
    }
}