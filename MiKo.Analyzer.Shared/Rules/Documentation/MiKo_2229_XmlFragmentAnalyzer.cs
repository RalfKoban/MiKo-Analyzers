using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2229_XmlFragmentAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2229";

        internal static readonly string[] Phrases = { "</", "/>", "/ >" };

        public MiKo_2229_XmlFragmentAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.DescendantTokens(SyntaxKind.XmlTextLiteralToken))
            {
                foreach (var location in GetAllLocations(token, Phrases))
                {
                    yield return Issue(location);
                }
            }
        }
    }
}