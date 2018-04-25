using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3014";

        public MiKo_3014_InvalidOperationNotSupportedNotImplementedExceptionAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var type = node.Type.ToString();
            switch (type)
            {
                case nameof(InvalidOperationException):
                case nameof(NotImplementedException):
                case nameof(NotSupportedException):
                    return true;

                default:
                    return false;
            }
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => node.ArgumentList.Arguments.Count == 0
                                                                                                                                                  ? new []{ ReportIssue(node.Type.ToString(), node.GetLocation()) }
                                                                                                                                                  : Enumerable.Empty<Diagnostic>();
    }
}