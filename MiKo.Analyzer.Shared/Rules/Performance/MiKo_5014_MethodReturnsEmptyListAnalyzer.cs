using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5014_MethodReturnsEmptyListAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5014";

        public MiKo_5014_MethodReturnsEmptyListAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid is false && symbol.ContainingType.TypeKind == TypeKind.Class)
            {
                var returnType = symbol.ReturnType;

                if (returnType.TypeKind == TypeKind.Interface)
                {
                    switch (returnType.SpecialType)
                    {
                        case SpecialType.None:
                        {
                            switch (returnType.Name)
                            {
                                case "IReadOnlyList":
                                case "IReadOnlyCollection":
                                    return true;

                                default:
                                    return false;
                            }
                        }

                        case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                        case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                            return true;
                    }
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            switch (symbol.GetSyntax())
            {
                case BaseMethodDeclarationSyntax method:
                    return Analyze(method, symbol.Name);

                case AccessorDeclarationSyntax accessor:
                    return Analyze(accessor, symbol.Name);

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        private static bool HasIssue(ObjectCreationExpressionSyntax creation)
        {
            if (creation.ArgumentList?.Arguments.Count == 0 || creation.Initializer?.Expressions.Count == 0)
            {
                var name = creation.Type.GetNameOnlyPartWithoutGeneric();

                return name == "List";
            }

            return false;
        }

        private IEnumerable<Diagnostic> Analyze(AccessorDeclarationSyntax accessor, string symbolName)
        {
            var expressionBody = accessor.ExpressionBody;

            return expressionBody != null
                   ? Analyze(expressionBody, symbolName)
                   : Analyze(accessor.Body, symbolName);
        }

        private IEnumerable<Diagnostic> Analyze(BaseMethodDeclarationSyntax method, string symbolName)
        {
            var expressionBody = method.ExpressionBody;

            return expressionBody != null
                   ? Analyze(expressionBody, symbolName)
                   : Analyze(method.Body, symbolName);
        }

        private IEnumerable<Diagnostic> Analyze(ArrowExpressionClauseSyntax expressionBody, string symbolName)
        {
            return Analyze(expressionBody.DescendantNodes<ObjectCreationExpressionSyntax>(), symbolName);
        }

        private IEnumerable<Diagnostic> Analyze(BlockSyntax body, string symbolName)
        {
            if (body is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return Analyze(body.DescendantNodes<ReturnStatementSyntax>().SelectMany(_ => _.DescendantNodes<ObjectCreationExpressionSyntax>()), symbolName);
        }

        private IEnumerable<Diagnostic> Analyze(IEnumerable<ObjectCreationExpressionSyntax> creations, string symbolName)
        {
            return creations.Where(HasIssue).Select(_ => Issue(symbolName, _));
        }
    }
}