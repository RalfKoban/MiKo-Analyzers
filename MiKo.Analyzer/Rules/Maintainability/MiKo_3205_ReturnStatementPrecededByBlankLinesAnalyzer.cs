﻿using System.Linq;

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

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeReturnStatementSyntax, SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement);
        }

        private void AnalyzeReturnStatementSyntax(SyntaxNodeAnalysisContext context)
        {
            var diagnostic = AnalyzeReturnStatementSyntax(context.Node);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
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
            var noBlankLinesBefore = statements.Any(_ => HasNoBlankLinesBefore(callLineSpan, _));

            return noBlankLinesBefore ? Issue(returnStatement, true, false) : null;
        }
    }
}