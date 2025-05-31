using System;

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

        private static readonly SyntaxKind[] EqualityExpressions = { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression };

        public MiKo_3099_DoNotCompareEnumsWithNullAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.NullLiteralExpression);

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is LiteralExpressionSyntax node)
            {
                var issues = Analyze(node, context);

                ReportDiagnostics(context, issues);
            }
        }

        private Diagnostic[] Analyze(LiteralExpressionSyntax node, in SyntaxNodeAnalysisContext context)
        {
            var parent = node.Parent;

            switch (parent)
            {
                case BinaryExpressionSyntax binary when binary.IsAnyKind(EqualityExpressions) && binary.Left.IsEnum(context.SemanticModel):
                {
                    return new[] { Issue(binary) };
                }

                case ConstantPatternSyntax pattern:
                {
                    switch (pattern.Parent)
                    {
                        case IsPatternExpressionSyntax isPattern when isPattern.IsEnum(context.SemanticModel):
                            return new[] { Issue(isPattern) };

                        case UnaryPatternSyntax unary when unary.Parent is IsPatternExpressionSyntax isPattern && isPattern.IsEnum(context.SemanticModel):
                            return new[] { Issue(isPattern) };
                    }

                    break;
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}