using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6007_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6007";

        public MiKo_6007_ObjectUnderTestStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
        }

        private static bool HasNoOtherObjectUnderTestExpression(IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                if (statement is ExpressionStatementSyntax ess && ess.IsInvocationOnObjectUnderTest())
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ExpressionStatementSyntax)context.Node;
            var issue = AnalyzeExpressionStatement(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeExpressionStatement(ExpressionStatementSyntax node)
        {
            if (node.IsInvocationOnObjectUnderTest())
            {
                return AnalyzeExpressionStatementBlock(node);
            }

            return null;
        }

        private Diagnostic AnalyzeExpressionStatementBlock(CSharpSyntaxNode node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case BlockSyntax block:
                        return AnalyzeExpressionStatements(block.Statements, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeExpressionStatements(section.Statements, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeExpressionStatements(SyntaxList<StatementSyntax> statements, CSharpSyntaxNode node)
        {
            var callLineSpan = node.GetLocation().GetLineSpan();

            var noBlankLinesBefore = HasNoOtherObjectUnderTestExpression(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
            var noBlankLinesAfter = HasNoOtherObjectUnderTestExpression(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(node, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}