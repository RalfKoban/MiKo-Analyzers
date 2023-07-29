using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6037_SingleArgumentsAreOnSameLineAsInvocationAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6037";

        public MiKo_6037_SingleArgumentsAreOnSameLineAsInvocationAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count == 1)
            {
                AnalyzeArgument(context, arguments[0], invocation);
            }
        }

        private void AnalyzeArgument(SyntaxNodeAnalysisContext context, ArgumentSyntax argument, InvocationExpressionSyntax invocation)
        {
            var invocationPosition = invocation.Expression.GetStartPosition();
            var argumentPosition = argument.GetStartPosition();

            if (invocationPosition.Line != argumentPosition.Line)
            {
                ReportDiagnostics(context, Issue(argument));
            }
        }
    }
}