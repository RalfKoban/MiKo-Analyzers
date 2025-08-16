using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2100_ExampleDefaultPhraseAnalyzer : ExampleDocumentationAnalyzer
    {
        public const string Id = "MiKo_2100";

        private const string Phrase = Constants.Comments.ExampleDefaultPhrase;

        public MiKo_2100_ExampleDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override IReadOnlyList<Diagnostic> AnalyzeExample(ISymbol symbol, IReadOnlyList<XmlElementSyntax> examples)
        {
            List<Diagnostic> results = null;

            foreach (var example in examples)
            {
                var tokens = example.GetXmlTextTokens();

                if (tokens.None(_ => _.ValueText.AsSpan().TrimStart().StartsWith(Phrase)))
                {
                    if (results is null)
                    {
                        results = new List<Diagnostic>(1);
                    }

                    results.Add(Issue(example.StartTag, Phrase));
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}