using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    //// <seealso cref="MiKo_3092_StatementInsideLockRaisesEventAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3093_StatementInsideLockTriggersActionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3093";

        private static readonly SyntaxKind[] Invocations = { SyntaxKind.ConditionalAccessExpression, SyntaxKind.InvocationExpression };

        public MiKo_3093_StatementInsideLockTriggersActionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);

        private static bool IsInvocation(SyntaxNode identifier) => identifier.Parent?.IsAnyKind(Invocations) is true;

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var issues = AnalyzeLockStatement(context, context.Node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeLockStatement(SyntaxNodeAnalysisContext context, SyntaxNode lockStatement)
        {
            var semanticModel = context.SemanticModel;

            foreach (var token in lockStatement.DescendantTokens(SyntaxKind.IdentifierToken))
            {
                var identifier = token.Parent;

                var type = identifier.GetTypeSymbol(semanticModel);
                if (type?.TypeKind == TypeKind.Delegate)
                {
                    if (token.GetSymbol(semanticModel) is IEventSymbol)
                    {
                        // found by rule MiKo 3092
                        continue;
                    }

                    // only warn if it is an invocation
                    if (IsInvocation(identifier))
                    {
                        var method = context.GetEnclosingMethod();

                        yield return Issue(method.Name, token);
                    }
                }
            }
        }
    }
}