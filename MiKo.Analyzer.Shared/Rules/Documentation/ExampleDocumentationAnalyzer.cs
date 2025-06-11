using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExampleDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ExampleDocumentationAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected sealed override IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol, SemanticModel semanticModel)
        {
            var examples = comment.GetExampleXmls();

            return examples.Count is 0
                   ? Array.Empty<Diagnostic>()
                   : AnalyzeExample(symbol, examples);
        }

        protected virtual IReadOnlyList<Diagnostic> AnalyzeExample(ISymbol symbol, IReadOnlyList<XmlElementSyntax> examples) => Array.Empty<Diagnostic>();
    }
}