using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6004";

        public MiKo_6004_VariableAssignmentPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleAssignmentExpression);
        }

        private static bool IsAssignmentOrDeclaration(AssignmentExpressionSyntax assignment, StatementSyntax statement)
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

        private static bool IsLocalSymbol(IdentifierNameSyntax syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.Identifier.GetSymbol(semanticModel);

            return symbol?.Kind == SymbolKind.Local;
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var node = (AssignmentExpressionSyntax)context.Node;
            var issue = Analyze(node, context.SemanticModel);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic Analyze(AssignmentExpressionSyntax assignment, SemanticModel semanticModel)
        {
            if (assignment.Left is IdentifierNameSyntax i)
            {
                foreach (var ancestor in assignment.Ancestors())
                {
                    switch (ancestor.Kind())
                    {
                        case SyntaxKind.Block:
                            return IsLocalSymbol(i, semanticModel)
                                   ? Analyze(assignment, ((BlockSyntax)ancestor).Statements)
                                   : null;

                        case SyntaxKind.SwitchSection:
                            return IsLocalSymbol(i, semanticModel)
                                   ? Analyze(assignment, ((SwitchSectionSyntax)ancestor).Statements)
                                   : null;

                        case SyntaxKind.IfStatement:
                        case SyntaxKind.ElseClause:
                            return null; // no issue

                        // initializers
                        case SyntaxKind.ObjectInitializerExpression:
                        case SyntaxKind.CollectionInitializerExpression:
                        case SyntaxKind.ArrayInitializerExpression:
                        case SyntaxKind.ComplexElementInitializerExpression:
                        case SyntaxKind.WithInitializerExpression:
                            return null; // no issue

                        // lambdas
                        case SyntaxKind.SimpleLambdaExpression:
                        case SyntaxKind.ParenthesizedLambdaExpression:
                            return null; // no issue

                        case SyntaxKind.LocalFunctionStatement:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // base methods
                        case SyntaxKind.ConversionOperatorDeclaration:
                        case SyntaxKind.ConstructorDeclaration:
                        case SyntaxKind.DestructorDeclaration:
                        case SyntaxKind.MethodDeclaration:
                        case SyntaxKind.OperatorDeclaration:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // base types
                        case SyntaxKind.RecordDeclaration:
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.InterfaceDeclaration:
                        case SyntaxKind.StructDeclaration:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic Analyze(AssignmentExpressionSyntax assignment, SyntaxList<StatementSyntax> statements)
        {
            var callLineSpan = assignment.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements.Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                               .Any(_ => IsAssignmentOrDeclaration(assignment, _) is false);

            if (noBlankLinesBefore)
            {
                return Issue(assignment.Left, true, false);
            }

            return null;
        }
    }
}