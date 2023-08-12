using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    //// <seealso cref="MiKo_3093_StatementInsideLockTriggersActionAnalyzer"/>
    //// <seealso cref="MiKo_3094_StatementInsideLockCallsParameterAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3092_StatementInsideLockRaisesEventAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3092";

        public MiKo_3092_StatementInsideLockRaisesEventAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;
            var issues = AnalyzeLockStatement(context, lockStatement);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeLockStatement(SyntaxNodeAnalysisContext context, LockStatementSyntax lockStatement)
        {
            foreach (var token in lockStatement.DescendantTokens(SyntaxKind.IdentifierToken))
            {
                switch (token.Parent?.Parent?.Kind())
                {
                    case SyntaxKind.AddAssignmentExpression:
                    case SyntaxKind.SubtractAssignmentExpression:
                    {
                        // we have an event assignment
                        continue;
                    }

                    case SyntaxKind.ConditionalAccessExpression: // MyEvent?.Invoke()
                    case SyntaxKind.InvocationExpression: // MyEvent()
                    case SyntaxKind.EqualsValueClause: // handler()
                    {
                        yield return AnalyzeToken(context, token);

                        continue;
                    }
                }
            }
        }

        private Diagnostic AnalyzeToken(SyntaxNodeAnalysisContext context, SyntaxToken token)
        {
            var eventName = token.ValueText;

            var method = context.GetEnclosingMethod();
            var events = method.ContainingType.GetMembersIncludingInherited<IEventSymbol>().ToHashSet(_ => _.Name);

            if (events.Contains(eventName) && token.GetSymbol(context.SemanticModel) is IEventSymbol)
            {
                return Issue(token);
            }

            return null;
        }
    }
}