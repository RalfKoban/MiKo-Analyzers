﻿using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6043";

        private const int MaxLineLength = 180;

        private static readonly Func<SyntaxNode, bool> IsLogicalExpression = IsLogicalExpressionCore;

        public MiKo_6043_LambdaExpressionBodiesAreOnSameLineAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression, SyntaxKind.SimpleLambdaExpression);

        private static bool IsLogicalExpressionCore(SyntaxNode node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.LogicalAndExpression:
                case (int)SyntaxKind.LogicalOrExpression:
                    return true;

                default:
                    return false;
            }
        }

        private static bool CanAnalyzeBody(SyntaxNode lambda)
        {
            switch (lambda)
            {
                case ParenthesizedLambdaExpressionSyntax p when p.ExpressionBody != null: return CanAnalyze(p.Body);
                case SimpleLambdaExpressionSyntax s when s.ExpressionBody != null: return CanAnalyze(s.Body);
                default:
                    return false; // nothing to analyze
            }

            bool CanAnalyze(SyntaxNode body)
            {
                switch (body)
                {
                    case AnonymousObjectCreationExpressionSyntax a: return CanAnalyzeAnonymousObjectCreationExpressionSyntax(a);
                    case ObjectCreationExpressionSyntax o: return CanAnalyzeObjectCreationExpressionSyntax(o);
                    case InvocationExpressionSyntax i: return CanAnalyzeInvocationExpressionSyntax(i);
                    case BinaryExpressionSyntax b: return CanAnalyzeBinaryExpressionSyntax(b);
                    default:
                        return true;
                }
            }

            bool CanAnalyzeAnonymousObjectCreationExpressionSyntax(AnonymousObjectCreationExpressionSyntax syntax)
            {
                if (syntax.Initializers.Count > 0)
                {
                    // initializers are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                return true;
            }

            bool CanAnalyzeObjectCreationExpressionSyntax(ObjectCreationExpressionSyntax syntax)
            {
                if (syntax.Initializer?.Expressions.Count > 0)
                {
                    // initializers are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                if (syntax.ArgumentList?.Arguments.Count > 1)
                {
                    // a lot of arguments are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                return true;
            }

            bool CanAnalyzeInvocationExpressionSyntax(InvocationExpressionSyntax syntax)
            {
                var argumentList = syntax.ArgumentList;

                if (argumentList is null)
                {
                    return true;
                }

                if (syntax.DescendantNodes<LambdaExpressionSyntax>().Any())
                {
                    // the other lambda get inspected itself, so nothing to analyze here
                    return false;
                }

                var arguments = argumentList.Arguments;

                switch (arguments.Count)
                {
                    case 0:
                        return true;

                    case 1:
                    {
                        var expression = arguments[0].Expression;

                        if (expression is ObjectCreationExpressionSyntax o && o.Initializer?.Expressions.Count > 0)
                        {
                            // initializers are allowed to span multiple lines, so nothing to analyze here
                            return false;
                        }

                        return true;
                    }

                    default:
                        return true; // TODO RKN: return false; // a lot of arguments are allowed to span multiple lines, so nothing to analyze here
                }
            }

            bool CanAnalyzeBinaryExpressionSyntax(BinaryExpressionSyntax syntax)
            {
                if (IsLogicalExpressionCore(syntax) && syntax.DescendantNodes<BinaryExpressionSyntax>().Any(IsLogicalExpression))
                {
                    // multiple binary expressions such as && or || are allowed to span multiple lines, so nothing to analyze here
                    return false;
                }

                return true;
            }
        }

        private static bool FitsOnSingleLine(SyntaxNode lambda)
        {
            var text = lambda.ToFullString().Without(Constants.WhiteSpaces);

            var completeLength = lambda.GetPositionWithinStartLine() + text.Length;

            return completeLength <= MaxLineLength;
        }

        private void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = context.Node;

            if (CanAnalyzeBody(lambda))
            {
                AnalyzeBody(context, lambda);
            }
        }

        private void AnalyzeBody(SyntaxNodeAnalysisContext context, SyntaxNode lambda)
        {
            if (lambda.IsSpanningMultipleLines() && FitsOnSingleLine(lambda))
            {
                ReportDiagnostics(context, Issue(lambda));
            }
        }
    }
}