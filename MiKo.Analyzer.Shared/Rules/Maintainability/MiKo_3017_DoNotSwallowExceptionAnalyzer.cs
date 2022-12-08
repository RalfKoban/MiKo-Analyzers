using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3017_DoNotSwallowExceptionAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3017";

        public MiKo_3017_DoNotSwallowExceptionAnalyzer() : base(Id)
        {
        }

        internal static SyntaxNode FindProblematicSyntaxNode(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var catchClause = node.FirstAncestorOrSelf<CatchClauseSyntax>();

            if (catchClause != null)
            {
                // we found an exception inside a catch block that does not get the caught exception as inner exception
                return catchClause;
            }

            // inspect any 'if' or 'switch' or 'else if' to see if there is an exception involved
            var expression = node.GetRelatedCondition()?.FirstDescendant<ExpressionSyntax>(_ => _.GetTypeSymbol(semanticModel)?.IsException() is true);

            if (expression != null)
            {
                return expression;
            }

            // inspect method arguments
            var parameter = node.GetEnclosing<MethodDeclarationSyntax>()?.ParameterList.Parameters.FirstOrDefault(_ => _.Type.IsException());

            if (parameter != null)
            {
                return parameter;
            }

            return null;
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var typeSymbol = node.GetTypeSymbol(semanticModel);

            if (typeSymbol.IsException())
            {
                var argumentList = node.ArgumentList;

                if (argumentList != null)
                {
                    if (argumentList.Arguments.Any(_ => _.GetTypeSymbol(semanticModel)?.IsException() is true))
                    {
                        // seems like we found the inner exception
                        return false;
                    }

                    // seems like this is an exception with no inner exception, so see if the exception type supports creation via exceptions
                    return typeSymbol.GetMethods(MethodKind.Constructor).Any(_ => _.Parameters.Any(__ => __.Type.IsException()));
                }
            }

            // it's no exception that gets created here
            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            if (HasIssue(node, semanticModel))
            {
                yield return Issue(node.Type.ToString(), node);
            }
        }

        private static bool HasIssue(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var problematicNode = FindProblematicSyntaxNode(node, semanticModel);

            switch (problematicNode)
            {
                case null:
                    return false;

                case CatchClauseSyntax _:
                    // always report missing exceptions inside catch clauses
                    return true;

                default:
                    // do not report argument exceptions as they most probably are used to verify arguments
                    return node.GetTypeSymbol(semanticModel).InheritsFrom<ArgumentException>() is false;
            }
        }
    }
}