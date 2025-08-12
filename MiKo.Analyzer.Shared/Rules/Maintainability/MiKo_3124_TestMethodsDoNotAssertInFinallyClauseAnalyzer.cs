using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3124_TestMethodsDoNotAssertInFinallyClauseAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3124";

        public MiKo_3124_TestMethodsDoNotAssertInFinallyClauseAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeFinallyClause, SyntaxKind.FinallyClause);

        private static bool IsAssert(ExpressionStatementSyntax statement) => statement.Expression is InvocationExpressionSyntax i && i.GetIdentifierName().EndsWith("Assert", StringComparison.Ordinal);

        private void AnalyzeFinallyClause(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is FinallyClauseSyntax clause)
            {
                var method = clause.FirstAncestor<MethodDeclarationSyntax>();

                if (method != null)
                {
                    var issues = clause.DescendantNodes<ExpressionStatementSyntax>()
                                       .Where(IsAssert)
                                       .Select(_ => Issue(_))
                                       .ToList();

                    if (issues.Count > 0)
                    {
                        ReportDiagnostics(context, issues);
                    }
                }
            }
        }
    }
}