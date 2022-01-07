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
                    foreach (var argument in argumentList.Arguments)
                    {
                        var symbol = argument.GetTypeSymbol(semanticModel);
                        if (symbol.IsException())
                        {
                            // seems like we found the inner exception
                            return false;
                        }
                    }

                    // seems like this is an exception with no inner exception
                    return true;
                }
            }

            // it's no exception that gets created here
            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var catchClause = node.Ancestors().OfType<CatchClauseSyntax>().FirstOrDefault();
            if (catchClause != null)
            {
                // we found an exception inside a catch block that does not get the caught exception as inner exception
                yield return Issue(node.Type.ToString(), node);
            }

            // inspect any 'if' or 'switch' or 'else if' to see if there is an exception involved
            var condition = node.GetRelatedCondition();
            if (condition != null)
            {
                if (condition.DescendantNodes().OfType<ExpressionSyntax>().Any(_ => _.GetTypeSymbol(semanticModel)?.IsException() is true))
                {
                    yield return Issue(node.Type.ToString(), node);
                }
            }
        }
    }
}