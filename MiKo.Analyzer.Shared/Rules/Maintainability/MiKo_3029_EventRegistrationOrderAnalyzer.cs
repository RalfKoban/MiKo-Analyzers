using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3029_EventRegistrationOrderAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3029";

        private static readonly string[] UnregistrationMethodNames = { "Unregister", "Deregister", "Unsubscribe", "Desubscribe" };

        public MiKo_3029_EventRegistrationOrderAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeGetAccessorDeclaration, SyntaxKind.GetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeSetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        }

        private static bool Is(in MethodKind kind, AssignmentExpressionSyntax node, SemanticModel semanticModel) => node.GetSymbol(semanticModel) is IMethodSymbol method && method.MethodKind == kind;

        private static bool IsInsideLoop(AssignmentExpressionSyntax node)
        {
            var grandParent = node?.Parent?.Parent;

            return IsInsideLoopLocal(grandParent) || IsInsideLoopLocal(grandParent?.Parent);

            bool IsInsideLoopLocal(SyntaxNode syntax)
            {
                switch (syntax)
                {
                    case ForEachStatementSyntax _:
                    case ForStatementSyntax _:
                    case WhileStatementSyntax _:
                    case DoStatementSyntax _:
                        return true;

                    default:
                        return false;
                }
            }
        }

        private void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context) => AnalyzeAssignmentExpressions(context);

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (method.IsTestMethod()
             || method.IsTestSetUpMethod()
             || method.IsTestTearDownMethod()
             || method.IsTestOneTimeSetUpMethod()
             || method.IsTestOneTimeTearDownMethod())
            {
                // ignore tests
                return;
            }

            var methodName = method.GetName();

            if (methodName.StartsWithAny(UnregistrationMethodNames, StringComparison.OrdinalIgnoreCase))
            {
                AnalyzeUnregistrationAssignmentExpressions(context);
            }
            else
            {
                AnalyzeAssignmentExpressions(context);
            }
        }

        private void AnalyzeGetAccessorDeclaration(SyntaxNodeAnalysisContext context) => AnalyzeAssignmentExpressions(context);

        private void AnalyzeSetAccessorDeclaration(SyntaxNodeAnalysisContext context) => AnalyzeAssignmentExpressions(context);

        private void AnalyzeAssignmentExpressions(in SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeAssignments(context.Node, context.SemanticModel));

        private IEnumerable<Diagnostic> AnalyzeAssignments(SyntaxNode node, SemanticModel semanticModel)
        {
            var assignments = node.DescendantNodes<AssignmentExpressionSyntax>()
                                  .Where(_ => _.IsKind(SyntaxKind.AddAssignmentExpression) || _.IsKind(SyntaxKind.SubtractAssignmentExpression))
                                  .Where(_ => _.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                                  .ToList();

            if (assignments.Count is 0)
            {
                // nothing to report
                return Array.Empty<Diagnostic>();
            }

            return AnalyzeAssignments(assignments, semanticModel);
        }

        private IEnumerable<Diagnostic> AnalyzeAssignments(List<AssignmentExpressionSyntax> assignments, SemanticModel semanticModel)
        {
            var addedLambdas = assignments.Where(_ => _.IsKind(SyntaxKind.AddAssignmentExpression) && _.Right is LambdaExpressionSyntax).ToList();
            var removedLambdas = assignments.Where(_ => _.IsKind(SyntaxKind.SubtractAssignmentExpression) && _.Right is LambdaExpressionSyntax).ToList();

            foreach (var assignment in addedLambdas)
            {
                if (Is(MethodKind.EventAdd, assignment, semanticModel))
                {
                    assignments.Remove(assignment);

                    // we found a registration for an anonymous method, so that's an issue
                    yield return Issue(assignment);
                }
            }

            foreach (var assignment in removedLambdas)
            {
                if (Is(MethodKind.EventRemove, assignment, semanticModel))
                {
                    assignments.Remove(assignment);

                    // we found a de-registration for an anonymous method, so that's an issue
                    yield return Issue(assignment);
                }
            }

            var addAssignments = assignments.OfKind(SyntaxKind.AddAssignmentExpression);
            var subtractAssignments = assignments.OfKind(SyntaxKind.SubtractAssignmentExpression);

            if (addAssignments.Count is 0 || subtractAssignments.Count is 0)
            {
                // do not report violations as we have
                // - either nothing
                // - or only '+=' or '-=' calls that are no lambdas
                yield break;
            }

            var issues = AnalyzeAssignments(addAssignments, subtractAssignments, semanticModel);

            foreach (var issue in issues)
            {
                yield return issue;
            }
        }

        private IEnumerable<Diagnostic> AnalyzeAssignments(IReadOnlyList<AssignmentExpressionSyntax> addAssignments, IReadOnlyList<AssignmentExpressionSyntax> subtractAssignments, SemanticModel semanticModel)
        {
            // seems we have both += and -= calls
            if (addAssignments.Count is 1 && addAssignments.Count == subtractAssignments.Count)
            {
                // seems we have only a single += and -= call
                // so what could go wrong here is that we use different events
                var add = addAssignments[0];
                var remove = subtractAssignments[0];

                if (add.Right is IdentifierNameSyntax added && remove.Right is IdentifierNameSyntax removed && added.GetName() != removed.GetName())
                {
                    if (Is(MethodKind.EventAdd, add, semanticModel))
                    {
                        yield return Issue(add);
                    }
                }

                if (IsInsideLoop(add) && IsInsideLoop(remove) is false)
                {
                    if (Is(MethodKind.EventAdd, add, semanticModel))
                    {
                        yield return Issue(add);
                    }
                }
            }
            else
            {
                // we have multiple or even different amounts of += and -= assignments, so check the used events and order of += or -= calls
                var subtractAssignmentsForInvestigation = subtractAssignments.ToList();

                foreach (var add in addAssignments)
                {
                    if (add.Right is IdentifierNameSyntax addedHandler && Is(MethodKind.EventAdd, add, semanticModel))
                    {
                        var addedIdentifier = addedHandler.GetName();

                        var remove = subtractAssignmentsForInvestigation.Find(_ => _.Right is IdentifierNameSyntax removedHandler && removedHandler.GetName() == addedIdentifier);

                        if (remove is null)
                        {
                            // we found an add but no remove, so that's an issue
                            yield return Issue(add);
                        }
                        else
                        {
                            if (Is(MethodKind.EventRemove, remove, semanticModel))
                            {
                                // we found one, so get rid of it as it shall not be re-used (adding more events than removing leads to those leaks that we want to find
                                subtractAssignmentsForInvestigation.Remove(remove);

                                if (IsInsideLoop(add) && IsInsideLoop(remove) is false)
                                {
                                    // we found an add inside a loop, so that's an issue
                                    yield return Issue(add);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AnalyzeUnregistrationAssignmentExpressions(in SyntaxNodeAnalysisContext context)
        {
            foreach (var add in context.Node.DescendantNodes<AssignmentExpressionSyntax>())
            {
                if (add.IsKind(SyntaxKind.AddAssignmentExpression) && Is(MethodKind.EventAdd, add, context.SemanticModel))
                {
                    ReportDiagnostics(context, Issue(add));
                }
            }
        }

        private Diagnostic Issue(AssignmentExpressionSyntax assignment) => Issue(assignment.OperatorToken);
    }
}