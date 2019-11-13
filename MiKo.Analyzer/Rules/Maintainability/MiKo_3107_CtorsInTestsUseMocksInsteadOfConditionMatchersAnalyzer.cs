using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3107_CtorsInTestsUseMocksInsteadOfConditionMatchersAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3107";

        public MiKo_3107_CtorsInTestsUseMocksInsteadOfConditionMatchersAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null)
            {
                yield break;
            }

            var method = node.GetEnclosingMethod(semanticModel);
            if (method is null)
            {
                yield break;
            }

            foreach (var argument in argumentList.Arguments)
            {
                if (argument.Expression is InvocationExpressionSyntax ies)
                {
                    if (ies.Expression is MemberAccessExpressionSyntax mae && mae.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        if (IsConditionMatcher(mae))
                        {
                            yield return Issue(method.Name, argument.GetLocation());
                        }
                    }
                }
            }
        }

        private static bool IsConditionMatcher(MemberAccessExpressionSyntax node)
        {
            if (node.Expression is IdentifierNameSyntax invokedType && invokedType.Identifier.ValueText == "It")
            {
                switch (node.Name.Identifier.ValueText)
                {
                    case "Is":
                    case "IsAny":
                        return true;
                }
            }

            return false;
        }
    }
}