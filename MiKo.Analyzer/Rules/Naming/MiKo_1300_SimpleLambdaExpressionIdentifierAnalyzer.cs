using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1300";

        private const string Identifier = "_";
        private const string IdentifierFallback = "__";
        private const string IdentifierFallback2 = "___";

        public MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
        }

        private void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (SimpleLambdaExpressionSyntax)context.Node;
            var diagnostic = AnalyzeSimpleLambdaExpression(node);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            if (node.Parameter is null) return null;

            var identifier = node.Parameter.Identifier;
            switch (identifier.ValueText)
            {
                case null: // we don't have one
                case Identifier: // correct identifier (default one)
                case IdentifierFallback: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                case IdentifierFallback2: // correct identifier (2nd fallback as there is already another identifier in the parent lambda expression)
                    return null;

                default:
                    return Issue(identifier.ValueText, identifier.GetLocation(), Identifier);
            }
        }
    }
}