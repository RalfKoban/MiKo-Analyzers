using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3092_StatementInsideLockRaisesEventAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3092";

        public MiKo_3092_StatementInsideLockRaisesEventAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;

            var method = context.GetEnclosingMethod();
            var events = method.ContainingType.GetMembersIncludingInherited<IEventSymbol>().Select(_ => _.Name).ToHashSet();

            foreach (var token in lockStatement.DescendantTokens().Where(_ => _.IsKind(SyntaxKind.IdentifierToken)))
            {
                var eventName = token.ValueText;

                if (events.Contains(eventName) && token.GetSymbol(context.SemanticModel) is IEventSymbol)
                {
                    var issue = Issue(method.Name, token);
                    context.ReportDiagnostic(issue);
                }
            }
        }
    }
}