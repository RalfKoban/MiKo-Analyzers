using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3401_NamespaceDepthAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3401";

        private const int MaxDepth = 7;

        public MiKo_3401_NamespaceDepthAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.NamespaceDeclaration);

        private static int CountNamespaces(NamespaceDeclarationSyntax declaration) => declaration.Name.DescendantTokens().Count(_ => _.IsKind(SyntaxKind.DotToken)) + 1;

        private static int GetNamespaceDepth(NamespaceDeclarationSyntax declaration)
        {
            var parentNamespacesCount = declaration.Ancestors<NamespaceDeclarationSyntax>().Sum(CountNamespaces);

            return parentNamespacesCount + CountNamespaces(declaration);
        }

        private void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
        {
            var node = (NamespaceDeclarationSyntax)context.Node;

            var depth = GetNamespaceDepth(node);

            if (depth > MaxDepth)
            {
                ReportDiagnostics(context, Issue(node.Name, depth, MaxDepth));
            }
        }
    }
}