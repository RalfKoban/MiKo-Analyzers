using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2204_DocumentationShallUseListAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2204";

        private static readonly string[] Delimiters = { ".)", ".", ")", ":" };

        private static readonly string[] Triggers = Enumerable.Empty<string>()
                                                              .Concat(new[] { " -", "--", "---", "*" }.SelectMany(_ => Constants.Comments.Delimiters, (_, delimiter) => string.Concat(delimiter, _, " ")))
                                                              .Concat(new[] { "1", "a", "2", "b", "3", "c" }.SelectMany(_ => Delimiters, (_, delimiter) => string.Concat(" ", _, delimiter, " ")))
                                                              .Concat(new[] { " -- ", " --- ", " * ", " ** ", " *** " })
                                                              .OrderBy(_ => _.Length).ThenBy(_ => _)
                                                              .ToArray();

        public MiKo_2204_DocumentationShallUseListAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                var text = token.ValueText;

                if (text.Length <= 2 && text.IsNullOrWhiteSpace())
                {
                    // nothing to inspect as the text is too short and consists of whitespaces only
                    continue;
                }

                // we do not want to find a ' - ' in the middle of the text (except it contains lots of whitespaces)
                if (token.HasLeadingTrivia && text.AsSpan().TrimStart().StartsWith("- ", StringComparison.OrdinalIgnoreCase))
                {
                    yield return Issue(symbol.Name, token);
                }
                else
                {
                    const int Offset = 1; // we do not want to underline the first and last char

                    foreach (var location in GetAllLocations(token, Triggers, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                    {
                        yield return Issue(symbol.Name, location);
                    }
                }
            }
        }
    }
}