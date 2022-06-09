using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3205";

        public MiKo_3205_ReturnStatementPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeReturnStatementSyntax, SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement);
        }

        private void AnalyzeReturnStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            var issue = AnalyzeReturnStatementSyntax(context.Node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeReturnStatementSyntax(SyntaxNode statement)
        {
            foreach (var ancestor in statement.Ancestors())
            {
                switch (ancestor)
                {
                    case BlockSyntax block:
                        return AnalyzeStatements(block.Statements, statement);

                    case SwitchSectionSyntax section:
                        return AnalyzeStatements(section.Statements, statement);

                    case IfStatementSyntax _:
                    case ElseClauseSyntax _:
                        return null; // no issue

                    case MethodDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                        return null; // stop lookup as there is no valid ancestor anymore
                }
            }

            return null;
        }

        private Diagnostic AnalyzeStatements(SyntaxList<StatementSyntax> statements, SyntaxNode returnStatement)
        {
            var callLineSpan = returnStatement.GetLocation().GetLineSpan();
            var noBlankLinesBefore = statements.Where(_ => _.IsKind(SyntaxKind.YieldReturnStatement) is false)
                                               .Any(_ => HasNoBlankLinesBefore(callLineSpan, _));

            if (noBlankLinesBefore)
            {
                switch (returnStatement)
                {
                    case ReturnStatementSyntax s when s.Expression is null:
                        return null; // no issue

                    case ReturnStatementSyntax s:
                        return Issue(s.ReturnKeyword, true, false);

                    case YieldStatementSyntax y:
                        return Issue(y.YieldKeyword, true, false);
                }
            }

            return null;
        }
    }
}