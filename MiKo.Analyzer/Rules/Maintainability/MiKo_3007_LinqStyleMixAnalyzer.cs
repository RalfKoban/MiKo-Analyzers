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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeQueryExpression, SyntaxKind.QueryExpression);

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
            if (foundNode is false)
            {
                return null;
            }

            if (query.HasLinqExtensionMethod(semanticModel))
            {
                // query itself contains Linq calls, so report an issue anyway
                return Issue(identifier, query);
            }

            InvocationExpressionSyntax firstCall = null;

            var calls = 0;
            foreach (var call in syntaxNode.LinqExtensionMethods(semanticModel))
            {
                calls++;

                switch (calls)
                {
                    case 1:
                        firstCall = call;
                        break;

                    case 2:
                        // no need to look further, found at least one additional call
                        return Issue(identifier, query);
                }
            }

            if (calls == 0)
            {
                return null;
            }

            // ignore "ToList" etc
            if (firstCall?.Expression is MemberAccessExpressionSyntax m)
            {
                switch (m.GetName())
                {
                    case nameof(Enumerable.ToList):
                    case nameof(Enumerable.ToArray):
                    case nameof(Enumerable.ToDictionary):
                    case nameof(Enumerable.ToLookup):
                    {
                        // that are the only calls which shall be allowed
                        return null;
                    }
                }
            }

            return Issue(identifier, query);
        }
    }
}