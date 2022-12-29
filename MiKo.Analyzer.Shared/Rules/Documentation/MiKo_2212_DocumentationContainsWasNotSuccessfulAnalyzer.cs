using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2212";

        public MiKo_2212_DocumentationContainsWasNotSuccessfulAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            foreach (var token in symbol.GetXmlTextTokens())
            {
                foreach (var location in GetAllLocations(token, Constants.Comments.WasNotSuccessfulPhrase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}