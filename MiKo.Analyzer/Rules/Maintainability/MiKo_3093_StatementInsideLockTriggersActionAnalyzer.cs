using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    //// <seealso cref="MiKo_3092_StatementInsideLockRaisesEventAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3093_StatementInsideLockTriggersActionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3093";

        public MiKo_3093_StatementInsideLockTriggersActionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            foreach (var token in lockStatement.DescendantTokens().Where(_ => _.IsKind(SyntaxKind.IdentifierToken)))
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
                    switch (identifier.Parent.Kind())
                    {
                        case SyntaxKind.ConditionalAccessExpression:
                        case SyntaxKind.InvocationExpression:
                        {
                            var method = context.GetEnclosingMethod();
                            var issue = Issue(method.Name, token);
                            context.ReportDiagnostic(issue);

                            break;
                        }
                    }
                }
            }
        }
    }
}