using System.Collections.Generic;

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

        protected abstract IEnumerable<Diagnostic> AnalyzeNamespaceName(string qualifiedName, Location location);

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var qualifiedName = ((NamespaceDeclarationSyntax)context.Node).Name;
            var issues = AnalyzeNamespaceName(qualifiedName.ToString(), qualifiedName.GetLocation());

            ReportDiagnostics(context, issues);
        }
    }
}