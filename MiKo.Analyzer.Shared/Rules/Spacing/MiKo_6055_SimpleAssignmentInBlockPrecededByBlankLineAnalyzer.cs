using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6055_SimpleAssignmentInBlockPrecededByBlankLineAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6055";

        private static readonly SyntaxKind[] SyntaxKinds = { SyntaxKind.Block, SyntaxKind.SwitchSection };

        public MiKo_6055_SimpleAssignmentInBlockPrecededByBlankLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKinds);

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

            for (var index = 1; index < statementsCount; index++)
            {
                var current = statements[index];

                if (current is ExpressionStatementSyntax statement && statement.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var before = statements[index - 1];

                    switch (before)
                    {
                        case BlockSyntax _:
                        case IfStatementSyntax _:
                        case ExpressionStatementSyntax beforeStatement when beforeStatement.Expression.IsKind(SyntaxKind.InvocationExpression):
                        {
                            if (HasNoBlankLinesBefore(current, before))
                            {
                                yield return Issue(current, true, false);
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}