using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class ExampleDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected ExampleDocumentationAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, string commentXml) => AnalyzeComment(symbol, compilation, symbol.GetDocumentationCommentXml());

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            if (commentXml.IsNullOrWhiteSpace())
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var comments = CommentExtensions.GetExamples(commentXml).ToArray();

            return AnalyzeExample(symbol, comments);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeExample(ISymbol owningSymbol, params string[] exampleComments) => Enumerable.Empty<Diagnostic>();
    }
}