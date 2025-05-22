﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

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

        private static LinePosition GetStartPosition(InvocationExpressionSyntax invocation)
        {
            switch (invocation.Expression)
            {
                case IdentifierNameSyntax i: return i.GetStartPosition();
                case MemberAccessExpressionSyntax m: return m.Name.GetStartPosition();
                case GenericNameSyntax g: return g.Identifier.GetStartPosition();

                default:
                    return invocation.Expression.GetStartPosition();
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            var arguments = invocation.ArgumentList.Arguments;

            if (arguments.Count is 1)
            {
                AnalyzeArgument(context, arguments[0], invocation);
            }
        }

        private void AnalyzeArgument(in SyntaxNodeAnalysisContext context, ArgumentSyntax argument, InvocationExpressionSyntax invocation)
        {
            var startPosition = GetStartPosition(invocation);
            var argumentPosition = argument.GetStartPosition();

            if (startPosition.Line != argumentPosition.Line)
            {
                ReportDiagnostics(context, Issue(argument));
            }
        }
    }
}