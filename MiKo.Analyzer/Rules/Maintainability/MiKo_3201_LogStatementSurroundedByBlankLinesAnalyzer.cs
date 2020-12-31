using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3201";

        public MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static bool HasNoBlankLines(FileLinePositionSpan callLineSpan, FileLinePositionSpan otherLineSpan)
        {
            var differenceBefore = callLineSpan.StartLinePosition.Line - otherLineSpan.EndLinePosition.Line;
            var differenceAfter = otherLineSpan.StartLinePosition.Line - callLineSpan.EndLinePosition.Line;

            return differenceBefore == 1 || differenceAfter == 1;
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

        private bool IsCall(StatementSyntax statement, SemanticModel semanticModel) => statement is ExpressionStatementSyntax e && IsCall(e, semanticModel);

        private bool IsCall(ExpressionStatementSyntax statement, SemanticModel semanticModel) => statement.Expression is InvocationExpressionSyntax i && IsCall(i, semanticModel);

        private bool IsCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel) => invocation.Expression is MemberAccessExpressionSyntax call && IsCall(call, semanticModel);

        private bool IsCall(MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            var type = call.GetTypeSymbol(semanticModel);

            return IsCall(type);
        }

        private bool IsCall(ITypeSymbol type) => type.Name == Constants.ILog.TypeName;
    }
}