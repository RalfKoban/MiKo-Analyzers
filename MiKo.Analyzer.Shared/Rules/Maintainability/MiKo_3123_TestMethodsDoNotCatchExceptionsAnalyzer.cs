using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3123";

        public MiKo_3123_TestMethodsDoNotCatchExceptionsAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);

        private void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is CatchClauseSyntax catchClause)
            {
                var method = catchClause.FirstAncestor<MethodDeclarationSyntax>();

                if (method != null && method.IsTestMethod())
                {
                    ReportDiagnostics(context, Issue(catchClause.CatchKeyword));
                }
            }
        }
    }
}