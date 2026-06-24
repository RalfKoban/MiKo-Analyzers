using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6069_ObjectInitializerExpressionsAreIndentedBesideOpenBraceAnalyzer : SpacingAnalyzer
    {
        public const string Id = "MiKo_6069";

        private const int AcceptedSpacesOnSameLine = 1;

        private static readonly SyntaxKind[] Expressions = { SyntaxKind.ObjectInitializerExpression };

        public MiKo_6069_ObjectInitializerExpressionsAreIndentedBesideOpenBraceAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, Expressions);

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InitializerExpressionSyntax initializer)
            {
                var issues = AnalyzeNode(initializer);

                if (issues.Count > 0)
                {
                    ReportDiagnostics(context, issues);
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeNode(InitializerExpressionSyntax initializer)
        {
            List<Diagnostic> issues = null;

            var expressions = initializer.Expressions;
            var count = expressions.Count;

            if (count > 0)
            {
                var openBraceToken = initializer.OpenBraceToken;
                var openBracePosition = openBraceToken.GetStartPosition();
                var onSameLine = openBraceToken.IsOnSameLineAs(initializer.CloseBraceToken);

                for (var index = 0; index < count; index++)
                {
                    var expression = expressions[index];
                    var spaces = 0;

                    if (onSameLine)
                    {
                        var token = index is 0
                                    ? openBraceToken
                                    : expressions.GetSeparator(index - 1); // calculate for consecutive expressions

                        var trailingTrivia = token.TrailingTrivia;

                        if (trailingTrivia.Count is 0)
                        {
                            if (expression.GetPositionWithinStartLine() != token.GetPositionWithinEndLine() + AcceptedSpacesOnSameLine)
                            {
                                spaces = AcceptedSpacesOnSameLine;
                            }
                        }
                        else
                        {
                            if (trailingTrivia.Any(_ => _.Span.Length != AcceptedSpacesOnSameLine && _.IsWhiteSpace()))
                            {
                                spaces = AcceptedSpacesOnSameLine;
                            }
                        }
                    }
                    else
                    {
                        var expressionPosition = expression.GetStartPosition();

                        if (expressionPosition.Line > openBracePosition.Line)
                        {
                            var expectedPosition = openBracePosition.Character + Constants.Indentation;

                            if (expressionPosition.Character != expectedPosition)
                            {
                                spaces = expectedPosition;
                            }
                        }
                    }

                    if (spaces > 0)
                    {
                        if (issues is null)
                        {
                            issues = new List<Diagnostic>(count);
                        }

                        issues.Add(Issue(expression, CreateProposalForSpaces(spaces)));
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)issues ?? Array.Empty<Diagnostic>();
        }
    }
}