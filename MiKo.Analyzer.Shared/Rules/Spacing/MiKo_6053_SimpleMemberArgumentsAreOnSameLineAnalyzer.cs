using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6053";

        public MiKo_6053_SimpleMemberArgumentsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.Argument);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ArgumentSyntax argument)
            {
                ReportDiagnostics(context, AnalyzeNode(argument));
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(ArgumentSyntax argument)
        {
            if (argument.Expression is MemberAccessExpressionSyntax maes && maes.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var operatorToken = maes.OperatorToken;

                var startingLine = maes.Expression.GetStartingLine();
                var operatorLine = operatorToken.GetStartingLine();
                var nameLine = maes.Name.GetStartingLine();

                if (startingLine != operatorLine || operatorLine != nameLine)
                {
                    return new[] { Issue(argument) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}