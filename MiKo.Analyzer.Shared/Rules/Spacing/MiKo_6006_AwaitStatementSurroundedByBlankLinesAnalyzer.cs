﻿using System.Collections.Generic;
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

        private static bool HasNonAwaitedStatement(IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case ExpressionStatementSyntax e when e.Expression.IsKind(SyntaxKind.AwaitExpression):
                    case LocalDeclarationStatementSyntax l when l.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                        continue;

                    default:
                        return true;
                }
            }

            return false;
        }

        private static bool HasNonAwaitedLocalAssignment(IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case LocalDeclarationStatementSyntax l1 when l1.AwaitKeyword.IsKind(SyntaxKind.AwaitKeyword):
                    case LocalDeclarationStatementSyntax l2 when l2.Declaration.DescendantNodes().Any(_ => _.IsKind(SyntaxKind.AwaitExpression)):
                        continue;

                    case ExpressionStatementSyntax e when e.Expression is AssignmentExpressionSyntax assignment && assignment.Right is AwaitExpressionSyntax:
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

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeAwaitExpression(AwaitExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.Block:
                        return AnalyzeAwaitExpression(((BlockSyntax)ancestor).Statements, node);

                    case SyntaxKind.SwitchSection:
                        return AnalyzeAwaitExpression(((SwitchSectionSyntax)ancestor).Statements, node);

                    // lambdas
                    case SyntaxKind.SimpleLambdaExpression:
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        return null; // stop lookup if it is a parameter

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

        private Diagnostic AnalyzeAwaitExpression(in SyntaxList<StatementSyntax> statements, AwaitExpressionSyntax node)
        {
            switch (node.Parent)
            {
                case StatementSyntax _:
                {
                    var callLineSpan = node.GetLineSpan();

                    var noBlankLinesBefore = HasNonAwaitedStatement(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
                    var noBlankLinesAfter = HasNonAwaitedStatement(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

                    if (noBlankLinesBefore || noBlankLinesAfter)
                    {
                        return Issue(node.AwaitKeyword, noBlankLinesBefore, noBlankLinesAfter);
                    }

                    break;
                }

                case EqualsValueClauseSyntax _:
                case AssignmentExpressionSyntax _:
                {
                    var callLineSpan = node.GetLineSpan();

                    var noBlankLinesBefore = HasNonAwaitedLocalAssignment(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
                    var noBlankLinesAfter = HasNonAwaitedLocalAssignment(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

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