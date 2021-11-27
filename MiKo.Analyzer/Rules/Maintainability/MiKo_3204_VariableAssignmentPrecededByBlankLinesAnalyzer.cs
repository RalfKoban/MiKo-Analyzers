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

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
        }

        private static bool IsAssignmentOrDeclaration(StatementSyntax statement, AssignmentExpressionSyntax assignment)
        {
            if (statement.DescendantNodes().Contains(assignment))
            {
                // it's our own assignment that we check
                return true;
            }

            switch (statement)
            {
                case LocalDeclarationStatementSyntax _:
                    return true; // the statement is a variable declaration

                case ExpressionStatementSyntax e when e.Expression.IsKind(SyntaxKind.SimpleAssignmentExpression):
                    return true; // it is another assignment expression

                default:
                    return false;
            }
        }

        private static bool IsLocalSymbol(SimpleNameSyntax syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.Identifier.GetSymbol(semanticModel);

            return symbol.Kind == SymbolKind.Local;
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
                foreach (var ancestor in statement.Ancestors())
                {
                    switch (ancestor)
                    {
                        case BlockSyntax block:
                            return IsLocalSymbol(i, semanticModel)
                                       ? AnalyzeSimpleAssignmentExpression(block.Statements, statement)
                                       : null;

                        case SwitchSectionSyntax section:
                            return IsLocalSymbol(i, semanticModel)
                                       ? AnalyzeSimpleAssignmentExpression(section.Statements, statement)
                                       : null;

                        case IfStatementSyntax _:
                        case ElseClauseSyntax _:
                        case ParenthesizedLambdaExpressionSyntax _:
                        case InitializerExpressionSyntax _:
                            return null; // no issue

                        case MethodDeclarationSyntax _:
                        case ClassDeclarationSyntax _:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeSimpleAssignmentExpression(SyntaxList<StatementSyntax> statements, AssignmentExpressionSyntax statement)
        {
            var callLineSpan = statement.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements
                                     .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                     .Any(_ => IsAssignmentOrDeclaration(_, statement) is false);

            if (noBlankLinesBefore)
            {
                return Issue(statement, true, false);
            }

            return null;
        }
    }
}