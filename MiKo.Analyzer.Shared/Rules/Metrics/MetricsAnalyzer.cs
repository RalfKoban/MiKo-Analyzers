using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Metrics
{
    /// <summary>
    /// Provides the base class for analyzers that enforce metrics rules.
    /// </summary>
    public abstract class MetricsAnalyzer : Analyzer
    {
        /// <summary>
        /// Contains the default syntax kinds that are analyzed for metrics violations.
        /// This field is read-only.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsAnalyzer"/> class with the unique identifier of the diagnostic and the syntax kinds to analyze.
        /// </summary>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
        /// <param name="syntaxKinds">
        /// The syntax kinds that shall be analyzed.
        /// </param>
        protected MetricsAnalyzer(string diagnosticId, params SyntaxKind[] syntaxKinds) : base(nameof(Metrics), diagnosticId) => m_syntaxKinds = syntaxKinds;

        /// <inheritdoc/>
        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, m_syntaxKinds);

        /// <summary>
        /// Analyzes a syntax node for metrics violations and reports any diagnostics found.
        /// </summary>
        /// <param name="context">
        /// The syntax node analysis context.
        /// </param>
        protected virtual void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeSyntaxNode(context, context.ContainingSymbol);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        /// <summary>
        /// Analyzes a block body for metrics violations and returns any diagnostic found.
        /// </summary>
        /// <param name="body">
        /// The block syntax representing the body to analyze.
        /// </param>
        /// <param name="containingSymbol">
        /// The symbol that contains the body.
        /// </param>
        /// <returns>
        /// A diagnostic if a metrics violation is found; otherwise, <see langword="null"/>.
        /// </returns>
        protected virtual Diagnostic AnalyzeBody(BlockSyntax body, ISymbol containingSymbol) => null;

        /// <summary>
        /// Analyzes an expression body for metrics violations and returns any diagnostic found.
        /// </summary>
        /// <param name="body">
        /// The arrow expression clause syntax representing the body to analyze.
        /// </param>
        /// <param name="containingSymbol">
        /// The symbol that contains the body.
        /// </param>
        /// <returns>
        /// A diagnostic if a metrics violation is found; otherwise, <see langword="null"/>.
        /// </returns>
        protected virtual Diagnostic AnalyzeExpressionBody(ArrowExpressionClauseSyntax body, ISymbol containingSymbol) => null;

        private static BlockSyntax GetBody(in SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case MethodDeclarationSyntax m: return m.Body;
                case AccessorDeclarationSyntax a: return a.Body;
                case ConstructorDeclarationSyntax c: return c.Body;
                case OperatorDeclarationSyntax o: return o.Body;
                case ConversionOperatorDeclarationSyntax co: return co.Body;
                case DestructorDeclarationSyntax d: return d.Body;
                case LocalFunctionStatementSyntax l: return l.Body;
                default: return null;
            }
        }

        private static ArrowExpressionClauseSyntax GetExpressionBody(in SyntaxNodeAnalysisContext context)
        {
            switch (context.Node)
            {
                case AccessorDeclarationSyntax a: return a.ExpressionBody;
                case MethodDeclarationSyntax m: return m.ExpressionBody;
                case OperatorDeclarationSyntax o: return o.ExpressionBody;
                case ConversionOperatorDeclarationSyntax co: return co.ExpressionBody;
                case ConstructorDeclarationSyntax c: return c.ExpressionBody;
                case DestructorDeclarationSyntax d: return d.ExpressionBody;
                case LocalFunctionStatementSyntax l: return l.ExpressionBody;
                default: return null;
            }
        }

        private Diagnostic AnalyzeSyntaxNode(in SyntaxNodeAnalysisContext context, ISymbol symbol)
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