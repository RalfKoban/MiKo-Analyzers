using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class SurroundedByBlankLinesAnalyzer : MaintainabilityAnalyzer
    {
        internal const string NoLineBefore = "before";
        internal const string NoLineAfter = "after";

        protected SurroundedByBlankLinesAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        protected abstract bool IsCall(ITypeSymbol type);

        private static bool HasNoBlankLinesBefore(FileLinePositionSpan callLineSpan, StatementSyntax other)
        {
            var otherLineSpan = other.GetLocation().GetLineSpan();

            var differenceBefore = callLineSpan.StartLinePosition.Line - otherLineSpan.EndLinePosition.Line;

            return differenceBefore == 1;
        }

        private static bool HasNoBlankLinesAfter(FileLinePositionSpan callLineSpan, StatementSyntax other)
        {
            var otherLineSpan = other.GetLocation().GetLineSpan();

            var differenceAfter = otherLineSpan.StartLinePosition.Line - callLineSpan.EndLinePosition.Line;

            return differenceAfter == 1;
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

                    var noBlankLinesBefore = block.Statements
                                                  .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                                  .Any(_ => IsCall(_, semanticModel) is false);
                    var noBlankLinesAfter = block.Statements
                                                 .Where(_ => HasNoBlankLinesAfter(callLineSpan, _))
                                                 .Any(_ => IsCall(_, semanticModel) is false);

                    if (noBlankLinesBefore || noBlankLinesAfter)
                    {
                        return Issue(call, noBlankLinesBefore, noBlankLinesAfter);
                    }
                }
            }

            return null;
        }

        private Diagnostic Issue(SyntaxNode call, bool noBlankLinesBefore, bool noBlankLinesAfter)
        {
            // prepare additional data so that code fix can benefit from information
            var dictionary = new Dictionary<string, string>();
            if (noBlankLinesBefore)
            {
                dictionary.Add(NoLineBefore, string.Empty);
            }

            if (noBlankLinesAfter)
            {
                dictionary.Add(NoLineAfter, string.Empty);
            }

            return Issue(call, dictionary);
        }
    }
}