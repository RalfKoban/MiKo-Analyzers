﻿using System.Collections.Generic;
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
        public override string FixableDiagnosticId => "MiKo_5010";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var invocation = syntaxNodes.OfType<InvocationExpressionSyntax>().First();

            var parent = invocation.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression when parent.Parent.IsKind(SyntaxKind.LogicalNotExpression):
                    return parent.Parent;

                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.IsPatternExpression:
                    return parent;

                default:
                    return invocation;
            }
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is IsPatternExpressionSyntax pattern)
            {
                var isFalsePattern = pattern.IsPatternCheckFor(SyntaxKind.FalseLiteralExpression);

                return GetUpdatedSyntax(pattern.Expression, issue, isFalsePattern ? SyntaxKind.NotEqualsExpression : SyntaxKind.None).WithoutTrailingTrivia();
            }

            return GetUpdatedSyntax(syntax, issue, SyntaxKind.None);
        }

        private static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax, Diagnostic issue, in SyntaxKind predefined)
        {
            var invocation = GetInvocationExpressionSyntax(syntax, out var kind);

            if (invocation is null)
            {
                return syntax;
            }

            if (predefined != SyntaxKind.None)
            {
                kind = predefined;
            }

            var arguments = invocation.ArgumentList.Arguments;

            switch (arguments.Count)
            {
                case 1 when invocation.Expression is MemberAccessExpressionSyntax maes:
                {
                    var argument0 = arguments[0];

                    var left = maes.Expression;
                    var right = argument0.Expression;

                    if (right is CastExpressionSyntax cast)
                    {
                        right = cast.Expression;
                    }

                    return SyntaxFactory.BinaryExpression(kind, left, right).WithTriviaFrom(syntax);
                }

                case 2:
                {
                    var argument0 = arguments[0];
                    var argument1 = arguments[1];

                    var left = argument0.Expression;
                    var right = argument1.Expression;

                    if (issue.Properties.IsEmpty)
                    {
                        return SyntaxFactory.BinaryExpression(kind, left, right).WithTriviaFrom(syntax);
                    }

                    var operand = Invocation(SimpleMemberAccess(left, nameof(Equals)), argument1);

                    if (kind == SyntaxKind.NotEqualsExpression)
                    {
                        return IsFalsePattern(operand);
                    }

                    return operand;
                }
            }

            return syntax;
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
                        case ParenthesizedExpressionSyntax p when p.WithoutParenthesis() is InvocationExpressionSyntax pi:
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