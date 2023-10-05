using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    //// <seealso cref="MiKo_3092_StatementInsideLockRaisesEventAnalyzer"/>
    //// <seealso cref="MiKo_3093_StatementInsideLockTriggersActionAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3094_StatementInsideLockCallsParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3094";

        private static readonly SyntaxKind[] AccessOrInvocations = { SyntaxKind.ConditionalAccessExpression, SyntaxKind.InvocationExpression, SyntaxKind.MemberBindingExpression, SyntaxKind.SimpleMemberAccessExpression };

        public MiKo_3094_StatementInsideLockCallsParameterAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);

        private static bool IsAccessOrInvocation(SyntaxNode node) => node?.IsAnyKind(AccessOrInvocations) is true;

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var issues = AnalyzeLockStatement(context, context.Node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeLockStatement(SyntaxNodeAnalysisContext context, SyntaxNode lockStatement)
        {
            var method = context.GetEnclosingMethod();

            if (method is null)
            {
                // nothing to analyze because it is no method
                yield break;
            }

            var parameterNames = method.Parameters
                                       .Where(_ => _.Type.IsReferenceType && _.Type.IsString() is false) // ignore value types and strings
                                       .ToHashSet(_ => _.Name);

            if (parameterNames.None())
            {
                // nothing to analyze because there are no parameters
                yield break;
            }

            var semanticModel = context.SemanticModel;

            foreach (var token in lockStatement.DescendantTokens(SyntaxKind.IdentifierToken))
            {
                if (parameterNames.Contains(token.ValueText))
                {
                    var identifier = token.Parent;
                    var parent = identifier?.Parent;

                    if (IsAccessOrInvocation(parent))
                    {
                        var type = identifier.GetTypeSymbol(semanticModel);

                        if (type?.TypeKind == TypeKind.Delegate)
                        {
                            // found by rule MiKo 3092 or MiKo 3093
                            continue;
                        }

                        yield return Issue(parent);
                    }
                }
            }
        }
    }
}