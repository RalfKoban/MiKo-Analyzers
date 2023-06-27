using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6006_AwaitStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6006";

        public MiKo_6006_AwaitStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
        }

        private static bool HasNonAwaitedExpression(IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                if (statement is ExpressionStatementSyntax ess && ess.Expression.IsKind(SyntaxKind.AwaitExpression))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static bool HasNonAwaitedLocalDeclaration(IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case LocalDeclarationStatementSyntax l1 when l1.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                    case LocalDeclarationStatementSyntax l2 when l2.Declaration.DescendantNodes().Any(_ => _.IsKind(SyntaxKind.AwaitExpression)):
                        continue;

                    default:
                        return true;
                }
            }

            return false;
        }

        private void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (AwaitExpressionSyntax)context.Node;
            var issue = AnalyzeAwaitExpression(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeAwaitExpression(AwaitExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case ParenthesizedLambdaExpressionSyntax _:
                        return null; // stop lookup if it is a parameter

                    case BlockSyntax block:
                        return AnalyzeAwaitExpression(block.Statements, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeAwaitExpression(section.Statements, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeAwaitExpression(SyntaxList<StatementSyntax> statements, AwaitExpressionSyntax node)
        {
            switch (node.Parent)
            {
                case StatementSyntax _:
                {
                    var callLineSpan = node.GetLocation().GetLineSpan();

                    var noBlankLinesBefore = HasNonAwaitedExpression(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
                    var noBlankLinesAfter = HasNonAwaitedExpression(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

                    if (noBlankLinesBefore || noBlankLinesAfter)
                    {
                        return Issue(node.AwaitKeyword, noBlankLinesBefore, noBlankLinesAfter);
                    }

                    break;
                }

                case EqualsValueClauseSyntax _:
                case AssignmentExpressionSyntax _:
                {
                    var callLineSpan = node.GetLocation().GetLineSpan();

                    var noBlankLinesBefore = HasNonAwaitedLocalDeclaration(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
                    var noBlankLinesAfter = HasNonAwaitedLocalDeclaration(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

                    if (noBlankLinesBefore || noBlankLinesAfter)
                    {
                        return Issue(node.AwaitKeyword, noBlankLinesBefore, noBlankLinesAfter);
                    }

                    break;
                }
            }

            return null;
        }
    }
}