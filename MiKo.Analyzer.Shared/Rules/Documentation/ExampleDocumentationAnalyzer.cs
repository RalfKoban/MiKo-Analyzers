using System.Collections.Generic;
using System.Linq;

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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment, string commentXml)
        {
            return AnalyzeComment(symbol, compilation, symbol.GetDocumentationCommentXml(), comment);
        }

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            var examples = comment.GetExampleXmls();

            return AnalyzeExample(symbol, examples);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeExample(ISymbol owningSymbol, IEnumerable<XmlElementSyntax> examples) => Enumerable.Empty<Diagnostic>();
    }
}