using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5014_MethodReturnsEmptyListAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5014";

        public MiKo_5014_MethodReturnsEmptyListAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid is false && symbol.ContainingType.TypeKind is TypeKind.Class)
            {
                var returnType = symbol.ReturnType;

                if (returnType.IsTask() || returnType.IsValueTask())
                {
                    return returnType.TryGetGenericArgumentType(out var generic) && ShallAnalyze(generic);
                }

                return ShallAnalyze(returnType);
            }

            return false;
        }

        private static bool ShallAnalyze(ITypeSymbol returnType)
        {
            if (returnType.TypeKind is TypeKind.Interface)
            {
                switch (returnType.OriginalDefinition.SpecialType)
                {
                    case SpecialType.System_Collections_Generic_IEnumerable_T:
                    case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                    case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                        return true;
                }
            }

            return false;
        }

        private static bool HasIssue(ObjectCreationExpressionSyntax creation)
        {
            if (creation.Type is GenericNameSyntax generic && generic.GetName() is "List")
            {
                var arguments = creation.ArgumentList?.Arguments;

                switch (arguments?.Count)
                {
                    case null:
                    case 0:
                    case 1 when arguments.GetValueOrDefault()[0].Expression is LiteralExpressionSyntax:
                    {
                        var initializer = creation.Initializer;

                        return initializer is null || initializer.Expressions.Count is 0;
                    }
                }
            }

            return false;
        }

        private static bool GetsReturned(ObjectCreationExpressionSyntax node)
        {
            foreach (var ancestor in node.AncestorsWithinMethods())
            {
                switch (ancestor)
                {
                    case ArrowExpressionClauseSyntax _:
                    case ReturnStatementSyntax _:
                        return true;

                    case AssignmentExpressionSyntax _:
                        return false;

                    case ArgumentSyntax argument:
                    {
                        // it shall be an issue when we have a read-only collection that uses the list as constructor parameter
                        switch (argument.Parent?.Parent)
                        {
                            case ObjectCreationExpressionSyntax o when o.Type is GenericNameSyntax generic:
                            {
                                var name = generic.GetName();

                                return name is "ReadOnlyCollection" || name is nameof(ValueTask);
                            }

                            case InvocationExpressionSyntax i when i.GetName() is nameof(Task.FromResult) && i.GetIdentifierName() is nameof(Task):
                                return true;

                            default:
                                return false;
                        }
                    }
                }
            }

            return false;
        }

        private void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ObjectCreationExpressionSyntax node && context.ContainingSymbol is IMethodSymbol method)
            {
                if (ShallAnalyze(method) && HasIssue(node) && GetsReturned(node))
                {
                    ReportDiagnostics(context, Issue(method.Name, node));
                }
            }
        }
    }
}