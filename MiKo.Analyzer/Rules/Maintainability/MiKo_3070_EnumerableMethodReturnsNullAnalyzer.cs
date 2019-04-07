using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SyntaxNode = Microsoft.CodeAnalysis.SyntaxNode;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3070_EnumerableMethodReturnsNullAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3070";

        public MiKo_3070_EnumerableMethodReturnsNullAnalyzer() : base(Id)
        {
        }

        private static readonly HashSet<SyntaxKind> ImportantAncestors = new HashSet<SyntaxKind>
                                                                             {
                                                                                 SyntaxKind.VariableDeclaration,
                                                                                 SyntaxKind.Parameter,
                                                                                 SyntaxKind.IfStatement,
                                                                                 SyntaxKind.ConditionalExpression,
                                                                                 SyntaxKind.SwitchStatement,
                                                                             };


        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);

        private static bool CanBeIgnored(SyntaxNodeAnalysisContext context) => CanBeIgnored(context.GetEnclosingMethod());

        private static bool CanBeIgnored(IMethodSymbol method) => method?.ReturnType.IsEnumerable() != true;

        private static IEnumerable<ExpressionSyntax> GetCandidates(SyntaxNode node, IEnumerable<string> names)
        {
            var descendantNodes = node.DescendantNodes().ToList();
            foreach (var descendant in descendantNodes)
            {
                switch (descendant)
                {
                    case VariableDeclaratorSyntax variable:
                        {
                            if (names.Contains(variable.Identifier.ValueText) && variable.Initializer != null)
                                yield return variable.Initializer.Value;

                            break;
                        }
                    case AssignmentExpressionSyntax a:
                        {
                            if (a.Left is IdentifierNameSyntax ins && names.Contains(ins.Identifier.ValueText))
                                yield return a.Right;

                            break;
                        }
                    case ReturnStatementSyntax r:
                        {
                            var literalExpressions = r.DescendantNodes().OfType<LiteralExpressionSyntax>().ToList();
                            foreach (var literalExpression in literalExpressions)
                                yield return literalExpression;

                            break;
                        }

                    case ParameterSyntax p:
                    {
                        if (names.Contains(p.Identifier.ValueText))
                        {
                            var literalExpressions = p.DescendantNodes().OfType<LiteralExpressionSyntax>().ToList();
                            foreach (var literalExpression in literalExpressions)
                                yield return literalExpression;
                        }

                        break;
                    }
                }
            }
        }

        private static bool HasIssue(SyntaxNode node) => node.IsKind(SyntaxKind.NullLiteralExpression);

        private static IEnumerable<ExpressionSyntax> GetIssues(SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditional)
        {
            var results = new List<ExpressionSyntax>();
            GetIssues(context, conditional.WhenTrue, results);
            GetIssues(context, conditional.WhenFalse, results);

            return results;
        }

        private static void GetIssues(SyntaxNodeAnalysisContext context, ExpressionSyntax expression, List<ExpressionSyntax> results)
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

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            if (CanBeIgnored(context))
                return;

            var method = (MethodDeclarationSyntax)context.Node;

            if (method.Body != null)
            {
                AnalyzeMethodBody(context, method);
            }
            else if (method.ExpressionBody != null)
            {
                AnalyzeExpressionBody(context, method);
            }
        }

        private void AnalyzeMethodBody(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            var semanticModel = context.SemanticModel;

            var controlFlow = semanticModel.AnalyzeControlFlow(method.Body);
            var returnStatements = controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>();

            foreach (var returnStatement in returnStatements)
            {
                if (GetAndReportIssue(context, returnStatement.Expression))
                    continue;

                var dataFlow = (returnStatement.Expression is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.CoalesceExpression))
                                 ? semanticModel.AnalyzeDataFlow(b.Right)
                                 : semanticModel.AnalyzeDataFlow(returnStatement);

                var localVariableNames = dataFlow.ReadInside.Select(_ => _.Name).ToHashSet();

                var candidates = GetCandidates(method, localVariableNames).ToList();
                if (candidates.Any())
                {
                    AnalyzeAssignments(context, candidates);
                }
            }
        }

        private void AnalyzeExpressionBody(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method)
        {
            var expression = method.ExpressionBody.Expression;

            if (expression is ConditionalExpressionSyntax conditional)
            {
                AnalyzeConditional(context, conditional);
            }
            else
            {
                GetAndReportIssue(context, expression);
            }
        }

        private void AnalyzeAssignments(SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignments)
        {
            if (assignments.Any(HasIssue))
            {
                var assignmentsWithIssues = new List<ExpressionSyntax>();

                var hasIssue = false;

                foreach (var assignment in assignments)
                {
                    hasIssue = HasIssue(assignment);

                    if (hasIssue && assignment.AncestorsAndSelf().Any(_ => ImportantAncestors.Contains(_.Kind())))
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

        private void AnalyzeConditional(SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditional)
        {
            var issues = GetIssues(context, conditional);

            ReportIssues(context, issues);
        }

        private bool GetAndReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode methodBody)
        {
            var hasIssue = HasIssue(methodBody);

            if (hasIssue)
            {
                ReportIssue(context, methodBody);
            }

            return hasIssue;
        }

        private void ReportIssues(SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignmentsWithIssues)
        {
            foreach (var assignment in assignmentsWithIssues)
            {
                ReportIssue(context, assignment);
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var name = node.ToString();
            var diagnostic = ReportIssue(name, node.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }
}