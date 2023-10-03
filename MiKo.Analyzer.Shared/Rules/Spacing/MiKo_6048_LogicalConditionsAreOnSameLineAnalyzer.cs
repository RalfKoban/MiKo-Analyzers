using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6048";

        private static readonly SyntaxKind[] Expressions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        public MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private static bool IsOnSingleLine(SyntaxNode syntax)
        {
            while (true)
            {
                if (syntax is BinaryExpressionSyntax)
                {
                    return true; // ignore combined binaries
                }

                if (syntax is ParenthesizedExpressionSyntax parenthesized)
                {
                    syntax = parenthesized.Expression;

                    continue;
                }

                var span = syntax.GetLocation().GetLineSpan();

                return span.StartLinePosition.Line == span.EndLinePosition.Line;
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax syntax)
            {
                ReportDiagnostics(context, AnalyzeNode(syntax));
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(BinaryExpressionSyntax syntax)
        {
            if (IsOnSingleLine(syntax.Left) is false)
            {
                yield return Issue(syntax.Left);
            }

            if (IsOnSingleLine(syntax.Right) is false)
            {
                yield return Issue(syntax.Right);
            }
        }
    }
}