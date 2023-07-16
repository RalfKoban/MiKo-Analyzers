using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6035";

        public MiKo_6035_OpenParenthesisAreOnSameLineAsInvocationAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            switch (invocation.Expression)
            {
                case MemberAccessExpressionSyntax maes:
                    AnalyzeNameSyntax(context, maes.Name, invocation.ArgumentList.OpenParenToken);

                    break;

                case NameSyntax name:
                    AnalyzeNameSyntax(context, name, invocation.ArgumentList.OpenParenToken);

                    break;
            }
        }

        private void AnalyzeNameSyntax(SyntaxNodeAnalysisContext context, NameSyntax name, SyntaxToken parenthesis)
        {
            var parenthesisPosition = parenthesis.GetStartPosition();
            var memberPosition = name.GetStartPosition();

            if (parenthesisPosition.Line != memberPosition.Line)
            {
                ReportDiagnostics(context, Issue(parenthesis));
            }
        }
    }
}