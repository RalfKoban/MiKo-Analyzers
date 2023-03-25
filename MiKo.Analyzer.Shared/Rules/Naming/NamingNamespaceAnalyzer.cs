using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingNamespaceAnalyzer : NamingAnalyzer
    {
        protected NamingNamespaceAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);

        protected abstract IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names);

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (NamespaceDeclarationSyntax)context.Node;

            // collect all names but start on namespace name to not include any attributes or else in the collection
            var names = node.Name.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select(_ => _.Identifier);

            var issues = AnalyzeNamespaceName(names);

            ReportDiagnostics(context, issues);
        }
    }
}