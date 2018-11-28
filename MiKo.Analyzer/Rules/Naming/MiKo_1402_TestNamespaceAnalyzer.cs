using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1402_TestNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1402";

        public MiKo_1402_TestNamespaceAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var qualifiedName = ((NamespaceDeclarationSyntax)context.Node).Name;

            var fullName = qualifiedName.ToString();
            if (fullName.Contains("Test", StringComparison.OrdinalIgnoreCase))
            {
                var diagnostic = ReportIssue(fullName, qualifiedName.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}