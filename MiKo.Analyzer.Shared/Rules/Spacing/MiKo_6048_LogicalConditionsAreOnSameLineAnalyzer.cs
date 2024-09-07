using System.Collections.Generic;
using System.Linq;

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

        private static readonly SyntaxKind[] LogicalExpressions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        public MiKo_6048_LogicalConditionsAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, LogicalExpressions);

        private static bool IsOnSingleLine(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                {
                    if (IsOnSingleLine(parenthesized.Expression))
                    {
                        // we have a parenthesized one, so let's check also the parenthesis
                        var openParenthesisSpan = parenthesized.OpenParenToken.GetLocation().GetLineSpan();
                        var closeParenthesisSpan = parenthesized.CloseParenToken.GetLocation().GetLineSpan();

                        if (openParenthesisSpan.StartLinePosition.Line == closeParenthesisSpan.EndLinePosition.Line)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                case BinaryExpressionSyntax binary:
                {
                    if (IsOnSingleLineLocal(binary))
                    {
                        return true;
                    }

                    var leftCondition = binary.Left;
                    var rightCondition = binary.Right;

                    var leftSpan = leftCondition.GetLocation().GetLineSpan();
                    var rightSpan = rightCondition.GetLocation().GetLineSpan();

                    // let's see if both conditions are on same line
                    if (leftSpan.EndLinePosition.Line == rightSpan.StartLinePosition.Line)
                    {
                        if (leftSpan.StartLinePosition.Line == rightSpan.EndLinePosition.Line)
                        {
                            // both are on same line
                            return true;
                        }

                        // at least one condition spans multiple lines
                        return false;
                    }

                    // they span different lines
                    return IsOnSingleLine(leftCondition) && IsOnSingleLine(rightCondition);
                }
            }

            return IsOnSingleLineLocal(syntax);

            bool IsOnSingleLineLocal(SyntaxNode s)
            {
                var span = s.GetLocation().GetLineSpan();

                return span.StartLinePosition.Line == span.EndLinePosition.Line;
            }
        }

        private static bool ShallAnalyzeNode(BinaryExpressionSyntax syntax)
        {
            switch (syntax.Parent)
            {
                case BinaryExpressionSyntax parent when parent.IsAnyKind(LogicalExpressions):
                case ParenthesizedExpressionSyntax parenthesized when parenthesized.Parent.IsAnyKind(LogicalExpressions):
                    return false; // ignore as the parent gets investigated already

                default:
                    return true;
            }
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is BinaryExpressionSyntax syntax)
            {
                if (ShallAnalyzeNode(syntax))
                {
                    var issues = AnalyzeNode(syntax);

                    ReportDiagnostics(context, issues);
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeNode(BinaryExpressionSyntax syntax) => IsOnSingleLine(syntax)
                                                                                      ? Enumerable.Empty<Diagnostic>()
                                                                                      : new[] { Issue(syntax) };
    }
}