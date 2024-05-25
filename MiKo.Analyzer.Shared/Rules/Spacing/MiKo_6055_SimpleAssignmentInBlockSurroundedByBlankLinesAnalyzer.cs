using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6055_SimpleAssignmentInBlockSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6055";

        private static readonly SyntaxKind[] SyntaxKinds = { SyntaxKind.Block, SyntaxKind.SwitchSection };

        public MiKo_6055_SimpleAssignmentInBlockSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKinds);

        private static bool IsProblematicStatement(StatementSyntax other)
        {
            switch (other)
            {
                case BlockSyntax _:
                case IfStatementSyntax _:
                case UsingStatementSyntax _:
                case LockStatementSyntax _:
                case SwitchStatementSyntax _:
                case DoStatementSyntax _:
                case ForEachStatementSyntax _:
                case ForStatementSyntax _:
                case WhileStatementSyntax _:
                case ExpressionStatementSyntax otherStatement when otherStatement.Expression.IsKind(SyntaxKind.InvocationExpression):
                    return true;

                default:
                    return false;
            }
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var issues = AnalyzeNode(context.Node);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeNode(SyntaxNode node)
        {
            switch (node)
            {
                case BlockSyntax block:
                    return AnalyzeStatements(block.Statements);

                case SwitchSectionSyntax section:
                    return AnalyzeStatements(section.Statements);

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        private IEnumerable<Diagnostic> AnalyzeStatements(SyntaxList<StatementSyntax> statements)
        {
            var statementsCount = statements.Count;

            for (var index = 0; index < statementsCount; index++)
            {
                var current = statements[index];

                if (current is ExpressionStatementSyntax statement && statement.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var hasNoBlankLinesBefore = false;
                    var hasNoBlankLinesAfter = false;

                    var previousIndex = index - 1;
                    var nextIndex = index + 1;

                    if (previousIndex >= 0)
                    {
                        var before = statements[previousIndex];

                        hasNoBlankLinesBefore = IsProblematicStatement(before) && HasNoBlankLinesBefore(current, before);
                    }

                    if (nextIndex < statementsCount)
                    {
                        var next = statements[nextIndex];

                        hasNoBlankLinesAfter = IsProblematicStatement(next) && HasNoBlankLinesAfter(current, next);
                    }

                    if (hasNoBlankLinesBefore || hasNoBlankLinesAfter)
                    {
                        yield return Issue(current, hasNoBlankLinesBefore, hasNoBlankLinesAfter);
                    }
                }
            }
        }
    }
}