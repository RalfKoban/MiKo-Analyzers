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

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => HasIssue(node, semanticModel)
                                                                                                                                              ? new[] { Issue(node.Type.ToString(), node) }
                                                                                                                                              : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var problematicNode = node.GetExceptionSwallowingNode(() => semanticModel);

            switch (problematicNode)
            {
                case null:
                    return false;

                case CatchClauseSyntax _:
                    // always report missing exceptions inside catch clauses
                    return true;

                default:
                    var typeSymbol = node.GetTypeSymbol(semanticModel);

                    // do not report argument exceptions as they most probably are used to verify arguments
                    return typeSymbol.InheritsFrom<ArgumentException>() is false;
            }
        }
    }
}