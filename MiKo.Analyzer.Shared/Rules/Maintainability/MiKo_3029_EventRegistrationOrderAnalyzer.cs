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

            AnalyzeAssignmentExpressions(context);
        }

        private void AnalyzeGetAccessorDeclaration(SyntaxNodeAnalysisContext context) => AnalyzeAssignmentExpressions(context);

        private void AnalyzeSetAccessorDeclaration(SyntaxNodeAnalysisContext context) => AnalyzeAssignmentExpressions(context);

        private void AnalyzeAssignmentExpressions(SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeAssignments(context.Node));

        private IEnumerable<Diagnostic> AnalyzeAssignments(SyntaxNode node)
        {
            var assignments = node.DescendantNodes<AssignmentExpressionSyntax>().ToList();

            if (assignments.Count > 0)
            {
                var addAssignments = assignments.OfKind(SyntaxKind.AddAssignmentExpression);
                var subtractAssignments = assignments.OfKind(SyntaxKind.SubtractAssignmentExpression);

                foreach (var assignment in addAssignments.Concat(subtractAssignments).Where(_ => _.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression) && _.Right is ParenthesizedLambdaExpressionSyntax))
                {
                    // we found a registration for an anonymous method, so that's an issue
                    yield return Issue(assignment);
                }

                var addAssignmentsCount = addAssignments.Count;
                var subtractAssignmentsCount = subtractAssignments.Count;

                // report violations only if we have both += and -= calls
                if (addAssignmentsCount != 0 && subtractAssignmentsCount != 0)
                {
                    if (addAssignmentsCount == subtractAssignmentsCount && addAssignmentsCount == 1)
                    {
                        var addAssignment = addAssignments[0];
                        var subtractAssignment = subtractAssignments[0];

                        // TODO RKN: Check for event handler delegate type, to avoid += and -= operations on numbers
                        if (addAssignment.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                         && addAssignment.Right is IdentifierNameSyntax addedHandler
                         && subtractAssignment.Right is IdentifierNameSyntax removedHandler
                         && addedHandler.GetName() != removedHandler.GetName())
                        {
                            yield return Issue(addAssignment);
                        }
                    }
                    else
                    {
                        // we have multiple or even differents amounts of += and -= assignments, so check the used events and order of += or -= calls
                        var addAssignmentsForInvestigation = addAssignments.ToList();
                        var subtractAssignmentsForInvestigation = subtractAssignments.ToList();

                        foreach (var addAssignment in addAssignmentsForInvestigation.Where(_ => _.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
                        {
                            if (addAssignment.Right is IdentifierNameSyntax addedHandler)
                            {
                                var addedIdentifier = addedHandler.GetName();

                                // TODO RKN: Check for event handler delegate type, to avoid += and -= operations on numbers
                                var handler = subtractAssignmentsForInvestigation.Find(_ => _.Right is IdentifierNameSyntax removedHandler && removedHandler.GetName() == addedIdentifier);

                                if (handler is null)
                                {
                                    // we found an add but no remove, so that's an issue
                                    yield return Issue(addAssignment);
                                }
                                else
                                {
                                    // TODO: Check for event
                                    // we found one, so get rid of it as it shall not be re-used (adding more events than removing leads to those leaks that we want to find
                                    subtractAssignmentsForInvestigation.Remove(handler);
                                }
                            }
                        }
                    }
                }

                // TODO RKN more situations
            }
        }

        private Diagnostic Issue(AssignmentExpressionSyntax assignment) => Issue(assignment.OperatorToken.GetLocation());
    }
}