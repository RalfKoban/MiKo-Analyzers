using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3242_RaceConditionOnEventRaiseAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3242";

        public MiKo_3242_RaceConditionOnEventRaiseAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);

        private static bool HasIssue(in SyntaxNodeAnalysisContext context, IfStatementSyntax node)
        {
            var condition = node.Condition;

            switch (condition)
            {
                case BinaryExpressionSyntax b when b.Left is IdentifierNameSyntax bi:
                    return HasIssue(context, node, bi);

                case IsPatternExpressionSyntax p when p.Expression is IdentifierNameSyntax pi:
                    return HasIssue(context, node, pi);
            }

            return false;
        }

        private static bool HasIssue(in SyntaxNodeAnalysisContext context, IfStatementSyntax node, IdentifierNameSyntax identifier)
        {
            if (node.Condition.IsNullCheck(identifier))
            {
                switch (node.Statement)
                {
                    case ExpressionStatementSyntax e:
                    {
                        return HasIssue(context, e, identifier);
                    }

                    case BlockSyntax block:
                    {
                        var statements = block.Statements;

                        return statements.Count is 1 && statements[0] is ExpressionStatementSyntax e && HasIssue(context, e, identifier);
                    }
                }
            }

            return false;
        }

        private static bool HasIssue(in SyntaxNodeAnalysisContext context, ExpressionStatementSyntax e, IdentifierNameSyntax identifier)
        {
            if (e.Expression is InvocationExpressionSyntax i && i.Expression is IdentifierNameSyntax invokedIdentifier && identifier.GetName() == invokedIdentifier.GetName())
            {
                var symbol = identifier.GetSymbol(context.SemanticModel);

                return symbol is IEventSymbol;
            }

            return false;
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is IfStatementSyntax node && HasIssue(context, node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}