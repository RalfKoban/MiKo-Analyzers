using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
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

        protected override IEnumerable<Diagnostic> AnalyzeExample(ISymbol owningSymbol, params string[] exampleComments) => AnalyzeStartingPhrase(owningSymbol, exampleComments);

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<string> comments)
        {
            foreach (var comment in comments)
            {
                var index = comment.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                if (index < 0)
                {
                    // nothing found
                    continue;
                }

                // determine if a '<code>' block is before and a '</code>' block is after the example
                var codeTagStartIndex = comment.IndexOf("<code>", StringComparison.OrdinalIgnoreCase);
                var codeTagEndIndex = comment.IndexOf("</code>", StringComparison.OrdinalIgnoreCase);
                if (codeTagStartIndex < 0)
                {
                    // we have an issue
                    yield return Issue(symbol);
                }

                if (codeTagStartIndex < index && index < codeTagEndIndex)
                {
                    // no issue, it's in between
                }
                else
                {
                    yield return Issue(symbol);
                }
            }
        }
    }
}