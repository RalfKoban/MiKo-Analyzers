﻿using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6004";

        public MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleAssignmentExpression);
        }

        private static bool IsAssignmentOrDeclaration(AssignmentExpressionSyntax assignment, StatementSyntax statement)
        {
            if (statement.DescendantNodes().Contains(assignment))
            {
                // it's our own assignment that we check
                return true;
            }

            switch (statement)
            {
                case LocalDeclarationStatementSyntax _:
                    return true; // the statement is a variable declaration

                case ExpressionStatementSyntax e when e.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression):
                    return true; // it is another assignment expression

                default:
                    return false;
            }
        }

        private static bool IsLocalSymbol(SimpleNameSyntax syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.Identifier.GetSymbol(semanticModel);

            return symbol?.Kind == SymbolKind.Local;
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var node = (AssignmentExpressionSyntax)context.Node;
            var issue = Analyze(node, context.SemanticModel);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic Analyze(AssignmentExpressionSyntax assignment, SemanticModel semanticModel)
        {
            if (assignment.Left is IdentifierNameSyntax i)
            {
                foreach (var ancestor in assignment.Ancestors())
                {
                    switch (ancestor)
                    {
                        case BlockSyntax block:
                            return IsLocalSymbol(i, semanticModel)
                                   ? Analyze(assignment, block.Statements)
                                   : null;

                        case SwitchSectionSyntax section:
                            return IsLocalSymbol(i, semanticModel)
                                   ? Analyze(assignment, section.Statements)
                                   : null;

                        case IfStatementSyntax _:
                        case ElseClauseSyntax _:
                        case ParenthesizedLambdaExpressionSyntax _:
                        case InitializerExpressionSyntax _:
                            return null; // no issue

                        case MethodDeclarationSyntax _:
                        case ClassDeclarationSyntax _:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic Analyze(AssignmentExpressionSyntax assignment, SyntaxList<StatementSyntax> statements)
        {
            var callLineSpan = assignment.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                               .Any(_ => IsAssignmentOrDeclaration(assignment, _) is false);

            if (noBlankLinesBefore)
            {
                return Issue(assignment.Left, true, false);
            }

            return null;
        }
    }
}