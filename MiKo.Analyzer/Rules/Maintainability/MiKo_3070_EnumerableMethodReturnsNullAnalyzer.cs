using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

            var semanticModel = context.SemanticModel;

            if (methodBody.IsNullExpression(semanticModel))
            {
                ReportIssue(context, methodBody);
                return;
            }

            if (methodBody is BlockSyntax block)
            {
                var controlFlow = semanticModel.AnalyzeControlFlow(methodBody);
                var returnStatements = controlFlow.ReturnStatements.OfType<ReturnStatementSyntax>();
                foreach (var returnStatement in returnStatements)
                {
                    if (returnStatement.Expression.IsNullExpression(semanticModel))
                    {
                        ReportIssue(context, returnStatement);
                    }
                    else
                    {
                        var dataFlow = semanticModel.AnalyzeDataFlow(returnStatement);
                        foreach (var localVariable in dataFlow.ReadInside)
                        {
                            var assignments = GetAssignments(localVariable.Name, block.Statements);

                            foreach (var assignment in assignments)
                            {
                                if (assignment.IsNullExpression(semanticModel))
                                {
                                    ReportIssue(context, assignment);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<ExpressionSyntax> GetAssignments(string variableName, IEnumerable<StatementSyntax> statements)
        {
            foreach (var statement in statements)
            {
                if (statement is LocalDeclarationStatementSyntax ldss)
                {
                    var declaration = ldss.Declaration;
                    foreach (var variable in declaration.Variables)
                    {
                        if (variable.Identifier.ValueText == variableName)
                        {
                            yield return variable.Initializer.Value;
                        }
                    }
                }

                if (statement is ExpressionStatementSyntax ess)
                {
                    yield return ess.Expression;
                }
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