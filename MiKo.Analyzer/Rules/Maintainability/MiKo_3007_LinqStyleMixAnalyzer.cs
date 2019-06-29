using System;
using System.Linq;

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
                identifier = methodDeclarationSyntax.Identifier.ValueText;
                return true;
            }

            // we do not find the enclosing method, so we might have a field (for which we need a variable), such as in MiKo_2071_EnumMethodSummaryAnalyzer
            var variableDeclaratorSyntax = query.GetEnclosing<VariableDeclaratorSyntax>();
            if (variableDeclaratorSyntax != null)
            {
                result = variableDeclaratorSyntax;
                identifier = variableDeclaratorSyntax.Identifier.ValueText;
                return true;
            }

            // we might have a constructor here
            var ctorDeclarationSyntax = query.GetEnclosing<ConstructorDeclarationSyntax>();
            if (ctorDeclarationSyntax != null)
            {
                result = ctorDeclarationSyntax;
                identifier = ctorDeclarationSyntax.Identifier.ValueText;
                return true;
            }

            result = null;
            identifier = null;
            return false;
        }

        private static bool HasLinqExtensionMethod(SyntaxNode syntaxNode, SemanticModel semanticModel)
        {
            return syntaxNode.DescendantNodes()
                             .OfType<InvocationExpressionSyntax>()
                             .Select(_ => semanticModel.GetSymbolInfo(_))
                             .Any(IsLinqExtensionMethod);
        }

        private static bool IsLinqExtensionMethod(SymbolInfo info) => IsLinqExtensionMethod(info.Symbol) || info.CandidateSymbols.Any(IsLinqExtensionMethod);

        private static bool IsLinqExtensionMethod(ISymbol symbol)
        {
            if (symbol is IMethodSymbol)
            {
                // this is an extension method !
                if (symbol.ContainingNamespace.ToDisplayString().StartsWith("System.Linq", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

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

        private Diagnostic AnalyzeQueryExpression(QueryExpressionSyntax query, SemanticModel semanticModel) => TryFindSyntaxNode(query, out var syntaxNode, out var identifier) && HasLinqExtensionMethod(syntaxNode, semanticModel)
                                                                                                                   ? Issue(identifier, query.GetLocation())
                                                                                                                   : null;
    }
}