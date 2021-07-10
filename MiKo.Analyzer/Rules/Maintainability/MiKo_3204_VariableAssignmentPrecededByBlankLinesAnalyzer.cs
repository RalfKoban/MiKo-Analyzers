using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3204_VariableAssignmentPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3204";

        public MiKo_3204_VariableAssignmentPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
        }

        private static bool IsAssignmentOrDeclaration(StatementSyntax statement, AssignmentExpressionSyntax assignment, string identifierName)
        {
            if (statement.DescendantNodes().Contains(assignment))
            {
                // it's our own assignment that we check
                return true;
            }

            if (statement is LocalDeclarationStatementSyntax)
            {
                // the statement is a variable declaration
                return true;
            }

            if (statement is ExpressionStatementSyntax e && e.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                // it is another assignment expression
                return true;
            }

            return false;
        }

        private void AnalyzeSimpleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var node = (AssignmentExpressionSyntax)context.Node;

            var diagnostic = AnalyzeSimpleAssignmentExpression(node, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeSimpleAssignmentExpression(AssignmentExpressionSyntax statement, SemanticModel semanticModel)
        {
            if (statement.Left is IdentifierNameSyntax i)
            {
                var identifierName = i.GetName();

                if (statement.GetAllUsedVariables(semanticModel).Contains(identifierName))
                {
                    var callLineSpan = statement.GetLocation().GetLineSpan();

                    foreach (var ancestor in statement.Ancestors())
                    {
                        if (ancestor is ParenthesizedLambdaExpressionSyntax)
                        {
                            // no issue
                            break;
                        }

                        if (ancestor is BlockSyntax block)
                        {
                            var noBlankLinesBefore = block.Statements
                                                          .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                                          .Any(_ => IsAssignmentOrDeclaration(_, statement, identifierName) is false);

                            if (noBlankLinesBefore)
                            {
                                return Issue(statement, true, false);
                            }

                            break;
                        }

                        if (ancestor is MethodDeclarationSyntax || ancestor is ClassDeclarationSyntax)
                        {
                            // stop lookup as there is no valid ancestor anymore
                            break;
                        }
                    }
                }
            }

            return null;
        }
    }
}