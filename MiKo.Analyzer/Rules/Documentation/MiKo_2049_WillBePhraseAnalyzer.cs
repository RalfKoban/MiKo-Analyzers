using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2049_WillBePhraseAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2049";

        private static readonly string[] Phrases = Constants.Comments.Delimiters.SelectMany(_ => new[] { " will be", " will also be", " will as well be" }, (delimiter, phrase) => phrase + delimiter).ToArray();

        public MiKo_2049_WillBePhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetAllLocations(token, Phrases, StringComparison.OrdinalIgnoreCase, 1))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}