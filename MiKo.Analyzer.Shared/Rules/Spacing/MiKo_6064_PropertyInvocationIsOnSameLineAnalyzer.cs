using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6064_PropertyInvocationIsOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6064";

        public MiKo_6064_PropertyInvocationIsOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleMemberAccessExpression);

        private static bool HasIssue(MemberAccessExpressionSyntax member)
        {
            if (member.Parent is InvocationExpressionSyntax)
            {
                return false;
            }

            return member.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                && member.Name is IdentifierNameSyntax name
                && member.Expression is IdentifierNameSyntax expression
                && name.IsOnSameLineAs(expression) is false;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberAccessExpressionSyntax member && HasIssue(member))
            {
                ReportDiagnostics(context, Issue(member));
            }
        }
    }
}