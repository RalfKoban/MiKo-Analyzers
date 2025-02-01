using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            const string Phrase = Constants.Comments.WasNotSuccessfulPhrase;

            foreach (var token in comment.GetXmlTextTokens())
            {
                if (token.ValueText.Length < Phrase.Length)
                {
                    continue;
                }

                foreach (var location in GetAllLocations(token, Phrase))
                {
                    yield return Issue(symbol.Name, location);
                }
            }
        }
    }
}