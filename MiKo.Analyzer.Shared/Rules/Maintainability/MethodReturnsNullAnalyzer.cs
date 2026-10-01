using System;
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

        private static bool IsOverwritten(ExpressionSyntax assignedExpression)
        {
            string name;
            SyntaxNode containingStatement;

            switch (assignedExpression.Parent)
            {
                case EqualsValueClauseSyntax e when e.Parent is VariableDeclaratorSyntax declarator:
                {
                    name = declarator.GetName();
                    containingStatement = declarator.Parent?.Parent;

                    break;
                }

                case AssignmentExpressionSyntax a when a.Left is IdentifierNameSyntax identifier:
                {
                    name = identifier.GetName();
                    containingStatement = a.Parent;

                    break;
                }

                default:
                    return false;
            }

            var current = GetStatementInBlock(containingStatement);

            // walk outwards through all enclosing blocks, as the overwrite may happen in any of them
            while (current != null)
            {
                // a return inside the enclosing statement might return the null value before it gets overwritten
                if (current.DescendantNodesAndSelf<ReturnStatementSyntax>().Any())
                {
                    return false;
                }

                var block = (BlockSyntax)current.Parent;
                var statements = block.Statements;

                for (var i = statements.IndexOf(current) + 1; i < statements.Count; i++)
                {
                    var statement = statements[i];

                    if (statement is ExpressionStatementSyntax s
                     && s.Expression is AssignmentExpressionSyntax later
                     && later.IsKind(SyntaxKind.SimpleAssignmentExpression)
                     && later.Left is IdentifierNameSyntax left
                     && left.GetName() == name
                     && later.Right.IsKind(SyntaxKind.NullLiteralExpression) is false)
                    {
                        return true;
                    }

                    // the null value may be returned before it gets overwritten
                    if (statement.DescendantNodesAndSelf<ReturnStatementSyntax>().Any())
                    {
                        return false;
                    }
                }

                current = GetStatementInBlock(block.Parent);
            }

            return false;
        }

        private static StatementSyntax GetStatementInBlock(SyntaxNode node)
        {
            while (node != null)
            {
                if (node is StatementSyntax statement && statement.Parent is BlockSyntax)
                {
                    return statement;
                }

                switch (node)
                {
                    case LambdaExpressionSyntax _:
                    case AnonymousMethodExpressionSyntax _:
                    case MemberDeclarationSyntax _:
                        return null;
                }

                node = node.Parent;
            }

            return null;
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

        private bool CanBeIgnored(in SyntaxNodeAnalysisContext context)
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
                AnalyzeMethodBody(context, method, body, new HashSet<SyntaxNode>());

                return;
            }

            var expressionBody = method.ExpressionBody;

            if (expressionBody != null)
            {
                AnalyzeMethodExpressionBody(context, method, expressionBody.Expression, new HashSet<SyntaxNode>());
            }
        }

        private void AnalyzeMethodBody(in SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, BlockSyntax methodBody, ISet<SyntaxNode> reported)
        {
            var controlFlow = context.SemanticModel.AnalyzeControlFlow(methodBody);

            foreach (var returnStatement in controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>())
            {
                AnalyzeExpression(context, method, returnStatement.Expression, reported);
            }
        }

        private void AnalyzeMethodExpressionBody(in SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, ExpressionSyntax expression, ISet<SyntaxNode> reported)
        {
            switch (expression)
            {
                case IdentifierNameSyntax identifier when identifier.Identifier.IsMissing:
                {
                    // code seems to be incomplete, so ignore that
                    break;
                }

                case ConditionalExpressionSyntax conditional:
                {
                    AnalyzeConditional(context, conditional, reported);

                    break;
                }

                case BinaryExpressionSyntax b when b.IsKind(SyntaxKind.CoalesceExpression):
                {
                    AnalyzeExpression(context, method, b.Right, reported);

                    break;
                }

                default:
                {
                    if (HasIssue(expression))
                    {
                        ReportIssue(context, expression, reported);
                    }

                    break;
                }
            }
        }

        private void AnalyzeExpression(in SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, ExpressionSyntax returnedExpression, ISet<SyntaxNode> reported)
        {
            switch (returnedExpression)
            {
                case null: // code seems to be incomplete, so ignore that
                case ArrayCreationExpressionSyntax _:
                case ImplicitArrayCreationExpressionSyntax _:
                case ObjectCreationExpressionSyntax _:
                case ImplicitObjectCreationExpressionSyntax _:
                    return;
            }

            if (returnedExpression is BinaryExpressionSyntax coalesce && coalesce.IsKind(SyntaxKind.CoalesceExpression))
            {
                AnalyzeExpression(context, method, coalesce.Right, reported);

                return;
            }

            if (HasIssue(returnedExpression))
            {
                ReportIssue(context, returnedExpression, reported);

                return;
            }

            if (returnedExpression is IdentifierNameSyntax identifier)
            {
                var grandParent = returnedExpression.Parent?.Parent;

                switch (grandParent)
                {
                    case BlockSyntax block when block.Parent is IfStatementSyntax i1 && i1.Condition.IsNullCheck(identifier):
                    case IfStatementSyntax i2 when i2.Condition.IsNullCheck(identifier):
                    {
                        // we seem to have a check for null that avoids returning null
                        return;
                    }
                }
            }

            var dataFlow = context.SemanticModel.AnalyzeDataFlow(returnedExpression);

            var localVariableNames = dataFlow.ReadInside.ToHashSet(_ => _.Name);

            var candidates = GetCandidates(method, localVariableNames).ToHashSet();

            if (candidates.Count != 0)
            {
                // we found 'null' candidates
                AnalyzeAssignments(context, candidates, reported); // TODO RKN: Inspect expression
            }

            // the conditional has to be inspected independent of any variable candidates
            if (returnedExpression is ConditionalExpressionSyntax conditional)
            {
                AnalyzeConditional(context, conditional, reported);
            }
        }

        private void AnalyzeAssignments(in SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignments, ISet<SyntaxNode> reported)
        {
            if (assignments.Any(HasIssue))
            {
                var assignmentsWithIssues = new List<ExpressionSyntax>();

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var assignment in assignments)
                {
                    if (HasIssue(assignment) && IsOverwritten(assignment) is false && assignment.AncestorsWithinMethods().Any(_ => _.IsAnyKind(ImportantAncestors)))
                    {
                        assignmentsWithIssues.Add(assignment);
                    }
                }

                if (assignmentsWithIssues.Count > 0)
                {
                    ReportIssues(context, assignmentsWithIssues, reported);
                }
            }
            else
            {
                foreach (var conditional in assignments.OfType<ConditionalExpressionSyntax>())
                {
                    AnalyzeConditional(context, conditional, reported);
                }
            }
        }

        private void AnalyzeConditional(in SyntaxNodeAnalysisContext context, ConditionalExpressionSyntax conditional, ISet<SyntaxNode> reported)
        {
            var issues = GetIssues(context, conditional);

            ReportIssues(context, issues, reported);
        }

        private void ReportIssues(in SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignmentsWithIssues, ISet<SyntaxNode> reported)
        {
            foreach (var assignment in assignmentsWithIssues)
            {
                ReportIssue(context, assignment, reported);
            }
        }

        private void ReportIssue(in SyntaxNodeAnalysisContext context, SyntaxNode node, ISet<SyntaxNode> reported)
        {
            // the same node may be found via multiple return statements, so report it only once
            if (reported.Add(node))
            {
                ReportDiagnostics(context, Issue(node));
            }
        }
    }
}