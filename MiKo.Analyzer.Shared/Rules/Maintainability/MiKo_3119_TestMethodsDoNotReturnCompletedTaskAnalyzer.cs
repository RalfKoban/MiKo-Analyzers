using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3119_TestMethodsDoNotReturnCompletedTaskAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3119";

        public MiKo_3119_TestMethodsDoNotReturnCompletedTaskAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.IsAsync)
            {
                return false;
            }

            var returnType = symbol.ReturnType;

            if (returnType.IsGeneric())
            {
                return false;
            }

            return returnType.IsTask() && symbol.IsTestMethod();
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation) => symbol.GetSyntax() is MethodDeclarationSyntax methodSyntax && HasIssue(methodSyntax)
                                                                                                             ? new[] { Issue(methodSyntax.ReturnType) }
                                                                                                             : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(MethodDeclarationSyntax methodSyntax)
        {
            var expressionBody = methodSyntax.ExpressionBody;

            if (expressionBody != null)
            {
                return HasIssue(expressionBody.Expression);
            }

            var returnStatements = methodSyntax.Body.DescendantNodes<ReturnStatementSyntax>();

            return returnStatements.Any(_ => _.ReturnsCompletedTask());
        }

        private static bool HasIssue(ExpressionSyntax expression) => expression is MemberAccessExpressionSyntax maes && maes.Expression.GetName() == nameof(Task) && maes.GetName() == nameof(Task.CompletedTask);
    }
}