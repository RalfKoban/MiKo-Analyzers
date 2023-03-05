using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2101_ExampleUsesCodeTagAnalyzer : ExampleDocumentationAnalyzer
    {
        public const string Id = "MiKo_2101";

        public MiKo_2101_ExampleUsesCodeTagAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeExample(ISymbol owningSymbol, IEnumerable<XmlElementSyntax> examples) => AnalyzeStartingPhrase(owningSymbol, examples);

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<XmlElementSyntax> examples)
        {
            foreach (var example in examples)
            {
                var tokens = example.GetXmlTextTokens();

                foreach (var token in tokens)
                {
                    var index = token.ValueText.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        // we have an issue
                        yield return Issue(symbol.Name, example);
                    }
                }
            }
        }
    }
}