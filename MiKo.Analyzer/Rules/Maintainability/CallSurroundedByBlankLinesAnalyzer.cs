using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class CallSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        protected CallSurroundedByBlankLinesAnalyzer(string id) : base(id)
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
        }

        protected abstract bool IsCall(ITypeSymbol type);

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
            var issue = AnalyzeSimpleMemberAccessExpression(node, context.SemanticModel);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            if (IsCall(call, semanticModel))
            {
                foreach (var ancestor in call.Ancestors())
                {
                    switch (ancestor)
                    {
                        case BlockSyntax block:
                            return AnalyzeSimpleMemberAccessExpression(block.Statements, call, semanticModel);

                        case SwitchSectionSyntax section:
                            return AnalyzeSimpleMemberAccessExpression(section.Statements, call, semanticModel);

                        // case IfStatementSyntax _: required by MiKo_3201
                        case ElseClauseSyntax _:
                        case CaseSwitchLabelSyntax _:
                        case ParenthesizedLambdaExpressionSyntax _:
                        case MethodDeclarationSyntax _:
                        case ClassDeclarationSyntax _:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(SyntaxList<StatementSyntax> statements, MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            var callLineSpan = call.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements
                                     .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                     .Any(_ => IsCall(_, semanticModel) is false);
            var noBlankLinesAfter = statements
                                    .Where(_ => HasNoBlankLinesAfter(callLineSpan, _))
                                    .Any(_ => IsCall(_, semanticModel) is false);

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(call, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}