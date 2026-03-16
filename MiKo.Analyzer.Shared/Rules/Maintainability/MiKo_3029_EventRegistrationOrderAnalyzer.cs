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

        private void AnalyzeAssignmentExpressions(in SyntaxNodeAnalysisContext context) => ReportDiagnostics(context, AnalyzeAssignments(context.Node, context.SemanticModel));

        private IEnumerable<Diagnostic> AnalyzeAssignments(SyntaxNode node, SemanticModel semanticModel)
        {
            var assignments = node.DescendantNodes<AssignmentExpressionSyntax>().Where(_ => _.Left.IsKind(SyntaxKind.SimpleMemberAccessExpression)).ToList();

            if (assignments.Count is 0)
            {
                // nothing to report
                yield break;
            }

            var addAssignments = assignments.OfKind(SyntaxKind.AddAssignmentExpression);
            var subtractAssignments = assignments.OfKind(SyntaxKind.SubtractAssignmentExpression);

            if (addAssignments.Count is 0 && subtractAssignments.Count is 0)
            {
                // nothing to report
                yield break;
            }

            foreach (var assignment in addAssignments.Concat(subtractAssignments).Where(_ => _.Right is ParenthesizedLambdaExpressionSyntax))
            {
                // we found a registration for an anonymous method, so that's an issue
                yield return Issue(assignment);
            }

            if (addAssignments.Count is 0 || subtractAssignments.Count is 0)
            {
                // do not report violations as we have only += or -= calls
                yield break;
            }

            // TODO RKN: Check for add and subtract assignments whether they are part of a loop (if so, report them)

            // report violations only if we have both += and -= calls
            if (addAssignments.Count is 1 && addAssignments.Count == subtractAssignments.Count)
            {
                // seems we have only a single += and -= call
                // so what could go wrong here is that we use different events
                var add = addAssignments[0];
                var subtract = subtractAssignments[0];

                // TODO RKN: Check for event handler delegate type, to avoid += and -= operations on numbers
                if (add.Right is IdentifierNameSyntax added && subtract.Right is IdentifierNameSyntax removed && added.GetName() != removed.GetName())
                {
                    yield return Issue(add);
                }
            }
            else
            {
                // we have multiple or even different amounts of += and -= assignments, so check the used events and order of += or -= calls
                var subtractAssignmentsForInvestigation = subtractAssignments.ToList();

                foreach (var addAssignment in addAssignments)
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

            // TODO RKN more situations
        }

        private Diagnostic Issue(AssignmentExpressionSyntax assignment) => Issue(assignment.OperatorToken);
    }
}