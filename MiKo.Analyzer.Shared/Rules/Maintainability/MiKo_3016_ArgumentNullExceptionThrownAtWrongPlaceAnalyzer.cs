using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3016";

        private static readonly HashSet<string> Mappings = new HashSet<string>
                                                               {
                                                                   nameof(ArgumentNullException),
                                                                   TypeNames.ArgumentNullException,
                                                               };

        private static readonly string[] EqualsMethods =
            {
                nameof(Equals),
                nameof(ReferenceEquals),
            };

        public MiKo_3016_ArgumentNullExceptionThrownAtWrongPlaceAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => Mappings.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var condition = node.GetRelatedCondition();
            if (condition is null)
            {
                // nothing there
                yield break;
            }

            var parameter = node.GetUsedParameter();
            if (parameter != null)
            {
                switch (condition)
                {
                    case BinaryExpressionSyntax binary when binary.Left is IdentifierNameSyntax || binary.Right is IdentifierNameSyntax:
                    case IsPatternExpressionSyntax pattern when pattern.Expression is IdentifierNameSyntax:
                    case InvocationExpressionSyntax invocation when invocation.GetName().EqualsAny(EqualsMethods):
                    {
                        // seems like a correct usage
                        yield break;
                    }
                }
            }

            yield return Issue(node.Type);
        }
    }
}