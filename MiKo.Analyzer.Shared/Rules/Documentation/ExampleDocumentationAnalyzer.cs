using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExampleDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ExampleDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        protected virtual IReadOnlyList<Diagnostic> AnalyzeExample(ISymbol symbol, IReadOnlyList<XmlElementSyntax> examples) => Array.Empty<Diagnostic>();

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                var symbol = context.ContainingSymbol;

                var issues = AnalyzeComment(comment, symbol);

                if (issues.Count > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
        {
            var examples = comment.GetExampleXmls();

            return examples.Count == 0
                   ? Array.Empty<Diagnostic>()
                   : AnalyzeExample(symbol, examples);
        }
    }
}