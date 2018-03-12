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

        public MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

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
                case Identifier: // correct identifier
                case IdentifierFallback: // correct identifier (fallback as there is already another identifier in the parent lambda expression)
                    return null;

                default:
                    return ReportIssue(identifier.ValueText, identifier.GetLocation(), Identifier);
            }
        }
    }
}