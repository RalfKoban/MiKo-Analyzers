using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3014";

        private static readonly HashSet<string> AllowedExceptionTypes = new HashSet<string>
                                                                            {
                                                                                nameof(InvalidOperationException),
                                                                                nameof(NotImplementedException),
                                                                                nameof(NotSupportedException),
                                                                                TypeNames.InvalidOperationException,
                                                                                TypeNames.NotImplementedException,
                                                                                TypeNames.NotSupportedException,
                                                                            };

        public MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => AllowedExceptionTypes.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var argumentList = node.ArgumentList;

            if (argumentList?.Arguments.Count == 0)
            {
                yield return Issue(node.Type.ToString(), argumentList);
            }
        }
    }
}