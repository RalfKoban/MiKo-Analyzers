using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3012";

        public MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            switch (node.Type.ToString())
            {
                case nameof(ArgumentOutOfRangeException):
                case nameof(InvalidEnumArgumentException):
                case "System." + nameof(ArgumentOutOfRangeException):
                case "System.ComponentModel." + nameof(InvalidEnumArgumentException):
                    return true;

                default:
                    return false;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => node.ArgumentList.Arguments.Count == 3
                                                                                                                                                  ? Enumerable.Empty<Diagnostic>()
                                                                                                                                                  : new []{ ReportIssue(node.Type.ToString(), node.GetLocation()) };
    }
}