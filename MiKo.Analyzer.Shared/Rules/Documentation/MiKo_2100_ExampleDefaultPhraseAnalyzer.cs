using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        protected override IEnumerable<Diagnostic> AnalyzeExample(ISymbol owningSymbol, IEnumerable<XmlElementSyntax> examples) => AnalyzeStartingPhrase(owningSymbol, examples);

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<XmlElementSyntax> examples)
        {
            const string Phrase = Constants.Comments.ExampleDefaultPhrase;

            foreach (var example in examples)
            {
                var tokens = example.GetXmlTextTokens();

                if (tokens.None(_ => _.ValueText.AsSpan().TrimStart().StartsWith(Phrase, StringComparison.Ordinal)))
                {
                    yield return Issue(symbol.Name, example.StartTag, Phrase);
                }
            }
        }
    }
}