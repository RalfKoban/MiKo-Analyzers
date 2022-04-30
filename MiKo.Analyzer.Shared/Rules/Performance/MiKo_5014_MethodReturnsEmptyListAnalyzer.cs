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
            var method = symbol.GetSyntax<BaseMethodDeclarationSyntax>();

            var expressionBody = method.ExpressionBody;
            if (expressionBody != null)
            {
                foreach (var creation in expressionBody.DescendantNodes<ObjectCreationExpressionSyntax>().Where(HasIssue))
                {
                    yield return Issue(symbol.Name, creation);
                }
            }
            else
            {
                foreach (var creation in method.Body.DescendantNodes<ReturnStatementSyntax>()
                                               .SelectMany(_ => _.DescendantNodes<ObjectCreationExpressionSyntax>().Where(HasIssue)))
                {
                    yield return Issue(symbol.Name, creation);
                }
            }
        }

        private static bool HasIssue(ObjectCreationExpressionSyntax creation)
        {
            if (creation.ArgumentList?.Arguments.Count == 0 || creation.Initializer?.Expressions.Count == 0)
            {
                var name = creation.Type.GetNameOnlyPartWithoutGeneric();
                if (name == "List")
                {
                    return true;
                }
            }

            return false;
        }
    }
}