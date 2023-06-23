using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3079_DoNotUseIntegerForHResultAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3079";

        public MiKo_3079_DoNotUseIntegerForHResultAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNumericLiteralExpression, SyntaxKind.NumericLiteralExpression);

        private void AnalyzeNumericLiteralExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (LiteralExpressionSyntax)context.Node;

            if (node.Parent is PrefixUnaryExpressionSyntax parent && parent.IsKind(SyntaxKind.UnaryMinusExpression))
            {
                var number = node.Token.ValueText;

                if (number.Length == 10 && number.StartsWith("2147", StringComparison.OrdinalIgnoreCase) && int.TryParse(number, out var result))
                {
                    var hexValue = ((-1) * result).ToString("X");

                    ReportDiagnostics(context, Issue(string.Empty, parent, hexValue));
                }
            }
        }
    }
}