using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2227_DocumentationDoesNotContainReSharperMarkerAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2227";

        public MiKo_2227_DocumentationDoesNotContainReSharperMarkerAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var token in comment.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, Constants.Markers.ReSharper))
                {
                    yield return Issue(location);
                }
            }
        }
    }
}