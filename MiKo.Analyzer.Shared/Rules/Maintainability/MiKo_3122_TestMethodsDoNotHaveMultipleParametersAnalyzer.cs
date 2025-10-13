using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3122";

        public MiKo_3122_TestMethodsDoNotHaveMultipleParametersAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax method && method.IsTestMethod())
            {
                var parameterList = method.ParameterList;

                if (parameterList.Parameters.Count > 3)
                {
                    ReportDiagnostics(context, Issue(parameterList));
                }
            }
        }
    }
}