using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    public abstract class MetricsAnalyzer : Analyzer
    {
        protected static readonly SyntaxKind[] DefaultSyntaxKinds =
            {
                SyntaxKind.GetAccessorDeclaration,
                SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.InitAccessorDeclaration,
                SyntaxKind.AddAccessorDeclaration,
                SyntaxKind.RemoveAccessorDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.LocalDeclarationStatement,
                SyntaxKind.MethodDeclaration,
            };

        private readonly SyntaxKind[] m_syntaxKinds;

        protected MetricsAnalyzer(string diagnosticId, params SyntaxKind[] syntaxKinds) : base(nameof(Metrics), diagnosticId) => m_syntaxKinds = syntaxKinds;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, m_syntaxKinds);

        protected virtual void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var body = GetBody(context);

            if (body != null)
            {
                ReportDiagnostics(context, AnalyzeBody(body, context.ContainingSymbol));
            }
        }

        protected virtual Diagnostic AnalyzeBody(BlockSyntax body, ISymbol containingSymbol) => null;

        private static BlockSyntax GetBody(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case AccessorDeclarationSyntax s: return s.Body;
                case ConstructorDeclarationSyntax s: return s.Body;
                case LocalFunctionStatementSyntax l: return l.Body;
                case MethodDeclarationSyntax s: return s.Body;
                default: return null;
            }
        }
    }
}