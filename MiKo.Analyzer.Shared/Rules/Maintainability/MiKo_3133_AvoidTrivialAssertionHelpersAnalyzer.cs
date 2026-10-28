using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3133_AvoidTrivialAssertionHelpersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3133";

        public MiKo_3133_AvoidTrivialAssertionHelpersAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);

        private static bool HasIssue(MethodDeclarationSyntax method)
        {
            if (method.Body is BlockSyntax body)
            {
                var statements = body.Statements;

                if (statements.Count is 1 && statements[0] is ExpressionStatementSyntax statement)
                {
                    return HasIssue(statement.Expression);
                }
            }
            else if (method.ExpressionBody is ArrowExpressionClauseSyntax expressionBody)
            {
                return HasIssue(expressionBody.Expression);
            }

            return false;
        }

        private static bool HasIssue(ExpressionSyntax expression)
        {
            var identifierName = expression.GetStartingIdentifierName();

            return Constants.Names.AssertionTypes.Contains(identifierName);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax method && HasIssue(method) && method.IsTestMethod() is false)
            {
                ReportDiagnostics(context, Issue(method));
            }
        }
    }
}