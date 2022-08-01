using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2220_DocumentationShouldUseToSeekAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2220";

        internal const string Replacement = "to seek";

        internal static readonly string[] Terms = { "to inspect for", "to look for", "to test for" };

        public MiKo_2220_DocumentationShouldUseToSeekAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetDocumentationCommentTriviaSyntax().DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
            {
                foreach (var location in GetAllLocations(token, Terms))
                {
                    yield return Issue(symbol.Name, location, Replacement);
                }
            }
        }
    }
}