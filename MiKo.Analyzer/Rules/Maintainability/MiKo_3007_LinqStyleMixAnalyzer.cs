using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3007_LinqStyleMixAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3007";

        public MiKo_3007_LinqStyleMixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQueryExpression, SyntaxKind.QueryExpression);

        private static bool TryFindSyntaxNode(QueryExpressionSyntax query, out SyntaxNode result, out string identifier)
        {
            var methodDeclarationSyntax = query.GetEnclosing<MethodDeclarationSyntax>();
            if (methodDeclarationSyntax != null)
            {
                result = methodDeclarationSyntax;
                identifier = methodDeclarationSyntax.GetName();
                return true;
            }

            // we do not find the enclosing method, so we might have a field (for which we need a variable), such as in MiKo_2071_EnumMethodSummaryAnalyzer
            var variableDeclaratorSyntax = query.GetEnclosing<VariableDeclaratorSyntax>();
            if (variableDeclaratorSyntax != null)
            {
                result = variableDeclaratorSyntax;
                identifier = variableDeclaratorSyntax.GetName();
                return true;
            }

            // we might have a constructor here
            var ctorDeclarationSyntax = query.GetEnclosing<ConstructorDeclarationSyntax>();
            if (ctorDeclarationSyntax != null)
            {
                result = ctorDeclarationSyntax;
                identifier = ctorDeclarationSyntax.GetName();
                return true;
            }

            result = null;
            identifier = null;
            return false;
        }

        private void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (QueryExpressionSyntax)context.Node;

            var diagnostic = AnalyzeQueryExpression(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeQueryExpression(QueryExpressionSyntax query, SemanticModel semanticModel)
        {
            if (TryFindSyntaxNode(query, out var syntaxNode, out var identifier) && syntaxNode.HasLinqExtensionMethod(semanticModel))
            {
                return Issue(identifier, query);
            }

            return null;
        }
    }
}