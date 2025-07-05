using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6065_InvocationIsIndentedAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6065";

        public MiKo_6065_InvocationIsIndentedAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, FindIssue(context));

        private Diagnostic FindIssue(in SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation)
            {
                switch (invocation.Expression)
                {
                    case MemberAccessExpressionSyntax maes:
                        return FindIssue(maes);

                    case MemberBindingExpressionSyntax mbes:
                        return FindIssue(mbes, invocation.Parent as ConditionalAccessExpressionSyntax);
                }
            }

            return null;
        }

        private Diagnostic FindIssue(MemberAccessExpressionSyntax member)
        {
            if (member.IsKind(SyntaxKind.SimpleMemberAccessExpression) && member.Name is IdentifierNameSyntax)
            {
                var dot = member.OperatorToken;
                var expression = member.Expression;

                if (dot.GetPositionWithinStartLine() < expression.GetPositionWithinStartLine())
                {
                    switch (expression)
                    {
                        case IdentifierNameSyntax identifier:
                            return FindIssue(dot, identifier);

                        case InvocationExpressionSyntax invocation when invocation.Expression is IdentifierNameSyntax identifier:
                            return FindIssue(dot, identifier);
                    }
                }
            }

            return null;
        }

        private Diagnostic FindIssue(MemberBindingExpressionSyntax member, ConditionalAccessExpressionSyntax conditional)
        {
            var expression = conditional?.Expression;

            if (expression is null)
            {
                return null;
            }

            var token = member.OperatorToken;

            if (token.GetPositionWithinStartLine() < expression.GetPositionWithinStartLine())
            {
                switch (expression)
                {
                    case IdentifierNameSyntax identifier:
                        return FindIssue(token, identifier);

                    case InvocationExpressionSyntax invocation when invocation.Expression is IdentifierNameSyntax identifier:
                        return FindIssue(token, identifier);
                }
            }

            return null;
        }

        private Diagnostic FindIssue(in SyntaxToken dot, IdentifierNameSyntax identifier)
        {
            var location = identifier.GetLocation();
            var position = location.GetPositionWithinEndLine();

            return Issue(dot, CreateProposalForSpaces(position - Constants.Indentation));
        }
    }
}