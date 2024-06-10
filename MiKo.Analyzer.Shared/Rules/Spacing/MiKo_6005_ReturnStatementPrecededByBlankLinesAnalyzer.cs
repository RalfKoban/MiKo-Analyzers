using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6005_ReturnStatementPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6005";

        public MiKo_6005_ReturnStatementPrecededByBlankLinesAnalyzer() : base(Id)
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
                switch (ancestor.Kind())
                {
                    case SyntaxKind.Block:
                        return AnalyzeStatements(((BlockSyntax)ancestor).Statements, statement);

                    case SyntaxKind.SwitchSection:
                        return AnalyzeStatements(((SwitchSectionSyntax)ancestor).Statements, statement);

                    case SyntaxKind.IfStatement:
                    case SyntaxKind.ElseClause:
                        return null; // no issue as the statement is not surrounded by brackets that follow a block

                    case SyntaxKind.LocalFunctionStatement:
                        return null; // stop lookup as there is no valid ancestor anymore

                    // base methods
                    case SyntaxKind.ConversionOperatorDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.OperatorDeclaration:
                        return null; // stop lookup as there is no valid ancestor anymore

                    // base types
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.StructDeclaration:
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