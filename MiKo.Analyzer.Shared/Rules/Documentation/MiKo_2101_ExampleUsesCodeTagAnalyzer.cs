using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<Diagnostic> AnalyzeExamples(ISymbol owningSymbol, IEnumerable<XmlElementSyntax> examples) => AnalyzeStartingPhrase(owningSymbol, examples);

        private IEnumerable<Diagnostic> AnalyzeStartingPhrase(ISymbol symbol, IEnumerable<XmlElementSyntax> examples)
        {
            foreach (var example in examples)
            {
                foreach (var token in example.DescendantNodes<XmlTextSyntax>().SelectMany(_ => _.TextTokens))
                {
                    var comment = token.ValueText;
                    var index = comment.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                    if (index < 0)
                    {
                        // nothing found
                        continue;
                    }

                    // determine if the example is inside a '<code>' block
                    var codeTag = token.Parent.FirstAncestor<XmlElementSyntax>(_ => _.GetName() == Constants.XmlTag.Code);
                    if (codeTag is null)
                    {
                        // we have an issue
                        yield return Issue(token);
                    }
                }
            }
        }
    }
}