using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc/>
    /// <seealso cref="MiKo_3107_OnlyMocksUseConditionMatchersAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3120_UseValueInsteadOfItIsConditionMatcherAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3120";

        public MiKo_3120_UseValueInsteadOfItIsConditionMatcherAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.Moq.MockFullQualified) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            base.InitializeCore(context);

            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax node)
            {
                var arguments = node.ArgumentList.Arguments;

                if (arguments.Count == 1)
                {
                    var issues = AnalyzeSimpleMemberAccessExpression(node, arguments[0], context);

                    ReportDiagnostics(context, issues);
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeSimpleMemberAccessExpression(InvocationExpressionSyntax node, ArgumentSyntax argument, SyntaxNodeAnalysisContext context)
        {
            if (node.IsMoqItIsConditionMatcher() && argument.Expression is LambdaExpressionSyntax lambda)
            {
                switch (lambda.ExpressionBody)
                {
                    case BinaryExpressionSyntax binary when binary.OperatorToken.IsKind(SyntaxKind.EqualsEqualsToken) && binary.Left is IdentifierNameSyntax:
                    {
                        switch (binary.Right)
                        {
                            case LiteralExpressionSyntax _:
                            case IdentifierNameSyntax identifier when identifier.IsConst(context):
                                return new[] { Issue(node) };
                        }

                        break;
                    }

                    case IsPatternExpressionSyntax pattern when pattern.Expression is IdentifierNameSyntax && pattern.Pattern is ConstantPatternSyntax c && c.Expression is LiteralExpressionSyntax:
                    {
                        return new[] { Issue(node) };
                    }
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
