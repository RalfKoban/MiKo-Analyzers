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

        protected override string Title => "Replace 'Equals' by '=='";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes)
        {
            var invocation = syntaxNodes.OfType<InvocationExpressionSyntax>().First();
            if (invocation.Parent.IsKind(SyntaxKind.LogicalNotExpression))
            {
                return invocation.Parent;
            }

            if (invocation.Parent.IsKind(SyntaxKind.ParenthesizedExpression) && invocation.Parent.Parent.IsKind(SyntaxKind.LogicalNotExpression))
            {
                return invocation.Parent.Parent;
            }

            if (invocation.Parent is BinaryExpressionSyntax b
                && b.IsKind(SyntaxKind.EqualsExpression)
                && b.Right.IsKind(SyntaxKind.FalseLiteralExpression))
            {
                return invocation.Parent;
            }

            return invocation;
        }

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var invocation = GetInvocationExpressionSyntax(syntax, out var kind);
            if (invocation is null)
            {
                return null;
            }

            var arguments = invocation.ArgumentList.Arguments;
            var left = arguments[0].Expression;
            var right = arguments[1].Expression;
            return SyntaxFactory.BinaryExpression(kind, left, right);
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

                    var child = syntax.ChildNodes().First();
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

                default:
                {
                    return (InvocationExpressionSyntax)syntax;
                }
            }
        }
    }
}