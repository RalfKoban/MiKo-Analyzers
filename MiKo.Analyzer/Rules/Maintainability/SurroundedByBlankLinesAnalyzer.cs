﻿using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class SurroundedByBlankLinesAnalyzer : MaintainabilityAnalyzer
    {
        protected SurroundedByBlankLinesAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        protected abstract bool IsCall(ITypeSymbol type);

        private static bool HasNoBlankLines(FileLinePositionSpan callLineSpan, FileLinePositionSpan otherLineSpan)
        {
            var differenceBefore = callLineSpan.StartLinePosition.Line - otherLineSpan.EndLinePosition.Line;
            var differenceAfter = otherLineSpan.StartLinePosition.Line - callLineSpan.EndLinePosition.Line;

            return differenceBefore == 1 || differenceAfter == 1;
        }

        private bool IsCall(StatementSyntax statement, SemanticModel semanticModel) => statement is ExpressionStatementSyntax e && IsCall(e, semanticModel);

        private bool IsCall(ExpressionStatementSyntax statement, SemanticModel semanticModel) => statement.Expression is InvocationExpressionSyntax i && IsCall(i, semanticModel);

        private bool IsCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel) => invocation.Expression is MemberAccessExpressionSyntax call && IsCall(call, semanticModel);

        private bool IsCall(MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            var type = call.GetTypeSymbol(semanticModel);

            return type != null && IsCall(type);
        }

        private void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (MemberAccessExpressionSyntax)context.Node;

            var diagnostic = AnalyzeSimpleMemberAccessExpression(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            if (IsCall(call, semanticModel))
            {
                var block = call.Ancestors().OfType<BlockSyntax>().FirstOrDefault();
                if (block != null)
                {
                    var callLineSpan = call.GetLocation().GetLineSpan();

                    foreach (var statement in block.Statements)
                    {
                        // check for empty lines
                        if (HasNoBlankLines(callLineSpan, statement.GetLocation().GetLineSpan()))
                        {
                            // no empty lines between, so check for another Log call
                            if (IsCall(statement, semanticModel))
                            {
                                continue;
                            }

                            return Issue(call);
                        }
                    }
                }
            }

            return null;
        }
    }
}