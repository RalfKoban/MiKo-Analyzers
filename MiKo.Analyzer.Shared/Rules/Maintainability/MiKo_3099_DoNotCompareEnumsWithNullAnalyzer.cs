using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3099_DoNotCompareEnumsWithNullAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3099";

        private static readonly SyntaxKind[] EqualityOperators = { SyntaxKind.EqualsEqualsToken, SyntaxKind.ExclamationEqualsToken };

        public MiKo_3099_DoNotCompareEnumsWithNullAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.NullLiteralExpression);

        private static bool IsEnum(ExpressionSyntax value, SyntaxNodeAnalysisContext context)
        {
            var type = value.GetTypeSymbol(context.SemanticModel);

            return type.IsEnum();
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LiteralExpressionSyntax node)
            {
                var issues = Analyze(node, context);

                ReportDiagnostics(context, issues);
            }
        }

        private IEnumerable<Diagnostic> Analyze(LiteralExpressionSyntax node, SyntaxNodeAnalysisContext context)
        {
            var parent = node.Parent;

            switch (parent)
            {
                case BinaryExpressionSyntax binary when binary.OperatorToken.IsAnyKind(EqualityOperators) && IsEnum(binary.Left, context):
                {
                    yield return Issue(binary);

                    break;
                }

                case ConstantPatternSyntax pattern:
                {
                    switch (pattern.Parent)
                    {
                        case IsPatternExpressionSyntax isPattern when IsEnum(isPattern.Expression, context):
                            yield return Issue(isPattern);

                            break;

                        case UnaryPatternSyntax unary when unary.Parent is IsPatternExpressionSyntax isPattern && IsEnum(isPattern.Expression, context):
                            yield return Issue(isPattern);

                            break;
                    }

                    break;
                }
            }
        }
    }
}