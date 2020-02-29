using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3012";

        private static readonly HashSet<string> AllowedExceptionTypes = new HashSet<string>
                                                                            {
                                                                                nameof(ArgumentOutOfRangeException),
                                                                                nameof(InvalidEnumArgumentException),
                                                                                TypeNames.ArgumentOutOfRangeException,
                                                                                TypeNames.InvalidEnumArgumentException,
                                                                            };

        public MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => AllowedExceptionTypes.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.ArgumentList?.Arguments.Count != 3)
            {
                yield return Issue(node.Type.ToString(), node);
            }
        }
    }
}