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

        protected override IReadOnlyList<Diagnostic> AnalyzeExample(ISymbol symbol, IReadOnlyList<XmlElementSyntax> examples)
        {
            List<Diagnostic> results = null;

            foreach (var example in examples)
            {
                var tokens = example.GetXmlTextTokens();

                foreach (var token in tokens)
                {
                    if (token.ValueText.IndexOf('=') >= 0)
                    {
                        if (results is null)
                        {
                            results = new List<Diagnostic>(1);
                        }

                        // we have an issue
                        results.Add(Issue(example));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}