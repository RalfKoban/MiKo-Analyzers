using System.Collections.Generic;
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

        private static bool TryFindInspectionTarget(SyntaxNode query, out SyntaxNode result, out string identifier)
        {
            result = query.GetEnclosing(SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.FieldDeclaration);

            switch (result)
            {
                case MethodDeclarationSyntax m:
                    identifier = m.GetName();
                    return true;

                // we have a constructor here
                case ConstructorDeclarationSyntax c:
                    identifier = c.GetName();
                    return true;

                // we have a field
                case FieldDeclarationSyntax f:
                    identifier = f.Declaration.Variables.First().GetName();
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
            var foundNode = TryFindInspectionTarget(query, out var syntaxNode, out var identifier);

            if (query.HasLinqExtensionMethod(semanticModel))
            {
                // query itself contains Linq calls, so report an issue
                return Issue(identifier, query);
            }

            var enclosingNode = query.GetEnclosing(SyntaxKind.SimpleMemberAccessExpression);
            if (enclosingNode is MemberAccessExpressionSyntax m)
            {
                // ignore surrounding "ToList", but only if it is the only call
                switch (m.GetName())
                {
                    case nameof(Enumerable.ToList):
                    case nameof(Enumerable.ToArray):
                    case nameof(Enumerable.ToDictionary):
                    case nameof(Enumerable.ToLookup):
                    {
                        var calls = 0;

                        foreach (var unused in syntaxNode.LinqExtensionMethods(semanticModel))
                        {
                            calls++;

                            if (calls == 2)
                            {
                                // no need to look further, found at least one additional call
                                return Issue(identifier, query);
                            }
                        }

                        // that's the only call which shall be allowed
                        return null;
                    }
                }
            }

            if (foundNode && syntaxNode.HasLinqExtensionMethod(semanticModel))
            {
                return Issue(identifier, query);
            }

            return null;
        }
    }
}