using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3210_IfStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3210";

        public MiKo_3210_IfStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (IfStatementSyntax)context.Node;
            var issue = AnalyzeIfStatement(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeIfStatement(IfStatementSyntax node)
        {
            if (node.Parent is ElseClauseSyntax)
            {
                // do not report 'else if' blocks
                return null;
            }

            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
                    case BlockSyntax block:
                        return AnalyzeIfStatement(block.Statements, node);

                    case SwitchSectionSyntax section:
                        return AnalyzeIfStatement(section.Statements, node);

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeIfStatement(SyntaxList<StatementSyntax> statements, IfStatementSyntax node)
        {
            var callLineSpan = node.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Any(_ => HasNoBlankLinesBefore(callLineSpan, _));
            var noBlankLinesAfter = statements.Any(_ => HasNoBlankLinesAfter(callLineSpan, _));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(node.IfKeyword, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}