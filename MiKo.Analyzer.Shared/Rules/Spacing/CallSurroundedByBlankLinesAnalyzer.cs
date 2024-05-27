using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class CallSurroundedByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        protected CallSurroundedByBlankLinesAnalyzer(string id) : base(id)
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);

        protected abstract bool IsCall(ITypeSymbol type);

        protected virtual bool IsCall(MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            var type = call.GetTypeSymbol(semanticModel);

            return type != null && IsCall(type);
        }

        protected virtual bool IsAlsoCall(MemberAccessExpressionSyntax call, SemanticModel semanticModel) => IsCall(call, semanticModel);

        private bool IsAlsoCall(StatementSyntax statement, SemanticModel semanticModel) => statement is ExpressionStatementSyntax e && IsAlsoCall(e, semanticModel);

        private bool IsAlsoCall(ExpressionStatementSyntax statement, SemanticModel semanticModel) => statement.Expression is InvocationExpressionSyntax i && IsAlsoCall(i, semanticModel);

        private bool IsAlsoCall(InvocationExpressionSyntax invocation, SemanticModel semanticModel) => invocation.Expression is MemberAccessExpressionSyntax call && IsAlsoCall(call, semanticModel);

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

                        case IfStatementSyntax statement when statement == call.Parent:
                            continue; // let's look into the surrounding block because the call is the condition of the 'if' statement

                        case IfStatementSyntax _:
                        case ElseClauseSyntax _:
                        case CaseSwitchLabelSyntax _:
                        case LambdaExpressionSyntax _:
                        case ArrowExpressionClauseSyntax _:
                        case InitializerExpressionSyntax _:
                        case LocalFunctionStatementSyntax _:
                        case BaseMethodDeclarationSyntax _:
                        case BaseTypeDeclarationSyntax _:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeSimpleMemberAccessExpression(SyntaxList<StatementSyntax> statements, MemberAccessExpressionSyntax call, SemanticModel semanticModel)
        {
            var callLineSpan = call.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                               .Any(_ => IsAlsoCall(_, semanticModel) is false);

            var noBlankLinesAfter = statements.Where(_ => HasNoBlankLinesAfter(callLineSpan, _))
                                              .Any(_ => IsAlsoCall(_, semanticModel) is false);

            if (noBlankLinesBefore || noBlankLinesAfter)
            {
                return Issue(call, noBlankLinesBefore, noBlankLinesAfter);
            }

            return null;
        }
    }
}