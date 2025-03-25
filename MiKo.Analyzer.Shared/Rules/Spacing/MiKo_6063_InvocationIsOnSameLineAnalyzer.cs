using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_6063_InvocationIsOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6063";

        public MiKo_6063_InvocationIsOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private static bool HasIssue(MemberAccessExpressionSyntax member) => member.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                                          && member.Name is IdentifierNameSyntax name
                                                                          && member.Expression is IdentifierNameSyntax expression
                                                                          && name.GetStartingLine() != expression.GetStartingLine();

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax member)
            {
                if (HasIssue(member))
                {
                    ReportDiagnostics(context, Issue(member));
                }
            }
        }
    }
}