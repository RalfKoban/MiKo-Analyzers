using System;
using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => Mappings.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);

            if (method?.Parameters.Length is 0)
            {
                return new[] { Issue(node.Type) };
            }

            return Array.Empty<Diagnostic>();
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is IMethodSymbol method && method.Parameters.Length is 0)
            {
                if (context.Node is InvocationExpressionSyntax node)
                {
                    var arguments = node.ArgumentList.Arguments;

                    if (arguments.Count > 0 && node.GetName().AsSpan().StartsWith("Throw"))
                    {
                        if (Mappings.Contains(node.GetIdentifierName()))
                        {
                            ReportDiagnostics(context, Issue(node.Expression));
                        }
                    }
                }
            }
        }
    }
}