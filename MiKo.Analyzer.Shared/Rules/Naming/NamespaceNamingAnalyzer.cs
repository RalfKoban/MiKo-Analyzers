using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamespaceNamingAnalyzer : NamingAnalyzer
    {
        protected NamespaceNamingAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);

        protected abstract IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames);

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (NamespaceDeclarationSyntax)context.Node;

            // collect all names but start on namespace name to not include any attributes or else in the collection
            var names = node.Name.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select(_ => _.Identifier).ToArray();

            var issues = AnalyzeNamespaceName(names);

            if (issues.Count > 0)
            {
                ReportDiagnostics(context, issues);
            }
        }
    }
}