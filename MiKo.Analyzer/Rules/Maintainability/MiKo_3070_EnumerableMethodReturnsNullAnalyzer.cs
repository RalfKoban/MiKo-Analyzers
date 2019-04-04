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

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private static bool CanBeIgnored(SyntaxNodeAnalysisContext context) => CanBeIgnored(context.GetEnclosingMethod());

        private static bool CanBeIgnored(IMethodSymbol method)
        {
            switch (method?.ReturnType.SpecialType)
            {
                case SpecialType.System_Collections_IEnumerable:
                case SpecialType.System_Collections_Generic_IEnumerable_T:
                    return false;

                default:
                    return true;
            }
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;

            Analyze(context, methodBody);
        }

        private void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody)
        {
            if (methodBody is null)
                return;

            if (CanBeIgnored(context))
                return;

            if (methodBody is ObjectCreationExpressionSyntax)
                return;

            if (HasIssue(methodBody, context))
            {
                ReportIssue(context, methodBody);
            }
            else if (methodBody is BlockSyntax block)
            {
                AnalyzeBlock(context, block);
            }
            else if (methodBody is ConditionalExpressionSyntax conditional)
            {
                AnalyzeAssignments(context, conditional.DescendantNodes().OfType<ExpressionSyntax>());
            }
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            var semanticModel = context.SemanticModel;

            var controlFlow = semanticModel.AnalyzeControlFlow(block);
            var returnStatements = controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>();

            foreach (var returnStatement in returnStatements)
            {
                if (HasIssue(returnStatement.Expression, context))
                {
                    ReportIssue(context, returnStatement);
                }
                else
                {
                    var dataFlow = semanticModel.AnalyzeDataFlow(returnStatement);
                    var localVariableNames = dataFlow.ReadInside.Select(_ => _.Name).ToHashSet();

                    var candidates = GetCandidates(block, localVariableNames).ToList();
                    if (candidates.Any())
                    {
                        AnalyzeAssignments(context, candidates);
                    }
                }
            }
        }

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
                }
            }
        }

        private void AnalyzeAssignments(SyntaxNodeAnalysisContext context, IEnumerable<ExpressionSyntax> assignments)
        {
            var assignmentsWithIssues = new List<ExpressionSyntax>();

            var isNull = false;

            foreach (var assignment in assignments)
            {
                isNull = HasIssue(assignment, context) || assignment.AncestorsAndSelf().Any(_ => _ is IfStatementSyntax || _ is ConditionalExpressionSyntax || _ is SwitchStatementSyntax);

                if (isNull)
                    assignmentsWithIssues.Add(assignment);
            }

            if (isNull)
            {
                foreach (var assignment in assignmentsWithIssues)
                {
                    ReportIssue(context, assignment);
                }
            }
        }

        private void ReportIssue(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            var name = node.ToString();
            var diagnostic = ReportIssue(name, node.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }

        private static bool HasIssue(SyntaxNode node, SyntaxNodeAnalysisContext context) => node.IsNullExpression(context.SemanticModel);
    }
}