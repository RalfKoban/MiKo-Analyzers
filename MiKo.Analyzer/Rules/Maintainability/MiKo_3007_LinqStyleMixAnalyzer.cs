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

        private void AnalyzeQueryExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (QueryExpressionSyntax)context.Node;

            var diagnostic = AnalyzeQueryExpression(node, context.SemanticModel);
            if (diagnostic != null) context.ReportDiagnostic(diagnostic);
        }

        private Diagnostic AnalyzeQueryExpression(QueryExpressionSyntax query, SemanticModel semanticModel)
        {
            var hasLinqMethods = false;
            if (TryFindSyntaxNode(query, out var syntaxNode, out var identifier))
            {
                hasLinqMethods = syntaxNode
                                     .DescendantNodes()
                                     .OfType<MemberAccessExpressionSyntax>()
                                     .SelectMany(_ => semanticModel.LookupSymbols(_.GetLocation().SourceSpan.Start))
                                     .Select(_ => _.ContainingNamespace?.ToString())
                                     .Where(_ => _ != null)
                                     .Any(_ => _.StartsWith("System.Linq", StringComparison.OrdinalIgnoreCase));
            }

            return hasLinqMethods
                       ? ReportIssue(identifier, query.GetLocation())
                       : null;
        }

        private static bool TryFindSyntaxNode(QueryExpressionSyntax query, out SyntaxNode syntaxNode, out string identifier)
        {
            var methodDeclarationSyntax = query.GetEnclosing<MethodDeclarationSyntax>();
            if (methodDeclarationSyntax != null)
            {
                syntaxNode = methodDeclarationSyntax;
                identifier = methodDeclarationSyntax.Identifier.ValueText;
                return true;
            }

            // we do not find the enclosing method, so we might have a field (for which we need a variable), such as in MiKo_2071_EnumMethodSummaryAnalyzer
            var variableDeclaratorSyntax = query.GetEnclosing<VariableDeclaratorSyntax>();
            if (variableDeclaratorSyntax != null)
            {
                syntaxNode = variableDeclaratorSyntax;
                identifier = variableDeclaratorSyntax.Identifier.ValueText;
                return true;
            }

            // we might have a constructor here
            var ctorDeclarationSyntax = query.GetEnclosing<ConstructorDeclarationSyntax>();
            if (ctorDeclarationSyntax != null)
            {
                syntaxNode = ctorDeclarationSyntax;
                identifier = ctorDeclarationSyntax.Identifier.ValueText;
                return true;
            }

            syntaxNode = null;
            identifier = null;
            return false;
        }
    }
}