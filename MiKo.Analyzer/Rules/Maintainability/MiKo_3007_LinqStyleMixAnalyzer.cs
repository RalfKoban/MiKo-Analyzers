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
            result = query.GetEnclosing(SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.VariableDeclarator);
            switch (result)
            {
                case MethodDeclarationSyntax m:
                    identifier = m.GetName();
                    return true;

                // we have a constructor here
                case ConstructorDeclarationSyntax c:
                    identifier = c.GetName();
                    return true;

                // we do not find the enclosing method, so we might have a field (for which we need a variable), such as in MiKo_2071_EnumMethodSummaryAnalyzer
                case VariableDeclaratorSyntax v:
                    identifier = v.GetName();
                    return true;

                // found something else
                default:
                    result = null;
                    identifier = null;
                    return false;
            }
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