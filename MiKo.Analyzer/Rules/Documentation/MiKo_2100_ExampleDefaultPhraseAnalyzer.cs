using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2100_ExampleDefaultPhraseAnalyzer : ExampleDocumentationAnalyzer
    {
        public const string Id = "MiKo_2100";

        public MiKo_2100_ExampleDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeExample(ISymbol owningSymbol, params string[] exampleComments) => AnalyzeStartingPhrase(owningSymbol, exampleComments, Constants.Comments.ExampleDefaultPhrase);

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<string> comments, params string[] phrases)
        {
            var found = comments.All(_ => phrases.Any(__ => _.StartsWith(__, StringComparison.Ordinal)));
            return found
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(symbol, phrases[0]) };
        }
    }
}