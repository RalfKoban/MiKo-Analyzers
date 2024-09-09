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
                                                                        SyntaxKind.DestructorDeclaration,
                                                                        SyntaxKind.MethodDeclaration,
                                                                        SyntaxKind.OperatorDeclaration,
                                                                        SyntaxKind.ConversionOperatorDeclaration,
                                                                        SyntaxKind.LocalDeclarationStatement,
                                                                    };

        private readonly SyntaxKind[] m_syntaxKinds;

        protected MetricsAnalyzer(string diagnosticId, params SyntaxKind[] syntaxKinds) : base(nameof(Metrics), diagnosticId) => m_syntaxKinds = syntaxKinds;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, m_syntaxKinds);

        protected virtual void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeSyntaxNode(context, context.ContainingSymbol);

            ReportDiagnostics(context, issue);
        }

        protected virtual Diagnostic AnalyzeBody(BlockSyntax body, ISymbol containingSymbol) => null;

        protected virtual Diagnostic AnalyzeExpressionBody(ArrowExpressionClauseSyntax body, ISymbol containingSymbol) => null;

        private static BlockSyntax GetBody(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case AccessorDeclarationSyntax a: return a.Body;
                case ConstructorDeclarationSyntax c: return c.Body;
                case DestructorDeclarationSyntax d: return d.Body;
                case MethodDeclarationSyntax m: return m.Body;
                case OperatorDeclarationSyntax o: return o.Body;
                case ConversionOperatorDeclarationSyntax co: return co.Body;
                case LocalFunctionStatementSyntax l: return l.Body;
                default: return null;
            }
        }

        private static ArrowExpressionClauseSyntax GetExpressionBody(SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case AccessorDeclarationSyntax a: return a.ExpressionBody;
                case ConstructorDeclarationSyntax c: return c.ExpressionBody;
                case DestructorDeclarationSyntax d: return d.ExpressionBody;
                case MethodDeclarationSyntax m: return m.ExpressionBody;
                case OperatorDeclarationSyntax o: return o.ExpressionBody;
                case ConversionOperatorDeclarationSyntax co: return co.ExpressionBody;
                case LocalFunctionStatementSyntax l: return l.ExpressionBody;
                default: return null;
            }
        }

        private Diagnostic AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context, ISymbol symbol)
        {
            var body = GetBody(context);

            if (body != null)
            {
                return AnalyzeBody(body, symbol);
            }

            var expressionBody = GetExpressionBody(context);

            if (expressionBody != null)
            {
                return AnalyzeExpressionBody(expressionBody, symbol);
            }

            return null;
        }
    }
}