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
            switch (other.Kind())
            {
                case SyntaxKind.Block:
                case SyntaxKind.IfStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.WhileStatement:
                    return true;

                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)other).Expression.IsKind(SyntaxKind.InvocationExpression);

                default:
                    return false;
            }
        }

        private static bool HasNoBlankLinesBefore(SyntaxNode current, int previousIndex, SyntaxList<StatementSyntax> statements)
        {
            if (previousIndex >= 0)
            {
                var before = statements[previousIndex];

                return IsProblematicStatement(before) && HasNoBlankLinesBefore(current, before);
            }

            return false;
        }

        private static bool HasNoBlankLinesAfter(SyntaxNode current, int nextIndex, SyntaxList<StatementSyntax> statements, int statementsCount, AssignmentExpressionSyntax assignment)
        {
            if (nextIndex >= statementsCount)
            {
                return false;
            }

            var next = statements[nextIndex];

            if (IsProblematicStatement(next) && HasNoBlankLinesAfter(current, next))
            {
                if (next is ExpressionStatementSyntax nextStatement && assignment.Left is IdentifierNameSyntax identifier)
                {
                    var expression = FindExpressionSyntax(nextStatement);

                    var assignedIdentifier = identifier.GetName();
                    var accessedIdentifier = expression.GetName();

                    return assignedIdentifier != accessedIdentifier;
                }

                return true;
            }

            return false;

            ExpressionSyntax FindExpressionSyntax(ExpressionStatementSyntax statement)
            {
                var expression = statement.Expression;

                var loop = true;

                while (loop)
                {
                    switch (expression)
                    {
                        case InvocationExpressionSyntax i: expression = i.Expression; break;
                        case MemberAccessExpressionSyntax m: expression = m.Expression; break;
                        default:
                            loop = false;

                            break;
                    }
                }

                return expression;
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

                if (current is ExpressionStatementSyntax statement && statement.Expression is AssignmentExpressionSyntax assignment && assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var hasNoBlankLinesBefore = HasNoBlankLinesBefore(current, index - 1, statements);
                    var hasNoBlankLinesAfter = HasNoBlankLinesAfter(current, index + 1, statements, statementsCount, assignment);

                    if (hasNoBlankLinesBefore || hasNoBlankLinesAfter)
                    {
                        yield return Issue(current, hasNoBlankLinesBefore, hasNoBlankLinesAfter);
                    }
                }
            }
        }
    }
}