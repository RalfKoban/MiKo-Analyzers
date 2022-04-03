using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5010_CodeFixProvider)), Shared]
    public sealed class MiKo_5010_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_5010_EqualsAnalyzer.Id;

        protected override string Title => Resources.MiKo_5010_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var invocation = syntaxNodes.OfType<InvocationExpressionSyntax>().First();

            var parent = invocation.Parent;
            switch (parent.Kind())
            {
                case SyntaxKind.ParenthesizedExpression when parent.Parent.IsKind(SyntaxKind.LogicalNotExpression):
                    return parent.Parent;

                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return parent;

                default:
                    return invocation;
            }
        }

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var invocation = GetInvocationExpressionSyntax(syntax, out var kind);
            if (invocation is null)
            {
                return syntax;
            }

            var arguments = invocation.ArgumentList.Arguments;
            var left = arguments[0].Expression;
            var right = arguments[1].Expression;
            var expression = SyntaxFactory.BinaryExpression(kind, left, right);

            return expression.WithTrailingTriviaFrom(syntax);
        }

        private static InvocationExpressionSyntax GetInvocationExpressionSyntax(SyntaxNode syntax, out SyntaxKind kind)
        {
            kind = SyntaxKind.EqualsExpression;

            var syntaxKind = syntax.Kind();
            switch (syntaxKind)
            {
                case SyntaxKind.LogicalNotExpression:
                {
                    kind = SyntaxKind.NotEqualsExpression;

                    var child = syntax.FirstChild();
                    switch (child)
                    {
                        case ParenthesizedExpressionSyntax p when p.Expression is InvocationExpressionSyntax pi:
                            return pi;

                        case InvocationExpressionSyntax i:
                            return i;

                        default:
                            return null;
                    }
                }

                case SyntaxKind.EqualsExpression:
                {
                    var b = (BinaryExpressionSyntax)syntax;
                    if (b.Right.IsKind(SyntaxKind.FalseLiteralExpression))
                    {
                        kind = SyntaxKind.NotEqualsExpression;
                    }

                    return (InvocationExpressionSyntax)b.Left;
                }

                case SyntaxKind.NotEqualsExpression:
                {
                    var b = (BinaryExpressionSyntax)syntax;
                    if (b.Right.IsKind(SyntaxKind.TrueLiteralExpression))
                    {
                        kind = SyntaxKind.NotEqualsExpression;
                    }

                    return (InvocationExpressionSyntax)b.Left;
                }

                default:
                {
                    return (InvocationExpressionSyntax)syntax;
                }
            }
        }
    }
}