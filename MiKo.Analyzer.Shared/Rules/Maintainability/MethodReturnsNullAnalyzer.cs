﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MethodReturnsNullAnalyzer : MaintainabilityAnalyzer
    {
        private static readonly ISet<SyntaxKind> ImportantAncestors = new HashSet<SyntaxKind>
                                                                          {
                                                                              SyntaxKind.VariableDeclaration,
                                                                              SyntaxKind.Parameter,
                                                                              SyntaxKind.IfStatement,
                                                                              SyntaxKind.ConditionalExpression,
                                                                              SyntaxKind.SwitchStatement,
                                                                          };

        protected MethodReturnsNullAnalyzer(string diagnosticId) : base(diagnosticId)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);

        private static IEnumerable<ExpressionSyntax> GetCandidates(SyntaxNode node, ICollection<string> names)
        {
            var descendantNodes = node.DescendantNodes();

            return descendantNodes.SelectMany(_ => GetSpecificCandidates(_, names));
        }

        private static IEnumerable<ExpressionSyntax> GetSpecificCandidates(SyntaxNode descendant, ICollection<string> names)
        {
            switch (descendant)
            {
                case VariableDeclaratorSyntax variable:
                {
                    var initializer = variable.Initializer;

                    if (initializer != null && names.Contains(variable.GetName()))
                    {
                        return new[] { initializer.Value };
                    }

                    break;
                }

                case AssignmentExpressionSyntax a:
                {
                    if (a.Left is IdentifierNameSyntax ins && names.Contains(ins.GetName()))
                    {
                        return new[] { a.Right };
                    }

                    break;
                }

                case ParameterSyntax p:
                {
                    if (names.Contains(p.GetName()))
                    {
                        return p.DescendantNodes<LiteralExpressionSyntax>();
                    }

                    break;
                }
            }

            return Array.Empty<ExpressionSyntax>();
        }

        private static bool HasIssue(SyntaxNode node) => node.IsKind(SyntaxKind.NullLiteralExpression) && ParentWithoutIssue(node.Parent) is false;

        private static bool ParentWithoutIssue(SyntaxNode node)
        {
            while (true)
            {
                // check for comparisons
                switch (node?.Kind())
                {
                    case null:
                        return false;

                    case SyntaxKind.EqualsExpression:
                    case SyntaxKind.NotEqualsExpression:
                    case SyntaxKind.ConstantPattern:
                    case SyntaxKind.Argument:
                        return true;

                    case SyntaxKind.CastExpression:
                        node = node.Parent;

                        continue;

                    default:
                        return false;
                }
            }
        }

        private static List<ExpressionSyntax> GetIssues(in SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditional)
        {
            var results = new List<ExpressionSyntax>();
            GetIssues(context, conditional.WhenTrue, results);
            GetIssues(context, conditional.WhenFalse, results);

            return results;
        }

        private static void GetIssues(in SyntaxNodeAnalysisContext context, ExpressionSyntax expression, List<ExpressionSyntax> results)
        {
            if (expression is ConditionalExpressionSyntax nested)
            {
                var nestedIssues = GetIssues(context, nested);

                results.AddRange(nestedIssues);
            }
            else
            {
                if (HasIssue(expression))
                {
                    results.Add(expression);
                }
            }
        }

        private bool CanBeIgnored(SyntaxNodeAnalysisContext context)
        {
            if (context.CancellationToken.IsCancellationRequested)
            {
                return true;
            }

            return ShallAnalyze(context.GetEnclosingMethod()) is false;
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            if (CanBeIgnored(context))
            {
                return;
            }

            var method = (MethodDeclarationSyntax)context.Node;

            var body = method.Body;

            if (body != null)
            {
                AnalyzeMethodBody(context, method, body);

                return;
            }

            var expressionBody = method.ExpressionBody;

            if (expressionBody != null)
            {
                AnalyzeMethodExpressionBody(context, method, expressionBody.Expression);
            }
        }

        private void AnalyzeMethodBody(in SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, BlockSyntax methodBody)
        {
            var controlFlow = context.SemanticModel.AnalyzeControlFlow(methodBody);

            foreach (var returnStatement in controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>())
            {
                AnalyzeExpression(context, method, returnStatement.Expression);
            }
        }

        private void AnalyzeMethodExpressionBody(in SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ConditionalExpressionSyntax conditional:
                    AnalyzeConditional(context, conditional);

                    break;

                case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.CoalesceExpression):
                    AnalyzeExpression(context, method, b.Right);

                    break;

                default:
                    if (HasIssue(expression))
                    {
                        ReportIssue(context, expression);
                    }

                    break;
            }
        }

        private void AnalyzeExpression(in SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, ExpressionSyntax expression)
        {
            if (HasIssue(expression))
            {
                ReportIssue(context, expression);
            }
            else
            {
                var exp = expression is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.CoalesceExpression) ? b.Right : expression;
                var dataFlow = context.SemanticModel.AnalyzeDataFlow(exp);

                var localVariableNames = dataFlow.ReadInside.ToHashSet(_ => _.Name);

                var candidates = GetCandidates(method, localVariableNames).ToHashSet();

                if (candidates.Count != 0)
                {
                    AnalyzeAssignments(context, candidates);
                }
                else
                {
                    // might be no candidates with variables, so check for ternary operators
                    if (exp is ConditionalExpressionSyntax conditional)
                    {
                        AnalyzeConditional(context, conditional);
                    }
                }
            }
        }

        private void AnalyzeAssignments(in SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignments)
        {
            if (assignments.Any(HasIssue))
            {
                var assignmentsWithIssues = new List<ExpressionSyntax>();

                var hasIssue = false;

                foreach (var assignment in assignments)
                {
                    hasIssue = HasIssue(assignment);

                    if (hasIssue && assignment.Ancestors().Any(_ => _.IsAnyKind(ImportantAncestors)))
                    {
                        assignmentsWithIssues.Add(assignment);
                    }
                }

                if (hasIssue)
                {
                    ReportIssues(context, assignmentsWithIssues);
                }
            }
            else
            {
                foreach (var conditional in assignments.OfType<ConditionalExpressionSyntax>())
                {
                    AnalyzeConditional(context, conditional);
                }
            }
        }

        private void AnalyzeConditional(in SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditional)
        {
            var issues = GetIssues(context, conditional);

            ReportIssues(context, issues);
        }

        private void ReportIssues(in SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignmentsWithIssues)
        {
            foreach (var assignment in assignmentsWithIssues)
            {
                ReportIssue(context, assignment);
            }
        }

        private void ReportIssue(in SyntaxNodeAnalysisContext context, SyntaxNode node) => ReportDiagnostics(context, Issue(node));
    }
}