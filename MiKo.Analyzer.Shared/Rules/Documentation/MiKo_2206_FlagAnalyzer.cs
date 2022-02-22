using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2206_FlagAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2206";

        private static readonly string[] Phrases = CreatePhrases(" flag", " flags").ToArray();

        public MiKo_2206_FlagAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                const int Offset = 1; // we do not want to underline the first and last char
                foreach (var location in GetAllLocations(token, Phrases, StringComparison.OrdinalIgnoreCase, Offset, Offset))
                {
                    yield return Issue(symbol.Name, location, location.GetText());
                }
            }
        }

        private static IEnumerable<string> CreatePhrases(params string[] forbiddenTerms) => from suffix in Constants.Comments.Delimiters from forbiddenTerm in forbiddenTerms select forbiddenTerm + suffix;
    }
}