using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3009_CommandInvokeNamedMethodsAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3009";

        public MiKo_3009_CommandInvokeNamedMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => node.Type.IsCommand(semanticModel);

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;
            if (argumentList is null)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var arguments = argumentList.Arguments;
            if (arguments.Count == 0)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            return arguments
                           .Where(_ => _.Expression is LambdaExpressionSyntax)
                           .Select(_ => Issue(_.ToString(), _.GetLocation()))
                           .ToList();
        }
    }
}