﻿using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3206_AwaitStatementSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3206";

        public MiKo_3206_AwaitStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
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

        private void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (AwaitExpressionSyntax)context.Node;

            var diagnostic = AnalyzeAwaitExpression(node);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeAwaitExpression(AwaitExpressionSyntax node)
        {
            foreach (var ancestor in node.Ancestors())
            {
                switch (ancestor)
                {
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

        private Diagnostic AnalyzeAwaitExpression(SyntaxList<StatementSyntax> statements, CSharpSyntaxNode node)
        {
            var callLineSpan = node.GetLocation().GetLineSpan();

            var noBlankLinesBefore = HasNonAwaitedExpression(statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _)));
            var noBlankLinesAfter = HasNonAwaitedExpression(statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _)));

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(node, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}