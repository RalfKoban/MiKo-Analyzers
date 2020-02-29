using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3015";

        private static readonly HashSet<string> Mappings = new HashSet<string>
                                                               {
                                                                   nameof(ArgumentException),
                                                                   TypeNames.ArgumentException,

                                                                   nameof(ArgumentNullException),
                                                                   TypeNames.ArgumentNullException,

                                                                   nameof(ArgumentOutOfRangeException),
                                                                   TypeNames.ArgumentOutOfRangeException,

                                                                   nameof(InvalidEnumArgumentException),
                                                                   TypeNames.InvalidEnumArgumentException,
                                                               };

        public MiKo_3015_ArgumentExceptionThrownAtWrongPlaceAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => Mappings.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);
            if (method != null && method.Parameters.Length == 0)
            {
                yield return Issue(node.Type.ToString(), node);
            }
        }
    }
}